using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using ISession = NHibernate.ISession;

namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class SignupController : ControllerBase
{
ISession session;

public SignupController(ISession session)
{
    this.session = session;
}
    [AllowAnonymous]
    [HttpPost]
    public IActionResult Signup([FromBody] signup userProfile)
    {
        var profileData = session.Query<profile>()
            .Where(e => e.EmailAddress == userProfile.EmailAddress.ToLower());

        if (profileData.Count() != 0)
        {
            return NotFound("User account already exists");
        }

        else 
        {
            var newProfile = new profile();
            var salt = BCrypt.Net.BCrypt.GenerateSalt(12, BCrypt.Net.SaltRevision.Revision2B);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userProfile.Password, salt);

            newProfile.FirstName = userProfile.FirstName;
            newProfile.LastName = userProfile.LastName;
            newProfile.EmailAddress = userProfile.EmailAddress.ToLower();
            newProfile.Password = hashedPassword;
            newProfile.Salt = salt;
            newProfile.ProfileIsActive = true;
            newProfile.ForcePasswordReset = false;
            session.Save(newProfile);
            return Ok("Account created. Return to Log in tab to log in");
        }
    }
    }
