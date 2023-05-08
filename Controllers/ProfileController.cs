using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using ISession = NHibernate.ISession;


namespace StockTraderAPI.Controllers;


[Route("api/[controller]")]
[ApiController]

public class ProfileController
{
    ISession session;

    public ProfileController(ISession session)
    {
        this.session = session;
    }

    [HttpGet("{email}")]
    public string Get(string email)
    {
        var profileData = session.Query<profile>().Where(e => e.EmailAddress == email).FirstOrDefault();

        if (profileData == null)
        {
            return "User account not found";
        }

        return $" {profileData.ProfileId}, {profileData.EmailAddress}, {profileData.Password}, {profileData.ProfileIsActive} ";
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

}
