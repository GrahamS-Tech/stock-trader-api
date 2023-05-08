using NHibernate;
using NHibernate.Context;
using ISession = NHibernate.ISession;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
