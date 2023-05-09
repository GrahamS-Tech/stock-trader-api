using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using ISession = NHibernate.ISession;

namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class LoginController
{
    ISession session;

    public LoginController(ISession session)
    {
        this.session = session;
    }

    [HttpGet("{enteredEmail}, {enteredPassword}")]
    public string Get(string enteredEmail, string enteredPassword)
    {
        
        var profileData = session.Query<profile>().Where(e => e.EmailAddress == enteredEmail).FirstOrDefault();
        //var loginAttempts = session.Query<login_log>().Where(l => l.ProfileId == profileData.ProfileId);
        int failedLogins = 0;
        var returnMessage = "";
        bool loginSuccessful = false;


        if (profileData is null)
        {
            return "User account not found";
        }
        else
        {
            IList<login_log> logins = new List<login_log>();
            logins = profileData.LoginLog;
            IEnumerable<login_log> sortedLogins = logins.OrderBy(i => i.Id);

            foreach (var loginAttempts in sortedLogins.TakeLast(5))
            {
                if (loginAttempts.Successful == false)
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
        else if (enteredPassword != profileData.Password) {
            returnMessage = "Incorrect password";
            loginSuccessful = false;
        }
        else {
            returnMessage = "Login successful";
            loginSuccessful = true;
        }
            var newLoginLog = new login_log();
            newLoginLog.ProfileId = profileData;
            newLoginLog.TimeStamp = DateTime.Now;
            newLoginLog.Successful = loginSuccessful;
            profileData.LoginLog.Add(newLoginLog);
            session.SaveOrUpdate(profileData);
            session.Flush();

        return returnMessage;


    }
}
