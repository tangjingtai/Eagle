using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using Util.Datas.Sql;

namespace Util.WebApi.Datas
{
    public class MySqlDatabase : IDatabase
    {
        IConfiguration _configuration;
        
        public MySqlDatabase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetConnection()
        {
            return new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
