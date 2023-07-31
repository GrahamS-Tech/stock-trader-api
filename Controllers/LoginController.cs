using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StockTraderAPI.Adapters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ISession = NHibernate.ISession;

namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class LoginController : ControllerBase
{
    private string Key { get; }
    private string Issuer { get; }
    private string Audience { get; }

    private ISession session;

    public LoginController(ISession session, IOptions<JwtInfo> options)
    {
        this.session = session;
        Key = options.Value.Key;
        Issuer = options.Value.Issuer;
        Audience = options.Value.Audience;
    }

    [AllowAnonymous]
    [HttpGet("{enteredEmail}")]

    public string Get(string enteredEmail) 
    {
        var profileData = session.Query<profile>()
            .Where(e => e.EmailAddress == enteredEmail.ToLower())
            .FirstOrDefault();
        var password = profileData.Password;
        var salt = profileData.Salt;
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return ($"Password:{password}, Salt:{salt}, Hashed Password:{hashedPassword}, JwtKey:{Key}");

    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login([FromBody] login userProfile)
    {

        var profileData = session.Query<profile>()
            .Where(e => e.EmailAddress == userProfile.EmailAddress.ToLower())
            .FirstOrDefault();
        int failedLogins = 0;
        var returnMessage = "";
        bool loginSuccessful = false;
        var hashedPassword = "";


        if (profileData is null)
        {
            return Unauthorized("User account not found");
        }
        else if (userProfile.Password == null || userProfile.Password == "")
        {
            return Ok(profileData.Salt);
        }
        else
        {
            IList<login_log> logins = new List<login_log>();
            logins = profileData.LoginLog;
            IEnumerable<login_log> sortedLogins = logins.OrderBy(i => i.Id);
            hashedPassword = BCrypt.Net.BCrypt.HashPassword(profileData.Password, profileData.Salt);

            foreach (var loginAttempts in sortedLogins.TakeLast(5))
            {
                if (!loginAttempts.Successful)
                {
                    failedLogins++;
                }
            }
        }

        if (failedLogins > 4)
        {
            returnMessage = "Too many failed login attempts. Reset password to continue";
            loginSuccessful = false;
            profileData.ForcePasswordReset = true;
        }
        else if (profileData.ProfileIsActive != true)
        {
            returnMessage = "This account is no longer active";
            loginSuccessful = false;
        }
        else if (profileData.ForcePasswordReset == true)
        {
            returnMessage = "You must reset your password before logging in";
            loginSuccessful = false;
        }
        else if (userProfile.Password != profileData.Password)
        {
            returnMessage = "Incorrect password";
            loginSuccessful = false;
        }
        else
        {
            userProfile.Id = profileData.ProfileId.ToString();
            var token = GenerateToken(userProfile);
            returnMessage = token;
            loginSuccessful = true;
        }

        var newLoginLog = new login_log();
        newLoginLog.ProfileId = profileData;
        newLoginLog.TimeStamp = DateTime.Now;
        newLoginLog.Successful = loginSuccessful;
        profileData.LoginLog.Add(newLoginLog);
        session.SaveOrUpdate(profileData);
        session.Flush();

        return loginSuccessful ? Ok(returnMessage) : Unauthorized(returnMessage);
    }

    private string GenerateToken(login userProfile)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, userProfile.EmailAddress),
            new Claim(ClaimTypes.NameIdentifier, userProfile.Id.ToString())
        };

        var token = new JwtSecurityToken(Issuer,
            Audience,
            claims,
            expires: DateTime.Now.AddMinutes(15), 
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
