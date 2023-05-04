using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace StockTraderAPI.Adapters
{
    public class NHibernateConnection
    {
        public static Configuration InitiateConnection()
        {
            //var dbConnString = Config.GetConnectionString("ConnectionStrings:myconnstring");

            var cfg = new Configuration()
                .DataBaseIntegration(db =>
                {
                    db.Driver<NpgsqlDriver>();
                    db.Dialect<PostgreSQLDialect>();
                    db.ConnectionString = "Server=pohst-st.postgres.database.azure.com;Database=postgres;Port=5432;User Id=kendric;Password=Plat!num12;Ssl Mode=VerifyFull";
                });

            return cfg;
        }

    }
}
