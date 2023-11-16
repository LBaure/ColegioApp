using Core.Repositories;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ConnectionProvider : IConnectionProvider
    {
        public readonly IConfiguration _configuration;
        public ConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IDbConnection> OpenAsync()
        {
            var connection = new MySqlConnection(_configuration.GetConnectionString("MySqlConnection"));
            await connection.OpenAsync();
            return connection;
        }
    }
}
