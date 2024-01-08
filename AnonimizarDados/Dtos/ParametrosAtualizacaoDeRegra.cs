using AnonimizarDados.Enums;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

public class ParametrosAtualizacaoDeRegraEqualityComparer : IEqualityComparer<ParametrosAtualizacaoDeRegra>
{
    public bool Equals(ParametrosAtualizacaoDeRegra? x, ParametrosAtualizacaoDeRegra? y)
    {
        return x?.NomeCompletoTabela == y?.NomeCompletoTabela;
    }

    public int GetHashCode([DisallowNull] ParametrosAtualizacaoDeRegra obj)
    {
        return obj.NomeCompletoTabela.GetHashCode();
    }
}