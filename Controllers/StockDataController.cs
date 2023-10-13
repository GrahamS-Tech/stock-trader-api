using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class StockDataController : ControllerBase
{
    private readonly IConfiguration _configuration;

    private readonly IHttpClientFactory _clientFactory;

    public StockDataController(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    [HttpGet("CurrentPrice")]
    [Authorize]
    public async Task<IActionResult> GetCurrentPrice(string ticker)
    {
        var apiKey = _configuration["AlphaVantageAPIKey"];
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null)
        {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonConvert.SerializeObject(response);
            return NotFound(jsonResponse);
        }

        var url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}&entitlement=delayed&apikey={apiKey}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        try {
            var client = _clientFactory.CreateClient("stockData");
            var data = await client.SendAsync(request);
            dynamic responseBody = await data.Content.ReadAsStringAsync();
            JObject result = JsonConvert.DeserializeObject<JObject>(responseBody);
            response.Data = result;        
            response.Status = "success";
            response.Message = "Stock data retrieved successfully";
        }
        catch (Exception err) {
            response.Status = "Error";
            response.Message = "Could not retrieve stock data";
            Console.WriteLine(err.Message);

        }
        
        jsonResponse = JsonConvert.SerializeObject(response);
        if (response.Status == "success")
        {
            return Ok(jsonResponse);
        }
        else { 
            return BadRequest(jsonResponse);
        }
        
    }

    [HttpGet("PriceHistory")]
    [Authorize]
    public async Task<IActionResult> GetPriceHistory(string ticker)
    {
        var apiKey = Environment.GetEnvironmentVariable("AlphaVantageAPIKey");
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null)
        {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonConvert.SerializeObject(response);
            return NotFound(jsonResponse);
        }

        var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={ticker}&interval=15min&entitlement=delayed&apikey={apiKey}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        try
        {
            var client = _clientFactory.CreateClient("stockData");
            var data = await client.SendAsync(request);
            dynamic responseBody = await data.Content.ReadAsStringAsync();
            JObject result = JsonConvert.DeserializeObject<JObject>(responseBody);
            response.Data = result;
            response.Status = "success";
            response.Message = "Stock data retrieved successfully";
        }
        catch (Exception err)
        {
            response.Status = "Error";
            response.Message = "Could not retrieve stock data";
            Console.WriteLine(err.Message);

        }

        jsonResponse = JsonConvert.SerializeObject(response);
        if (response.Status == "success")
        {
            return Ok(jsonResponse);
        }
        else
        {
            return BadRequest(jsonResponse);
        }

    }

    private login ValidateUser()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        if (identity is not null)
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