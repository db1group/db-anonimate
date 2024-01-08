using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AnonimizarDados.Dtos;

public class ParametrosAtualizacaoDeValor : TemplateParametrosDeTabela
{
    public string Valor { get; set; }

    public ParametrosAtualizacaoDeValor()
    {
        Valor = string.Empty;
    }
}

public class ParametrosAtualizacaoDeValorEqualityComparer : IEqualityComparer<ParametrosAtualizacaoDeValor>
{
    public bool Equals(ParametrosAtualizacaoDeValor? x, ParametrosAtualizacaoDeValor? y)
    {
        return x?.NomeCompletoTabela == y?.NomeCompletoTabela;
    }

    public int GetHashCode([DisallowNull] ParametrosAtualizacaoDeValor obj)
    {
        return obj.NomeCompletoTabela.GetHashCode();
    }
}