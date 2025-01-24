using AnonimizarDados.Dtos;
using AnonimizarDados.Servicos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnonimizarDados.Adapters.Contrato;
using AnonimizarDados.Adapters.Postgres;
using AnonimizarDados.Adapters.SqlServer;
using AnonimizarDados.Enums;
using SqlKata.Compilers;

namespace AnonimizarDados;

internal static class Program
{
    private static readonly CancellationTokenSource TokenSource = new();

    private static async Task Main()
    {
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            Console.WriteLine("Operação cancelada.");
            TokenSource.Cancel();
            eventArgs.Cancel = true;
        };

        var configuracao = LerArquivoDeConfiguracao();

        var parametros = configuracao.GetSection("Parametros").Get<Parametros>();
        var tipoBanco = configuracao.GetSection("TipoBanco").Get<TipoBanco>();
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

        IConnection connection = tipoBanco switch
        {
            TipoBanco.SqlServer => new ConnectionSqlServer(stringConexao),
            TipoBanco.Postgres => new ConnectionPostgres(stringConexao),
            _ => throw new NotImplementedException()
        };

        Compiler compiler = tipoBanco switch
        {
            TipoBanco.SqlServer => new SqlServerCompiler(),
            TipoBanco.Postgres => new PostgresCompiler(),
            _ => throw new NotImplementedException()
        };

        if (!TestarConexaoComBanco(connection))
        {
            return;
        }

        if (parametros.AtualizarPorValor.Any())
        {
            await new AtualizarDadosPorValorServico(connection, compiler).AtualizarAsync(
                parametros.AtualizarPorValor,
                TokenSource.Token);
        }

        if (parametros.AtualizarPorRegra.Any())
        {
            await new AtualizarDadosPorRegraServico(connection, compiler).AtualizarAsync(
                parametros.AtualizarPorRegra,
                TokenSource.Token);
        }

        if (parametros.Excluir.Any())
        {
            await new ExcluirDadosServico(connection, compiler).ExcluirAsync(
                parametros.Excluir,
                TokenSource.Token);
        }

        if (parametros.Truncar.Any())
        {
            await new ExcluirDadosServico(connection, compiler).TruncarAsync(
                parametros.Truncar,
                TokenSource.Token);
        }

        Console.WriteLine("Operação finalizada. Pressione qualquer tecla para encerrar.");
        Console.ReadKey();
    }

    private static IConfiguration LerArquivoDeConfiguracao()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true);

        return configurationBuilder.Build();
    }

    private static bool TestarConexaoComBanco(IConnection connection)
    {
        using var conexao = connection.Create();

        try
        {
            conexao.Open();

                return true;
            // if (conexao.Database.EndsWith("_RANDOM"))
            //
            // Console.WriteLine("Banco de dados inválido. Aponte para um banco de dados com final '_RANDOM'.");
            //
            // return false;
        }
        catch (Exception ex)
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