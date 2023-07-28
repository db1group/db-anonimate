namespace AnonimizarDados.Dtos;

public abstract class TemplateParametrosDeBanco
{
    public string Esquema { get; set; }
    
    public string Tabela { get; set; }

    public string NomeCompletoTabela => $"{Esquema}.{Tabela}";

    protected TemplateParametrosDeBanco()
    {
        Esquema = string.Empty;
        Tabela = string.Empty;
    }
}