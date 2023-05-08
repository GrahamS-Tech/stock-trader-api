using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using StockTraderAPI.Adapters;
using ISession = NHibernate.ISession;

namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class SignupController
{
ISession session;

public SignupController(ISession session)
{
    this.session = session;
}
    [HttpPost]
    public string Post(string enteredFirstName, string enteredLastName, string enteredEmail, string enteredPassword)
    {
        var profileData = session.Query<profile>().Where(e => e.EmailAddress == enteredEmail);

        if (profileData.Count() != 0)
        {
            return "User account already exists";
        }

        else 
        {
            var newProfile = new profile();

            newProfile.FirstName = enteredFirstName;
            newProfile.LastName = enteredLastName;
            newProfile.EmailAddress = enteredEmail;
            newProfile.Password = enteredPassword;
            newProfile.ProfileIsActive = true;
            newProfile.ForcePasswordReset = false;
            session.Save(newProfile);
            return "Account created";
        }
    }
    }
