namespace AnonimizarDados.Dtos;

public abstract class TemplateParametrosDeTabela : TemplateParametrosDeBanco
{
    public string Coluna { get; set; }

    protected TemplateParametrosDeTabela()
    {
        Coluna = string.Empty;
    }
}