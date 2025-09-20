using RoboSalvamento.Core;

namespace RoboSalvamento.Core;

/// <summary>
/// Registro de log melhorado com informações adicionais para debug.
/// </summary>
public class RegistroLogMelhorado
{
    public EComandoRobo Comando { get; set; }
    public ELeituraSensor SensorEsquerdo { get; set; }
    public ELeituraSensor SensorDireito { get; set; }
    public ELeituraSensor SensorFrente { get; set; }
    public EEstadoCarga EstadoCarga { get; set; }

    // ✨ NOVOS CAMPOS PARA DEBUG
    public Posicao PosicaoRobo { get; set; } = null!;
    public EDirecao DirecaoRobo { get; set; }
    public int NumeroComando { get; set; }

    public string ToCsvLine()
    {
        // Ordem correta conforme o documento
        var estadoCarga = EstadoCarga.ToString().Replace("_", " ");
        return $"{Comando},{SensorEsquerdo},{SensorDireito},{SensorFrente},{estadoCarga}";
    }

}

/// <summary>
/// Sistema de log melhorado com funcionalidades avançadas.
/// </summary>
public class LogOperacao
{
    private readonly List<RegistroLogMelhorado> _registros;
    private readonly string _nomeArquivo;
    private int _contadorComandos = 0;

    public LogOperacao(string nomeArquivoMapa)
    {
        _registros = new List<RegistroLogMelhorado>();
        _nomeArquivo = Path.ChangeExtension(nomeArquivoMapa, ".csv");
    }

    public void AdicionarRegistro(RegistroLogMelhorado registro)
    {
        registro.NumeroComando = ++_contadorComandos;
        _registros.Add(registro);
    }

    public void SalvarArquivos()
    {
        try
        {
            using (var writer = new StreamWriter(_nomeArquivo))
            {
                foreach (var registro in _registros)
                {
                    writer.WriteLine(registro.ToCsvLine());
                }
            }

            Console.WriteLine($"📝 Log salvo em: {_nomeArquivo}");
        }
        catch (Exception ex)
        {
            throw new DomainException($"Erro ao salvar log: {ex.Message}", ex);
        }
    }

}
