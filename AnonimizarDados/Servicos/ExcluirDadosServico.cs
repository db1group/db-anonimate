using AnonimizarDados.Dtos;
using AnonimizarDados.ValueObjects;
using Dapper;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnonimizarDados.Servicos;

public class ExcluirDadosServico : BaseService
{
    public ExcluirDadosServico(string stringConexao)
        : base(stringConexao)
    { }

    public async Task ExcluirAsync(
        IEnumerable<ParametrosExclusao> parametrosParaExcluir,
        CancellationToken cancellationToken)
    {
        foreach (var parametroParaExcluir in parametrosParaExcluir.OrderBy(o => o.Prioridade))
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (!await VerificarExistenciaTabela(new DefinicaoTabela(parametroParaExcluir), cancellationToken)) continue;

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

            if (!await VerificarExistenciaTabela(new DefinicaoTabela(parametroParaExcluir), cancellationToken)) continue;

            LogService.Info($"Truncando: {parametroParaExcluir.NomeCompletoTabela}");

            await _conexao.ExecuteAsync($"TRUNCATE TABLE {parametroParaExcluir.NomeCompletoTabela}", cancellationToken);
        }
    }
}