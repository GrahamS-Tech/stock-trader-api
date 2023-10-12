using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NHibernate;
using NHibernate.Context;
using StockTraderAPI.Controllers;
using System.Text;
using ISession = NHibernate.ISession;
using System.Diagnostics;
using NHibernate.Hql.Ast;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o => {
    o.AddPolicy(name: "_allowedOrigins", policy => {
        policy.WithOrigins("https://localhost:44450")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure <JwtInfo>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.FromSeconds(5)
    };
});

var connString = builder.Configuration.GetConnectionString("azurePostgres");
var userId = "";
var password = "";
if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("IDENTITY_ENDPOINT")))
{
    userId = builder.Configuration["azurePostgres:UserId"];
    password = builder.Configuration["azurePostgres:Password"];
}
else
{
    userId = "User Id=azure_stock_trader_api";
    var credential = new DefaultAzureCredential();
    var token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://ossrdbms-aad.database.windows.net/.default" }));
    password = "Password=" + token.Token;
}
    
builder.Services.AddSingleton<ISessionFactory>((provider) => { 
var cfg = new NHibernate.Cfg.Configuration();
    cfg.Configure(".\\Adapters\\Mappings\\hibernate.cfg.xml");
    cfg.SetProperty("connection.connection_string", connString +";" + userId + ";" + password);
    cfg.CurrentSessionContext<WebSessionContext>();
    return cfg.BuildSessionFactory();
});
builder.Services.AddScoped<ISession>((provider) => {
    var session = provider.GetService<ISessionFactory>();
    return session.OpenSession();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("_allowedOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapControllers();

app.Run();
