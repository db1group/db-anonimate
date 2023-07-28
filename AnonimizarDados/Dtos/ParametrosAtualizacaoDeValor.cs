namespace AnonimizarDados.Dtos;

public class ParametrosAtualizacaoDeValor : TemplateParametrosDeTabela
{
    public string Valor { get; set; }

    public ParametrosAtualizacaoDeValor()
    {
        Valor = string.Empty;
    }
}