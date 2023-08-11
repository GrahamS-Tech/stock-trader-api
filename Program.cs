using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NHibernate;
using NHibernate.Context;
using StockTraderAPI.Controllers;
using System.Text;
using ISession = NHibernate.ISession;

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
var connString = builder.Configuration["ConnectionStrings:azurePostgres"];
builder.Services.AddSingleton<ISessionFactory>((provider) => { 
var cfg = new NHibernate.Cfg.Configuration();
    cfg.Configure();
    cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connString);
    cfg.CurrentSessionContext<WebSessionContext>();
    return cfg.BuildSessionFactory();
});
builder.Services.AddScoped<ISession>((provider) => {
    var session = provider.GetService<ISessionFactory>();
    return session.OpenSession();
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
