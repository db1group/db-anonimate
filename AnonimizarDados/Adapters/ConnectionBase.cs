using System.Data.Common;
using System.Threading.Tasks;

namespace AnonimizarDados.Adapters
{
    public class ConnectionBase
    {
        public async Task OpenAsync(DbConnection connection)
        {
            await connection.OpenAsync();
        }

        public async Task CloseAsync(DbConnection connection)
        {
            await connection.CloseAsync();
        }

        public async Task ExecuteAsync(DbConnection connection, string query)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();
        }

        public async Task ExecuteAsync(DbConnection connection, string query, params object[] parameters)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddRange(parameters);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<T?> ExecuteScalarAsync<T>(DbConnection connection, string query)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            return (T?) await command.ExecuteScalarAsync();
        }

        public async Task<T?> ExecuteScalarAsync<T>(DbConnection connection, string query, params object[] parameters)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddRange(parameters);
            return (T?) await command.ExecuteScalarAsync();
        }
    }
}