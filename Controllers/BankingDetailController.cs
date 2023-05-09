using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;
using StockTraderAPI.Adapters;
using System.Text.Json;

namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class BankingDetailController
{
    ISession session;

    public BankingDetailController(ISession session)
    {
        this.session = session;
    }

    //Get by profile id returns all banking entries for user, including id returns a single entry
    [HttpGet("{profile_id}")]
    public string Get(int profile_id, int id)
    {
        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);
        
        IList<banking_detail> bankingDetails = new List<banking_detail>();
        bankingDetails = profileData.BankingDetails;
        List<banking_detail> requestedBankingDetail = new List<banking_detail>();
        var jsonBankingDetail = "";

        if (id == 0)
        {

            foreach (var bankingDetail in bankingDetails)
            {
                banking_detail banking_Detail = new banking_detail()
                {
                    Id = bankingDetail.Id,
                    AccountType = bankingDetail.AccountType,
                    AccountNumber = bankingDetail.AccountNumber,
                    RoutingNumber = bankingDetail.RoutingNumber,
                    DateAdded = bankingDetail.DateAdded,
                    IsActive = bankingDetail.IsActive
                };

                requestedBankingDetail.Add(banking_Detail);
            }

            jsonBankingDetail = JsonSerializer.Serialize(requestedBankingDetail);
            return jsonBankingDetail;
        }

        else
        {
            IEnumerable<banking_detail> selectedBankingDetail = bankingDetails.Where(i => i.Id == id);

            foreach (var bankingDetail in selectedBankingDetail)
            {
                banking_detail banking_Detail = new banking_detail()
                {
                    Id = bankingDetail.Id,
                    AccountType = bankingDetail.AccountType,
                    AccountNumber = bankingDetail.AccountNumber,
                    RoutingNumber = bankingDetail.RoutingNumber,
                    DateAdded = bankingDetail.DateAdded,
                    IsActive = bankingDetail.IsActive
                };

                requestedBankingDetail.Add(banking_Detail);
            }

            jsonBankingDetail = JsonSerializer.Serialize(requestedBankingDetail);
            return jsonBankingDetail;
        }
    }

    [HttpPost("{profile_id}")]
    public string Post(int profile_id, string enteredAccountType, string enteredAccountNumber, string enteredRoutingNumber)
    {
        profile profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);
        banking_detail newBankingDetails = new banking_detail();
        newBankingDetails.ProfileId = profileData;
        newBankingDetails.AccountType = enteredAccountType;
        newBankingDetails.AccountNumber = enteredAccountNumber;
        newBankingDetails.RoutingNumber = enteredRoutingNumber;
        newBankingDetails.DateAdded = DateTime.Now;
        newBankingDetails.IsActive = true;
        profileData.BankingDetails.Add(newBankingDetails);
        session.Save(newBankingDetails);
        session.Flush();

        return "Account successfully added";
    }

    [HttpPut("{profile_id}, {id}")]
    public string Put(int profile_id, int id)
{
        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);

        IList<banking_detail> bankingDetails = new List<banking_detail>();
        bankingDetails = profileData.BankingDetails;
        IEnumerable<banking_detail> selectedBankingDetail = bankingDetails.Where(i => i.Id == id);

        var returnMessage = "";

        foreach (var bankingDetail in selectedBankingDetail)
        {
            bankingDetail.IsActive = false;
            session.SaveOrUpdate(bankingDetail);
            session.Flush();

            returnMessage = "Account removed";
        }

        return returnMessage;
}

}