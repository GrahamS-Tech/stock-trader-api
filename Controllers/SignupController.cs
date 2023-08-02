using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Text.Json;
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
        var response = new api_response<object> { };
        var profileData = session.Query<profile>()
        .Where(e => e.EmailAddress == userProfile.EmailAddress.ToLower());

        if (profileData.Count() != 0)
        {
            response.Status = "error";
            response.Message = "User account already exists";
            response.Data = "";
            var jsonResponse = JsonSerializer.Serialize(response);
            return BadRequest(jsonResponse);
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
            newProfile.SignupDate = DateTime.UtcNow;
            session.Save(newProfile);

            response.Status = "success";
            response.Message = "Account created";
            response.Data = "";
            var jsonResponse = JsonSerializer.Serialize(response);
            return Ok(jsonResponse);
        }
    }
    }
