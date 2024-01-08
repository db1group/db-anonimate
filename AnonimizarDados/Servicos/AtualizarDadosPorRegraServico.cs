using AnonimizarDados.Dtos;
using AnonimizarDados.Enums;
using AnonimizarDados.ValueObjects;
using Dapper;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnonimizarDados.Servicos;

public class AtualizarDadosPorRegraServico : BaseService
{
    public AtualizarDadosPorRegraServico(string stringConexao)
        : base(stringConexao)
    { }

    public async Task AtualizarAsync(
        IEnumerable<ParametrosAtualizacaoDeRegra> parametrosParaAtualizar,
        CancellationToken cancellationToken)
    {
        var parametrosAgrupados = AgruparParametrosPorTabela(parametrosParaAtualizar);

        foreach (var parametroParaAtualizar in parametrosAgrupados)
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (!await VerificarExistenciaTabela(parametroParaAtualizar.Tabela, cancellationToken)) continue;

            LogService.Info($"Anonimizando: {parametroParaAtualizar.Tabela.NomeCompletoTabela}");

            var quantidadeDeRegistros = await _queryFactory
                .Query(parametroParaAtualizar.Tabela.NomeCompletoTabela)
                .CountAsync<int>(cancellationToken: cancellationToken);

            var colunasRegras = parametroParaAtualizar.ColunaRegra.ToArray();
            var controle = new Controle(quantidadeDeRegistros);

            var sbCommand = new StringBuilder();
            var sbUpdate = new StringBuilder();

            while (controle.Resto > 0)
            {
                if (cancellationToken.IsCancellationRequested) return;

                var ids = await _queryFactory.Query(parametroParaAtualizar.Tabela.NomeCompletoTabela)
                    .Select($"{parametroParaAtualizar.ColunaId}")
                    .OrderBy($"{parametroParaAtualizar.ColunaId}")
                    .Limit(controle.Quantidade)
                    .Skip(controle.Pular)
                    .GetAsync<int>(cancellationToken: cancellationToken);

                var builder = CriarBuilder(colunasRegras);
                var dataEnumerator = builder.Criar().Generate(controle.Quantidade).GetEnumerator();
                var idsEnumerator = ids.GetEnumerator();

                for (var i = 0; i < controle.Quantidade; i++)
                {
                    dataEnumerator.MoveNext();
                    idsEnumerator.MoveNext();

                    if (cancellationToken.IsCancellationRequested) return;

                    var entidade = dataEnumerator.Current;
                    var id = idsEnumerator.Current;

                    sbUpdate.Clear();

                    sbUpdate.Append($"UPDATE {parametroParaAtualizar.Tabela.NomeCompletoTabela} SET ");

                    var sets = string.Join(", ", colunasRegras.Select(s => $"{s.Key} = '{RegraService.ObterValor(entidade, s.Value)}'"));
                    sbUpdate.Append(sets);

                    sbUpdate.Append($" WHERE {parametroParaAtualizar.ColunaId} = {id}");

                    sbCommand.AppendLine(sbUpdate.ToString());
                }

                await _conexao.ExecuteAsync(
                    sbCommand.ToString(),
                    cancellationToken);

                controle.AtualizarControle();

                sbCommand.Clear();
            }
        }
    }

    private static BuilderEntidadeFicticia CriarBuilder(IEnumerable<KeyValuePair<string, RegrasEnum>> colunas)
    {
        var regrasDistintas = colunas.Select(c => c.Value).Distinct();

        var builder = new BuilderEntidadeFicticia();

        foreach (var regra in regrasDistintas)
        {
            RegraService.GerarBuilder(builder, regra);
        }

        return builder;
    }

    private static IEnumerable<Agrupamento> AgruparParametrosPorTabela(
        IEnumerable<ParametrosAtualizacaoDeRegra> parametrosParaAtualizar)
    {
        return parametrosParaAtualizar.GroupBy(
                keySelector: p => p,
                elementSelector: p => new KeyValuePair<string, RegrasEnum>(p.Coluna, p.Regra),
                resultSelector: (g, p) => new Agrupamento(g, g.ColunaId, p),
                comparer: new ParametrosAtualizacaoDeRegraEqualityComparer())
            .ToList();
    }

    private class Agrupamento
    {
        public string ColunaId { get; }

        public DefinicaoTabela Tabela { get; }

        public IEnumerable<KeyValuePair<string, RegrasEnum>> ColunaRegra { get; }

        public Agrupamento(
            TemplateParametrosDeBanco informacoesTabela,
            string colunaId,
            IEnumerable<KeyValuePair<string, RegrasEnum>> colunaRegra)
        {
            Tabela = new DefinicaoTabela(informacoesTabela);
            ColunaId = colunaId;
            ColunaRegra = colunaRegra;
        }
    }

    private class Controle
    {
        public int Quantidade { get; private set; }

        public int Pular { get; private set; }

        public int Resto { get; private set; }

        public Controle(
            int resto,
            short quantidade = 3_000,
            short pular = 0)
        {
            Quantidade = quantidade;
            Pular = pular;
            Resto = resto;
        }

        public void AtualizarControle()
        {
            Pular += Quantidade;
            Resto -= Quantidade;

            Quantidade = Resto > Quantidade ?
                Quantidade :
                Resto;
        }
    }
}