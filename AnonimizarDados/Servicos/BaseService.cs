using System.Data.Common;
using AnonimizarDados.ValueObjects;
using Dapper;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Threading;
using System.Threading.Tasks;
using AnonimizarDados.Adapters.Contrato;

namespace AnonimizarDados.Servicos
{
    public abstract class BaseService
    {
        internal IConnection _connection;
        internal readonly DbConnection _conexao;
        internal readonly QueryFactory _queryFactory;

        public BaseService(IConnection connection, Compiler compiler)
        {
            _connection = connection;
            _conexao = connection.Create();
            _queryFactory = new QueryFactory(_conexao, compiler, _conexao.ConnectionTimeout);
        }

        public async Task<bool> VerificarExistenciaTabela(
            DefinicaoTabela tabela,
            CancellationToken cancellationToken)
        {
            var tabelaExiste = await _connection.TableExists(_conexao, tabela.NomeEsquema, tabela.NomeTabela);

            if (!tabelaExiste)
            {
                LogService.Info($"Tabela não encontrada: {tabela.NomeCompletoTabela}");
            }

            return tabelaExiste;
        }
    }
}
