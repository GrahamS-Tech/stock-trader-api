﻿using Microsoft.AspNetCore.Authorization;
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
    public IActionResult GetProfileDetails()
    {
        var currentUser = ValidateUser();

        if (currentUser.Id == null)
        {
            return NotFound("User account not found");
        }

        var userId = int.Parse(currentUser.Id);
        var profileData = session.Query<profile>().Where(e => e.ProfileId == userId).FirstOrDefault();
        var currentProfile = new profile();
        currentProfile.ProfileId = profileData.ProfileId;
        currentProfile.FirstName = profileData.FirstName;
        currentProfile.LastName = profileData.LastName;
        currentProfile.EmailAddress = profileData.EmailAddress;
        currentProfile.Address1 = profileData.Address1;
        currentProfile.Address2 = profileData.Address2;
        currentProfile.City = profileData.City;
        currentProfile.State = profileData.State;
        currentProfile.PostalCode = profileData.PostalCode;
        currentProfile.ProfileIsActive = profileData.ProfileIsActive;
        currentProfile.ProfileComplete = profileData.ProfileComplete;
        return Ok(currentProfile);
    }

    [HttpPut("ProfileUpdate")]
    [Authorize]
    public IActionResult ProfileUpdate([FromBody] profile userProfile)
    {
        var currentUser = ValidateUser();

        if (currentUser.Id == null)
        {
            return NotFound("User account not found");
        }
        Console.WriteLine(userProfile);
        var profileUpdate = new profile();
        var userId = int.Parse(currentUser.Id);
        profileUpdate = session.Query<profile>().Where(e => e.ProfileId == userId).FirstOrDefault();
        profileUpdate.FirstName = userProfile.FirstName;
        profileUpdate.LastName = userProfile.LastName;
        profileUpdate.EmailAddress = userProfile.EmailAddress;
        profileUpdate.Address1 = userProfile.Address1;
        profileUpdate.Address2 = userProfile.Address2;
        profileUpdate.City = userProfile.City;
        profileUpdate.State = userProfile.State;
        profileUpdate.PostalCode = userProfile.PostalCode;
        profileUpdate.ProfileComplete = userProfile.ProfileComplete;
        session.SaveOrUpdate(profileUpdate);
        session.Flush();

        return Ok();
    }

    private login ValidateUser()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        if (identity != null)
        { 
            var userClaims = identity.Claims;

            return new login
            {
                Id = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value
            };
        }
        return null;
    }

}
