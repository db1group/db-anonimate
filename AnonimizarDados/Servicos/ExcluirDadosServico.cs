using AnonimizarDados.Dtos;
using Dapper;
using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnonimizarDados.Servicos;

public class ExcluirDadosServico
{
    private readonly SqlConnection _conexao;
    private readonly QueryFactory _queryFactory;
    
    public ExcluirDadosServico(string stringConexao)
    {
        _conexao = new SqlConnection(stringConexao);
        var compilador = new SqlServerCompiler();
        _queryFactory = new QueryFactory(_conexao, compilador);
    }

    public async Task ExcluirAsync(
        IEnumerable<ParametrosExclusao> parametrosParaExcluir,
        CancellationToken cancellationToken)
    {
        foreach (var parametroParaExcluir in parametrosParaExcluir.OrderBy(o => o.Prioridade))
        {
            if (cancellationToken.IsCancellationRequested) return;
            
            LogService.Info($"Excluindo: {parametroParaExcluir.NomeCompletoTabela}");
            
            await _queryFactory.Query(parametroParaExcluir.NomeCompletoTabela)
                .DeleteAsync(cancellationToken: cancellationToken);
        }
    }
    
    public async Task TruncarAsync(
        IEnumerable<ParametrosExclusao> parametrosParaExcluir,
        CancellationToken cancellationToken)
    {
        foreach (var parametroParaExcluir in parametrosParaExcluir.OrderBy(o => o.Prioridade))
        {
            if (cancellationToken.IsCancellationRequested) return;
            
            LogService.Info($"Truncando: {parametroParaExcluir.NomeCompletoTabela}");
            
            await _conexao.ExecuteAsync($"TRUNCATE TABLE {parametroParaExcluir.NomeCompletoTabela}", cancellationToken);
        }
    }
}