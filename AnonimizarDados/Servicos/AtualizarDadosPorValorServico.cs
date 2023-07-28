using AnonimizarDados.Dtos;
using Microsoft.Data.SqlClient;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnonimizarDados.Servicos;

public class AtualizarDadosPorValorServico
{
    private readonly QueryFactory _queryFactory;
    
    public AtualizarDadosPorValorServico(string stringConexao)
    {
        var conexao = new SqlConnection(stringConexao);
        var compilador = new SqlServerCompiler();
        _queryFactory = new QueryFactory(conexao, compilador);
    }

    public async Task AtualizarAsync(
        IEnumerable<ParametrosAtualizacaoDeValor> parametrosParaAtualizar,
        CancellationToken cancellationToken)
    {
        var parametrosAgrupados = AgruparParametrosPorTabela(parametrosParaAtualizar);
        
        foreach (var parametroParaAtualizar in parametrosAgrupados)
        {
            if (cancellationToken.IsCancellationRequested) return;
            
            LogService.Info($"Atualizando: {parametroParaAtualizar.NomeCompletoTabela}");
            
            await _queryFactory.Query(parametroParaAtualizar.NomeCompletoTabela)
                .UpdateAsync(
                    parametroParaAtualizar.ColunaValor,
                    cancellationToken: cancellationToken);
        }
    }

    private static IEnumerable<Agrupamento> AgruparParametrosPorTabela(
        IEnumerable<ParametrosAtualizacaoDeValor> parametrosParaAtualizar)
    {
        return parametrosParaAtualizar.GroupBy(
                p => new { p.NomeCompletoTabela },
                p => new KeyValuePair<string, object?>(p.Coluna, p.Valor.Equals("null") ? null : p.Valor),
                (g, p) => new Agrupamento(g.NomeCompletoTabela, p))
            .ToList();
    }

    private class Agrupamento
    {
        public string NomeCompletoTabela { get; }
        
        public IEnumerable<KeyValuePair<string, object?>> ColunaValor { get; }

        public Agrupamento(
            string nomeCompletoTabela,
            IEnumerable<KeyValuePair<string, object?>> colunaValor)
        {
            NomeCompletoTabela = nomeCompletoTabela;
            ColunaValor = colunaValor;
        }
    }
}