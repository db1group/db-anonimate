using AnonimizarDados.Dtos;
using AnonimizarDados.ValueObjects;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnonimizarDados.Servicos;

public class AtualizarDadosPorValorServico : BaseService
{
    public AtualizarDadosPorValorServico(string stringConexao)
        : base(stringConexao)
    { }

    public async Task AtualizarAsync(
        IEnumerable<ParametrosAtualizacaoDeValor> parametrosParaAtualizar,
        CancellationToken cancellationToken)
    {
        var parametrosAgrupados = AgruparParametrosPorTabela(parametrosParaAtualizar);

        foreach (var parametroParaAtualizar in parametrosAgrupados)
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (!await VerificarExistenciaTabela(parametroParaAtualizar.Tabela, cancellationToken)) continue;

            LogService.Info($"Atualizando: {parametroParaAtualizar.Tabela.NomeCompletoTabela}");

            await _queryFactory.Query(parametroParaAtualizar.Tabela.NomeCompletoTabela)
                .UpdateAsync(
                    parametroParaAtualizar.ColunaValor,
                    cancellationToken: cancellationToken);
        }
    }

    private static IEnumerable<Agrupamento> AgruparParametrosPorTabela(
        IEnumerable<ParametrosAtualizacaoDeValor> parametrosParaAtualizar)
    {
        return parametrosParaAtualizar.GroupBy(
                keySelector: p => p,
                elementSelector: p => new KeyValuePair<string, object?>(p.Coluna, p.Valor.Equals("null") ? null : p.Valor),
                resultSelector: (g, p) => new Agrupamento(g, p),
                comparer: new ParametrosAtualizacaoDeValorEqualityComparer())
            .ToList();
    }

    private class Agrupamento
    {
        public DefinicaoTabela Tabela { get; }

        public IEnumerable<KeyValuePair<string, object?>> ColunaValor { get; }

        public Agrupamento(
            TemplateParametrosDeBanco informacoesTabela,
            IEnumerable<KeyValuePair<string, object?>> colunaValor)
        {
            Tabela = new DefinicaoTabela(informacoesTabela);
            ColunaValor = colunaValor;
        }
    }
}