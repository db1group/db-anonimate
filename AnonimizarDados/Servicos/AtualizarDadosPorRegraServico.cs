using AnonimizarDados.Dtos;
using AnonimizarDados.Enums;
using Dapper;
using Microsoft.Data.SqlClient;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnonimizarDados.Servicos;

public class AtualizarDadosPorRegraServico
{
    private const string NomeColunaLinha = "Linha";
    private const string NomeCommonTableExpression = "TMP";
    
    private readonly SqlConnection _conexao;
    private readonly QueryFactory _queryFactory;
    
    public AtualizarDadosPorRegraServico(string stringConexao)
    {
        _conexao = new SqlConnection(stringConexao);
        var compilador = new SqlServerCompiler();
        _queryFactory = new QueryFactory(_conexao, compilador);
    }
    
    public async Task AtualizarAsync(
        IEnumerable<ParametrosAtualizacaoDeRegra> parametrosParaAtualizar,
        CancellationToken cancellationToken)
    {
        var parametrosAgrupados = AgruparParametrosPorTabela(parametrosParaAtualizar);
        
        foreach (var parametroParaAtualizar in parametrosAgrupados)
        {
            if (cancellationToken.IsCancellationRequested) return;
            
            LogService.Info($"Anonimizando: {parametroParaAtualizar.NomeCompletoTabela}");
            
            await Solucao2(parametroParaAtualizar, cancellationToken);
        }
    }

    /// <summary>
    /// Com uso do CTE, executar a instrução UPDATE registro a registro partindo de um ROW_NUMBER
    /// Foi identificado como a solução menos performática
    /// </summary>
    [Obsolete("Não possui um bom desempenho")]
    private async Task Solucao1(
        Agrupamento parametroParaAtualizar,
        CancellationToken cancellationToken)
    {
        var quantidadeDeRegistros = await _queryFactory
            .Query(parametroParaAtualizar.NomeCompletoTabela)
            .CountAsync<int>(cancellationToken: cancellationToken);
        
        var colunasEnvolvidas = parametroParaAtualizar.ColunaRegra
            .Select(s => s.Key)
            .ToArray();
        
        var querywith = new Query(parametroParaAtualizar.NomeCompletoTabela)
            .SelectRaw($"ROW_NUMBER() OVER (ORDER BY {parametroParaAtualizar.ColunaId}) AS {NomeColunaLinha}");
        
        foreach (var coluna in colunasEnvolvidas)
        {
            querywith.Select(coluna);
        }
        
        for (var i = 1; i <= quantidadeDeRegistros; i++)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var colunasValores = parametroParaAtualizar.ColunaRegra
                .Select(s => new KeyValuePair<string, object>(s.Key, RegraService.GerarValor(s.Value)));

            await _queryFactory.Query(NomeCommonTableExpression)
                .With(NomeCommonTableExpression, querywith)
                .Where(NomeColunaLinha, "=", i)
                .UpdateAsync(
                    colunasValores,
                    cancellationToken: cancellationToken);
        }
    }

    private async Task Solucao2(
        Agrupamento parametroParaAtualizar,
        CancellationToken cancellationToken)
    {
        var quantidadeDeRegistros = await _queryFactory
            .Query(parametroParaAtualizar.NomeCompletoTabela)
            .CountAsync<int>(cancellationToken: cancellationToken);
        
        var colunasRegras = parametroParaAtualizar.ColunaRegra.ToArray();
        var controle = new Controle(quantidadeDeRegistros);
        
        var sbCommand = new StringBuilder();
        var sbUpdate = new StringBuilder();
        
        while (controle.Resto > 0)
        {
            if (cancellationToken.IsCancellationRequested) return;
            
            var ids = await _queryFactory.Query(parametroParaAtualizar.NomeCompletoTabela)
                .Select($"{parametroParaAtualizar.ColunaId}")
                .OrderBy($"{parametroParaAtualizar.ColunaId}")
                .Limit(controle.Quantidade)
                .Skip(controle.Pular)
                .GetAsync<int>(cancellationToken: cancellationToken);
            
            foreach (var id in ids)
            {
                if (cancellationToken.IsCancellationRequested) return;
                
                sbUpdate.Clear();
                
                sbUpdate.Append($"UPDATE {parametroParaAtualizar.NomeCompletoTabela} SET ");

                var sets = string.Join(", ", colunasRegras.Select(s => $"{s.Key} = '{RegraService.GerarValor(s.Value)}'"));
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
    
    private static IEnumerable<Agrupamento> AgruparParametrosPorTabela(
        IEnumerable<ParametrosAtualizacaoDeRegra> parametrosParaAtualizar)
    {
        return parametrosParaAtualizar.GroupBy(
                p => new { p.NomeCompletoTabela, p.ColunaId },
                p => new KeyValuePair<string, RegrasEnum>(p.Coluna, p.Regra),
                (g, p) => new Agrupamento(g.NomeCompletoTabela, g.ColunaId, p))
            .ToList();
    }

    private class Agrupamento
    {
        public string ColunaId { get; }
        
        public string NomeCompletoTabela { get; }
        
        public IEnumerable<KeyValuePair<string, RegrasEnum>> ColunaRegra { get; }

        public Agrupamento(
            string nomeCompletoTabela,
            string colunaId,
            IEnumerable<KeyValuePair<string, RegrasEnum>> colunaRegra)
        {
            NomeCompletoTabela = nomeCompletoTabela;
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
            short quantidade = 5_000,
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