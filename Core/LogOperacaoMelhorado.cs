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
        return $"{Comando},{SensorEsquerdo},{SensorDireito},{SensorFrente},{EstadoCarga}";
    }

    public string ToDebugLine()
    {
        // Linha extra para debug com mais informações
        return $"{NumeroComando:D3},{Comando},{PosicaoRobo},{DirecaoRobo},{SensorEsquerdo},{SensorDireito},{SensorFrente},{EstadoCarga}";
    }
}

/// <summary>
/// Sistema de log melhorado com funcionalidades avançadas.
/// </summary>
public class LogOperacaoMelhorado
{
    private readonly List<RegistroLogMelhorado> _registros;
    private readonly string _nomeArquivo;
    private readonly string _nomeArquivoDebug;
    private int _contadorComandos = 0;

    public LogOperacaoMelhorado(string nomeArquivoMapa)
    {
        _registros = new List<RegistroLogMelhorado>();
        _nomeArquivo = Path.ChangeExtension(nomeArquivoMapa, ".csv");
        _nomeArquivoDebug = Path.ChangeExtension(nomeArquivoMapa, "_debug.csv");
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
            // Salvar arquivo oficial (conforme especificação)
            using (var writer = new StreamWriter(_nomeArquivo))
            {
                foreach (var registro in _registros)
                {
                    writer.WriteLine(registro.ToCsvLine());
                }
            }

            // Salvar arquivo de debug (com informações extras)
            using (var writer = new StreamWriter(_nomeArquivoDebug))
            {
                writer.WriteLine("Seq,Comando,Posicao,Direcao,SensorEsq,SensorDir,SensorFrente,Carga");
                foreach (var registro in _registros)
                {
                    writer.WriteLine(registro.ToDebugLine());
                }
            }

            Console.WriteLine($"📝 Log oficial salvo em: {_nomeArquivo}");
            Console.WriteLine($"🐛 Log debug salvo em: {_nomeArquivoDebug}");
        }
        catch (Exception ex)
        {
            throw new DomainException($"Erro ao salvar log: {ex.Message}", ex);
        }
    }

    public void ExibirCaminhoPercorrido()
    {
        Console.WriteLine("\n🗺️ CAMINHO PERCORRIDO PELO ROBÔ:");
        Console.WriteLine("═══════════════════════════════════");

        for (int i = 0; i < _registros.Count; i++)
        {
            var registro = _registros[i];
            var icone = registro.Comando switch
            {
                EComandoRobo.LIGAR => "🔌",
                EComandoRobo.Avancar => "🚶",
                EComandoRobo.Girar90GrausDireita => "🔄",
                EComandoRobo.PegarHumano => "🤝",
                EComandoRobo.EjetarHumano => "🚀",
                _ => "❓"
            };

            var direcaoSeta = registro.DirecaoRobo switch
            {
                EDirecao.Norte => "↑",
                EDirecao.Leste => "→",
                EDirecao.Sul => "↓",
                EDirecao.Oeste => "←",
                _ => "?"
            };

            Console.WriteLine($"{registro.NumeroComando:D2}. {icone} {registro.Comando,-20} | Pos: {registro.PosicaoRobo} {direcaoSeta} | Sensores: E={registro.SensorEsquerdo} D={registro.SensorDireito} F={registro.SensorFrente} | {registro.EstadoCarga}");
        }
    }

    public void ExibirResumo()
    {
        Console.WriteLine("\n📋 RESUMO DA OPERAÇÃO:");
        Console.WriteLine($"   Total de comandos: {_registros.Count}");

        var comandosAvancar = _registros.Count(r => r.Comando == EComandoRobo.Avancar);
        var comandosGirar = _registros.Count(r => r.Comando == EComandoRobo.Girar90GrausDireita);
        var comandosPegar = _registros.Count(r => r.Comando == EComandoRobo.PegarHumano);
        var comandosEjetar = _registros.Count(r => r.Comando == EComandoRobo.EjetarHumano);

        Console.WriteLine($"   Avanços: {comandosAvancar}");
        Console.WriteLine($"   Giros: {comandosGirar}");
        Console.WriteLine($"   Coletas: {comandosPegar}");
        Console.WriteLine($"   Ejeções: {comandosEjetar}");

        var tempoComHumano = _registros.Count(r => r.EstadoCarga == EEstadoCarga.COM_HUMANO);
        Console.WriteLine($"   Comandos com humano coletado: {tempoComHumano}");

        // Informações sobre o percurso
        var posicoes = _registros.Select(r => r.PosicaoRobo).Distinct().ToList();
        Console.WriteLine($"   Posições únicas visitadas: {posicoes.Count}");

        if (_registros.Any(r => r.EstadoCarga == EEstadoCarga.COM_HUMANO))
        {
            Console.WriteLine("   ✅ Missão bem-sucedida: Humano encontrado!");
        }
    }
}
