using System.Collections.Generic;
using System.Linq;

namespace AnonimizarDados.Dtos;

public class Parametros
{
    public IEnumerable<ParametrosAtualizacaoDeValor> AtualizarPorValor { get; set; }
    
    public IEnumerable<ParametrosAtualizacaoDeRegra> AtualizarPorRegra { get; set; }
    
    public IEnumerable<ParametrosExclusao> Excluir { get; set; }
    
    public IEnumerable<ParametrosExclusao> Truncar { get; set; }
    
    public Parametros()
    {
        AtualizarPorValor = Enumerable.Empty<ParametrosAtualizacaoDeValor>();
        AtualizarPorRegra = Enumerable.Empty<ParametrosAtualizacaoDeRegra>();
        Excluir = Enumerable.Empty<ParametrosExclusao>();
        Truncar = Enumerable.Empty<ParametrosExclusao>();
    }
}