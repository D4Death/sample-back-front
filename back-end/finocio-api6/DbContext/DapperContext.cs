using System;
using Npgsql;
using System.Data;

namespace sample_api6.DbContext
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("HostConnection");
        }
        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}

