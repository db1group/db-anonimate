using AnonimizarDados.Dtos;
using AnonimizarDados.Servicos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnonimizarDados;

internal static class Program
{
    private static readonly CancellationTokenSource TokenSource = new();
    
    private static async Task Main(string[] args)
    {
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            Console.WriteLine("Operação cancelada.");
            TokenSource.Cancel();
            eventArgs.Cancel = true;
        };

        var configuracao = LerArquivoDeConfiguracao();
        
        var parametros = configuracao.GetSection("Parametros").Get<Parametros>();
        var stringConexao = configuracao.GetConnectionString("Banco");
        
        if (parametros is null)
        {
            Console.WriteLine("Nenhum parâmetro registrado.");
            return;
        }
        
        if (string.IsNullOrEmpty(stringConexao))
        {
            Console.WriteLine("String de conexão não informada.");
            return;
        }

        if (!TestarConexaoComBanco(stringConexao))
        {
            return;
        }

        if (parametros.AtualizarPorValor.Any())
        {
            await new AtualizarDadosPorValorServico(stringConexao).AtualizarAsync(
                parametros.AtualizarPorValor,
                TokenSource.Token);
        }

        if (parametros.AtualizarPorRegra.Any())
        {
            await new AtualizarDadosPorRegraServico(stringConexao).AtualizarAsync(
                parametros.AtualizarPorRegra,
                TokenSource.Token);
        }
        
        if (parametros.Excluir.Any())
        {
            await new ExcluirDadosServico(stringConexao).ExcluirAsync(
                parametros.Excluir,
                TokenSource.Token);
        }
        
        if (parametros.Truncar.Any())
        {
            await new ExcluirDadosServico(stringConexao).TruncarAsync(
                parametros.Truncar,
                TokenSource.Token);
        }
        
        Console.WriteLine("Operação concluída. Pressione qualquer tecla para finalizar.");
        Console.ReadKey();
    }

    private static IConfiguration LerArquivoDeConfiguracao()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true);

        return configurationBuilder.Build();
    }

    private static bool TestarConexaoComBanco(string stringConexao)
    {
        using var conexao = new SqlConnection(stringConexao);
        
        try
        {
            conexao.Open();

            if (conexao.Database.EndsWith("_RANDOM"))
                return true;
            
            Console.WriteLine("Banco de dados inválido. Aponte para um banco de dados com final '_RANDOM'.");
            
            return false;
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Falha ao conectar no banco de dados.");
            Console.WriteLine(ex.Message);
            
            return false;
        }
        finally
        {
            conexao.Close();
        }
    }
}