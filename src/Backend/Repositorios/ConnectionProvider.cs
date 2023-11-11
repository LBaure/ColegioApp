using Core.Repositorios;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorios
{
    public class ConnectionProvider : IConnectionProvider
    {
        private readonly IConfiguration _configuration;
        public ConnectionProvider(IConfiguration configuration)
        {
            // 4 providers
            _configuration = configuration;
        }

        public async Task<IDbConnection> OpenAsync()
        {
            var connection = new OracleConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            return connection;
        }
    }
}
