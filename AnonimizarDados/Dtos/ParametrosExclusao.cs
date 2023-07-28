namespace AnonimizarDados.Dtos;

public class ParametrosExclusao : TemplateParametrosDeBanco
{
    public ushort Prioridade { get; set; }

    public ParametrosExclusao()
    {
        Prioridade = 0;
    }
}