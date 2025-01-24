using System.Data.Common;
using System.Threading.Tasks;
using AnonimizarDados.Adapters.Contrato;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AnonimizarDados.Adapters.SqlServer
{
    public class ConnectionSqlServer(string connectionString): ConnectionBase, IConnection
    {
        private string ConnectionString { get; } = connectionString;

        public DbConnection Create()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<bool> TableExists(DbConnection connection, string schema, string table)
        {
            var query = $@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_TYPE='BASE TABLE'
                    AND TABLE_SCHEMA = '{schema}'
                    AND TABLE_NAME = '{table}')
                SELECT 1
                ELSE
                SELECT 0";

            return await connection.ExecuteScalarAsync<bool>(query);
        }
    }
}