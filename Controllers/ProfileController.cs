using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using NHibernate.Cfg;
using NHibernate;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockTraderAPI.Controllers
{

[Route("api/[controller]")]
    [ApiController]

    public class ProfileController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public ProfileController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/Profile
        [HttpGet]
        public IEnumerable<string> Get()
        {

            var connectionString = _configuration.GetConnectionString("azurePostgres");
            Configuration cfg = new Configuration();
            cfg.Configure();
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);
            var sessionFactory = cfg.BuildSessionFactory();
            var session = sessionFactory.OpenSession();

            var profileData = session.Query<profile>().ToList();

            string firstNames = "";

            foreach (var profiles in profileData)
            {
                firstNames += profiles.FirstName;
            };

            return new string[] { firstNames };
        }

        // GET api/Profile/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/Profile
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/Profile/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/Profile/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
