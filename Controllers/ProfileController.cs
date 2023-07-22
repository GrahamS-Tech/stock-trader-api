using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Security.Claims;
using ISession = NHibernate.ISession;


namespace StockTraderAPI.Controllers;


[Route("api/[controller]")]
[ApiController]

public class ProfileController : ControllerBase
{
    ISession session;

    public ProfileController(ISession session)
    {
        this.session = session;
    }

    [HttpGet("ProfileDetails")]
    [Authorize]
    public IActionResult Get()
    {
        var currentUser = GetCurrentUser();

        if (currentUser.Id == null)
        {
            return NotFound("User account not found");
        }

        var userId = int.Parse(currentUser.Id);
        var profileData = session.Query<profile>().Where(e => e.ProfileId == userId).FirstOrDefault();
        return Ok($" {profileData.ProfileId}, {profileData.EmailAddress}, {profileData.Password}, {profileData.ProfileIsActive} ");
    }

    [HttpPut("{profile_id}")]
    public void Put(int profile_id, string newFirstName, string newLastName, string newEmailAddress, string newAddress1, string newAddress2, string newCity, string newState, string newPostalCode)
    {

        var profileUpdate = new profile();
        profileUpdate = session.Query<profile>().Where(p => p.ProfileId == profile_id).First();
        profileUpdate.FirstName = newFirstName;
        profileUpdate.LastName = newLastName;
        profileUpdate.EmailAddress = newEmailAddress;
        profileUpdate.Address1 = newAddress1;
        profileUpdate.Address2 = newAddress2;
        profileUpdate.City = newCity;
        profileUpdate.State = newState;
        profileUpdate.PostalCode = newPostalCode;
        session.SaveOrUpdate(profileUpdate);
        session.Flush();

    }

    private login GetCurrentUser()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        if (identity != null)
        { 
            var userClaims = identity.Claims;

            return new login
            {
                Id = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                EmailAddress = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value
            };
        }
        return null;
    }

}
