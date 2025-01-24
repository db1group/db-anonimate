using System.Data.Common;
using System.Threading.Tasks;
using AnonimizarDados.Adapters.Contrato;
using Dapper;
using Npgsql;

namespace AnonimizarDados.Adapters.Postgres
{
    public class ConnectionPostgres(string connectionString): ConnectionBase, IConnection
    {
        private string ConnectionString { get; } = connectionString;

        public DbConnection Create()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        public async Task<bool> TableExists(DbConnection connection, string schema, string table)
        {
            var query = $@"
                        SELECT
                            CASE
                                WHEN EXISTS (
                                    SELECT 1
                                    FROM information_schema.tables
                                    WHERE table_schema = '{schema}'
                                      AND table_name = '{table}'
                                )
                                THEN 1
                                ELSE 0
                            END AS table_exists;";

            return await connection.ExecuteScalarAsync<bool>(query);
        }
    }
}