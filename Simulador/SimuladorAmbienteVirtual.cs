using RoboSalvamento.Core;
using RoboSalvamento.Extensions;

namespace RoboSalvamento.Simulador;

/// <summary>
/// Simulador de ambiente virtual para o robô de salvamento.
/// Implementa todas as validações de segurança e gera logs auditáveis.
/// </summary>
public class SimuladorAmbienteVirtual
{
    private readonly Mapa _mapa;
    private Posicao _posicaoRobo = null!;
    private EDirecao _direcaoRobo;
    private bool _humanoColetado;
    private bool _missaoCompleta;
    private readonly LogOperacaoMelhorado _log;

    public SimuladorAmbienteVirtual(Mapa mapa)
    {
        _mapa = mapa ?? throw new ArgumentNullException(nameof(mapa));
        _log = new LogOperacaoMelhorado($"mapa_{DateTime.Now:yyyyMMdd_HHmmss}");
        InicializarRobo();
    }

    public Posicao PosicaoRobo => _posicaoRobo;
    public EDirecao DirecaoRobo => _direcaoRobo;
    public bool HumanoColetado => _humanoColetado;
    public bool MissaoCompleta => _missaoCompleta;
    public LogOperacaoMelhorado Log => _log;

    private void InicializarRobo()
    {
        _posicaoRobo = _mapa.Entrada;
        _direcaoRobo = DeterminarDirecaoInicial();
        _humanoColetado = false;
        _missaoCompleta = false;
    }

    private EDirecao DeterminarDirecaoInicial()
    {
        var entrada = _mapa.Entrada;

        // Verifica se está na borda e determina direção para o interior
        if (entrada.Linha == 0) return EDirecao.Sul; // Borda superior
        if (entrada.Linha == _mapa.QuantidadeDeLinhas - 1) return EDirecao.Norte; // Borda inferior
        if (entrada.Coluna == 0) return EDirecao.Leste; // Borda esquerda
        if (entrada.Coluna == _mapa.QuantidadeDeColunas - 1) return EDirecao.Oeste; // Borda direita

        throw new DomainException("Entrada não está na borda do labirinto!");
    }

    public RegistroLogMelhorado ExecutarComando(EComandoRobo comando)
    {
        switch (comando)
        {
            case EComandoRobo.LIGAR:
                return LerSensores(comando);

            case EComandoRobo.Avancar:
                return Avancar();

            case EComandoRobo.Girar90GrausDireita:
                return Girar();

            case EComandoRobo.PegarHumano:
                return PegarHumano();

            case EComandoRobo.EjetarHumano:
                return EjetarHumano();

            default:
                throw new DomainException($"Comando inválido: {comando}");
        }
    }

    private RegistroLogMelhorado Avancar()
    {
        var proximaPosicao = _direcaoRobo.ObterPosicaoFrente(_posicaoRobo);

        // 🚨 VALIDAÇÃO CRÍTICA: Verificar colisão com parede
        if (EhParede(proximaPosicao))
        {
            throw new DomainException("🚨 ALARME DE COLISÃO: Tentativa de avançar para uma parede!");
        }

        // 🚨 VALIDAÇÃO CRÍTICA: Verificar atropelamento de humano
        if (EhHumano(proximaPosicao) && !_humanoColetado)
        {
            throw new DomainException("🚨 ALARME DE ATROPELAMENTO: Tentativa de atropelar o humano!");
        }

        _posicaoRobo = proximaPosicao;
        return LerSensores(EComandoRobo.Avancar);
    }

    private RegistroLogMelhorado Girar()
    {
        _direcaoRobo = _direcaoRobo.GirarDireita();
        return LerSensores(EComandoRobo.Girar90GrausDireita);
    }

    private RegistroLogMelhorado PegarHumano()
    {
        var posicaoFrente = _direcaoRobo.ObterPosicaoFrente(_posicaoRobo);

        // 🚨 VALIDAÇÃO CRÍTICA: Verificar se há humano à frente
        if (!EhHumano(posicaoFrente))
        {
            throw new DomainException("🚨 ALARME DE COLETA INVÁLIDA: Tentativa de pegar humano sem humano à frente!");
        }

        _humanoColetado = true;
        return LerSensores(EComandoRobo.PegarHumano);
    }

    private RegistroLogMelhorado EjetarHumano()
    {
        // 🚨 VALIDAÇÃO CRÍTICA: Verificar se há humano para ejetar
        if (!_humanoColetado)
        {
            throw new DomainException("🚨 ALARME DE EJEÇÃO INVÁLIDA: Tentativa de ejetar humano sem humano coletado!");
        }

        // 🚨 VALIDAÇÃO CRÍTICA: Verificar se está na entrada (saída)
        if (!_posicaoRobo.Equals(_mapa.Entrada))
        {
            throw new DomainException("🚨 ALARME DE EJEÇÃO INVÁLIDA: Tentativa de ejetar fora da entrada!");
        }

        _humanoColetado = false;
        _missaoCompleta = true;
        return LerSensores(EComandoRobo.EjetarHumano);
    }

