using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StockTraderAPI.Adapters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
    [HttpPost]
    public IActionResult Login([FromBody] login userProfile)
    {
        var response = new api_response<object> { };
        var profileData = session.Query<profile>()
            .Where(e => e.EmailAddress == userProfile.EmailAddress.ToLower())
            .FirstOrDefault();
        int failedLogins = 0;
        bool loginSuccessful = false;
        var hashedPassword = "";
        var jsonResponse = "";


        if (profileData is null)
        {
            response.Status = "error";
            response.Message = "User account not found";
            response.Data = "";
            jsonResponse = JsonSerializer.Serialize(response);
            return Unauthorized(jsonResponse);
        }
        else if (userProfile.Password is null || userProfile.Password == "")
        {
            response.Status = "success";
            response.Message = "User salt found";
            response.Data = profileData.Salt;
            jsonResponse = JsonSerializer.Serialize(response);
            return Ok(jsonResponse);
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
            response.Status="error";
            response.Message = "Too many failed login attempts. Reset password to continue";
            loginSuccessful = false;
            profileData.ForcePasswordReset = true;
        }
        else if (profileData.ProfileIsActive != true)
        {
            response.Status = "error";
            response.Message = "This account is no longer active";
            loginSuccessful = false;
        }
        else if (profileData.ForcePasswordReset == true)
        {
            response.Status = "error";
            response.Message = "You must reset your password before logging in";
            loginSuccessful = false;
        }
        else if (userProfile.Password != profileData.Password)
        {
            response.Status = "error";
            response.Message = "Incorrect password";
            loginSuccessful = false;
        }
        else
        {
            userProfile.Id = profileData.ProfileId.ToString();
            var token = GenerateToken(userProfile);
            response.Status = "success";
            response.Data = token;
            loginSuccessful = true;
        }

        var newLoginLog = new login_log();
        newLoginLog.ProfileId = profileData;
        newLoginLog.TimeStamp = DateTime.UtcNow;
        newLoginLog.Successful = loginSuccessful;
        profileData.LoginLog.Add(newLoginLog);
        session.SaveOrUpdate(profileData);
        session.Flush();

        jsonResponse = JsonSerializer.Serialize(response);
        return loginSuccessful ? Ok(jsonResponse) : Unauthorized(jsonResponse);
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
