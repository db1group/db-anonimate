using AnonimizarDados.Enums;

namespace AnonimizarDados.Dtos;

public class ParametrosAtualizacaoDeRegra : TemplateParametrosDeTabela
{
    public string ColunaId { get; set; }
    
    public RegrasEnum Regra { get; set; }

    public ParametrosAtualizacaoDeRegra()
    {
        ColunaId = string.Empty;
        Regra = RegrasEnum.Infefinido;
    }
}