    private RegistroLogMelhorado LerSensores(EComandoRobo comando)
    {
        // 🚨 VALIDAÇÃO CRÍTICA: Verificar beco sem saída após coleta
        if (_humanoColetado && comando != EComandoRobo.PegarHumano)
        {
            var sensorEsq = LerSensorEsquerdo();
            var sensorDir = LerSensorDireito();
            var sensorFrente = LerSensorFrente();

            Console.WriteLine($"      🔍 Validação claustrofobia em {_posicaoRobo} direção {_direcaoRobo}: Esq={sensorEsq}, Dir={sensorDir}, Frente={sensorFrente}");
            Console.WriteLine($"      🔍 Comparação: Esq==PAREDE? {sensorEsq == ELeituraSensor.PAREDE}, Dir==PAREDE? {sensorDir == ELeituraSensor.PAREDE}, Frente==PAREDE? {sensorFrente == ELeituraSensor.PAREDE}");

            if (sensorEsq == ELeituraSensor.PAREDE &&
                sensorDir == ELeituraSensor.PAREDE &&
                sensorFrente == ELeituraSensor.PAREDE)
            {
                throw new DomainException("🚨 ALARME DE BECO SEM SAÍDA: Robô com humano coletado em beco sem saída (claustrofobia)!");
            }
        }

        return new RegistroLogMelhorado
        {
            Comando = comando,
            SensorEsquerdo = LerSensorEsquerdo(),
            SensorDireito = LerSensorDireito(),
            SensorFrente = LerSensorFrente(),
            EstadoCarga = _humanoColetado ? EEstadoCarga.COM_HUMANO : EEstadoCarga.SEM_CARGA,
            PosicaoRobo = new Posicao(_posicaoRobo.Linha, _posicaoRobo.Coluna), // Cópia da posição
            DirecaoRobo = _direcaoRobo
        };
    }

    private ELeituraSensor LerSensorEsquerdo()
    {
        var posicao = _direcaoRobo.ObterPosicaoEsquerda(_posicaoRobo);
        return ClassificarPosicao(posicao);
    }

    private ELeituraSensor LerSensorDireito()
    {
        var posicao = _direcaoRobo.ObterPosicaoDireita(_posicaoRobo);
        return ClassificarPosicao(posicao);
    }

    private ELeituraSensor LerSensorFrente()
    {
        var posicao = _direcaoRobo.ObterPosicaoFrente(_posicaoRobo);
        return ClassificarPosicao(posicao);
    }

    private ELeituraSensor ClassificarPosicao(Posicao posicao)
    {
        // Se está fora do mapa, verificar se é a entrada (deve ser VAZIO)
        if (EhForaDoMapa(posicao))
        {
            // Se o robô está na entrada e olhando para fora, é VAZIO
            if (_posicaoRobo.Equals(_mapa.Entrada))
            {
                return ELeituraSensor.VAZIO;
            }
            return ELeituraSensor.PAREDE;
        }

        if (EhParede(posicao))
            return ELeituraSensor.PAREDE;

        if (EhHumano(posicao) && !_humanoColetado)
            return ELeituraSensor.HUMANO;

        return ELeituraSensor.VAZIO;
    }

    private bool EhForaDoMapa(Posicao posicao)
    {
        return posicao.Linha < 0 || posicao.Linha >= _mapa.QuantidadeDeLinhas ||
               posicao.Coluna < 0 || posicao.Coluna >= _mapa.QuantidadeDeColunas;
    }

    private bool EhParede(Posicao posicao)
    {
        if (EhForaDoMapa(posicao)) return true;
        return _mapa.Labirinto[posicao.Linha, posicao.Coluna] == 'X';
    }

    private bool EhHumano(Posicao posicao)
    {
        if (EhForaDoMapa(posicao)) return false;
        return _mapa.Labirinto[posicao.Linha, posicao.Coluna] == '@';
    }

    public void ExibirMapaComRobo()
    {
        Console.WriteLine($"\n🤖 POSIÇÃO ATUAL: {_posicaoRobo} | DIREÇÃO: {_direcaoRobo} | CARGA: {(_humanoColetado ? "COM_HUMANO" : "SEM_CARGA")}");
        Console.WriteLine("═══════════════════");

        for (int i = 0; i < _mapa.QuantidadeDeLinhas; i++)
        {
            Console.Write($"{i:D2} │ ");

            for (int j = 0; j < _mapa.QuantidadeDeColunas; j++)
            {
                var posicaoAtual = new Posicao(i, j);

                // Se é a posição do robô, mostrar o robô com direção
                if (posicaoAtual.Equals(_posicaoRobo))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    char simboloRobo = _direcaoRobo switch
                    {
                        EDirecao.Norte => '↑',
                        EDirecao.Leste => '→',
                        EDirecao.Sul => '↓',
                        EDirecao.Oeste => '←',
                        _ => '?'
                    };
                    Console.Write(simboloRobo);
                    Console.ResetColor();
                }
                else
                {
                    // Mostrar o caractere normal do mapa
                    char caractere = _mapa.Labirinto[i, j];

                    // Se o humano foi coletado, não mostrar @ no mapa
                    if (caractere == '@' && _humanoColetado)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write('·');
                        Console.ResetColor();
                    }
                    else
                    {
                        ExibirCaractereColorido(caractere);
                    }
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine("═══════════════════");
    }

    private static void ExibirCaractereColorido(char c)
    {
        switch (c)
        {
            case 'X': // Parede
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write('█');
                break;
            case '.': // Espaço livre
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write('·');
                break;
            case 'E': // Entrada
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('E');
                break;
            case '@': // Humano
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write('@');
                break;
            case ' ': // Espaço vazio
                Console.Write(' ');
                break;
            default: // Caractere desconhecido
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write('?');
                break;
        }
        Console.ResetColor();
    }

    public void SalvarLogs()
    {
        _log.SalvarArquivos();
    }
}
