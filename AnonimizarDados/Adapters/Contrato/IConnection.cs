using System.Data.Common;
using System.Threading.Tasks;

namespace AnonimizarDados.Adapters.Contrato
{
    public interface IConnection
    {
        DbConnection Create();
        Task<bool> TableExists(DbConnection connection, string schema, string table);
        Task OpenAsync(DbConnection connection);
        Task CloseAsync(DbConnection connection);
        Task ExecuteAsync(DbConnection connection, string query);
        Task ExecuteAsync(DbConnection connection, string query, params object[] parameters);
        Task<T?> ExecuteScalarAsync<T>(DbConnection connection, string query);
        Task<T?> ExecuteScalarAsync<T>(DbConnection connection, string query, params object[] parameters);
    }
}