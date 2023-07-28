using System;

namespace AnonimizarDados.Servicos;

public static class LogService
{
    public static void Info(string mensagem)
    {
        Console.WriteLine($"{DateTime.Now} - {mensagem}");
    }
}