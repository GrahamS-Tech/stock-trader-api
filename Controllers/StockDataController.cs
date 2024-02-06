using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

    [HttpGet("CurrentPrice/{ticker}")]
    [Authorize]
    public async Task<IActionResult> GetCurrentPrice(string ticker)
    {
        var apiKey = _configuration["AlphaVantageAPIKey"];
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null) {
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
        if (response.Status == "success") {
            return Ok(jsonResponse);
        }
        else {
            return BadRequest(jsonResponse);
        }
    }

    [HttpGet("IntradayPriceHistory/{ticker}")]
    [Authorize]
    public async Task<IActionResult> GetIntradayPriceHistory(string ticker)
    {
        var apiKey = Environment.GetEnvironmentVariable("AlphaVantageAPIKey");
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null) {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonConvert.SerializeObject(response);
            return NotFound(jsonResponse);
        }

        var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={ticker}&interval=15min&entitlement=delayed&apikey={apiKey}";
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
        if (response.Status == "success") {
            return Ok(jsonResponse);
        }
        else {
            return BadRequest(jsonResponse);
        }
    }

    [HttpGet("DailyPriceHistory/{ticker}")]
    [Authorize]
    public async Task<IActionResult> GetDailyPriceHistory(string ticker)
    {
        var apiKey = Environment.GetEnvironmentVariable("AlphaVantageAPIKey");
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null) {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonConvert.SerializeObject(response);
            return NotFound(jsonResponse);
        }

        var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={ticker}&interval=15min&entitlement=delayed&apikey={apiKey}";
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
        }

        jsonResponse = JsonConvert.SerializeObject(response);
        if (response.Status == "success") {
            return Ok(jsonResponse);
        }
        else {
            return BadRequest(jsonResponse);
        }
    }

    [HttpGet("MonthlyPriceHistory/{ticker}")]
    [Authorize]
    public async Task<IActionResult> GetMonthlyPriceHistory(string ticker)
    {
        var apiKey = Environment.GetEnvironmentVariable("AlphaVantageAPIKey");
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null) {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonConvert.SerializeObject(response);
            return NotFound(jsonResponse);
        }

        var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol={ticker}&apikey={apiKey}";
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
        }

        jsonResponse = JsonConvert.SerializeObject(response);
        if (response.Status == "success") {
            return Ok(jsonResponse);
        }
        else {
            return BadRequest(jsonResponse);
        }
    }

    [HttpGet("TopMovers")]
    [Authorize]
    public async Task<IActionResult> GetTopMovers()
    {
        var apiKey = Environment.GetEnvironmentVariable("AlphaVantageAPIKey");
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null) {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonConvert.SerializeObject(response);
            return NotFound(jsonResponse);
        }

        var url = $"https://www.alphavantage.co/query?function=TOP_GAINERS_LOSERS&apikey={apiKey}";
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
        }

        jsonResponse = JsonConvert.SerializeObject(response);
        if (response.Status == "success") {
            return Ok(jsonResponse);
        }
        else {
            return BadRequest(jsonResponse);
        }
    }

    [HttpGet("News")]
    [Authorize]
    public async Task<IActionResult> GetNews()
    {
        var apiKey = Environment.GetEnvironmentVariable("AlphaVantageAPIKey");
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null) {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonConvert.SerializeObject(response);
            return NotFound(jsonResponse);
        }

        var url = $"https://www.alphavantage.co/query?function=NEWS_SENTIMENT&apikey={apiKey}";
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
        }

        jsonResponse = JsonConvert.SerializeObject(response);
        if (response.Status == "success") {
            return Ok(jsonResponse);
        }
        else {
            return BadRequest(jsonResponse);
        }
    }

    [HttpGet("News/{ticker}")]
    [Authorize]
    public async Task<IActionResult> GetNewsByTicker(string ticker)
    {
        var apiKey = Environment.GetEnvironmentVariable("AlphaVantageAPIKey");
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null) {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonConvert.SerializeObject(response);
            return NotFound(jsonResponse);
        }

        var url = $"https://www.alphavantage.co/query?function=NEWS_SENTIMENT&tickers={ticker}&apikey={apiKey}";
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
        }

        jsonResponse = JsonConvert.SerializeObject(response);
        if (response.Status == "success") {
            return Ok(jsonResponse);
        }
        else {
            return BadRequest(jsonResponse);
        }
    }

    [HttpGet("News/{topic}")]
    [Authorize]
    public async Task<IActionResult> GetNewsByTopic(string topic)
    {
        var apiKey = Environment.GetEnvironmentVariable("AlphaVantageAPIKey");
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null) {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonConvert.SerializeObject(response);
            return NotFound(jsonResponse);
        }

        var url = $"https://www.alphavantage.co/query?function=NEWS_SENTIMENT&topics={topic}&apikey={apiKey}";
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
        catch (Exception) {
            response.Status = "Error";
            response.Message = "Could not retrieve stock data";
        }

        jsonResponse = JsonConvert.SerializeObject(response);
        if (response.Status == "success") {
            return Ok(jsonResponse);
        }
        else {
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