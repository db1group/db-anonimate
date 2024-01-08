using AnonimizarDados.ValueObjects;
using Dapper;
using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Threading;
using System.Threading.Tasks;

namespace AnonimizarDados.Servicos
{
    public abstract class BaseService
    {
        internal readonly SqlConnection _conexao;
        internal readonly QueryFactory _queryFactory;

        public BaseService(string stringConexao)
        {
            _conexao = new SqlConnection(stringConexao);
            var compilador = new SqlServerCompiler();
            _queryFactory = new QueryFactory(_conexao, compilador, _conexao.CommandTimeout);
        }

        public async Task<bool> VerificarExistenciaTabela(
            DefinicaoTabela tabela,
            CancellationToken cancellationToken)
        {
            var query = $@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_TYPE='BASE TABLE'
                    AND TABLE_SCHEMA = '{tabela.NomeEsquema}'
                    AND TABLE_NAME = '{tabela.NomeTabela}')
                SELECT 1
                ELSE
                SELECT 0";

            var tabelaExiste = await _conexao.ExecuteScalarAsync<bool>(query, cancellationToken);

            if (!tabelaExiste)
            {
                LogService.Info($"Tabela não encontrada: {tabela.NomeCompletoTabela}");
            }

            return tabelaExiste;
        }
    }
}
