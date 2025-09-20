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

            case EComandoRobo.A:
                return Avancar();

            case EComandoRobo.G:
                return Girar();

            case EComandoRobo.P:
                return PegarHumano();

            case EComandoRobo.E:
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
        return LerSensores(EComandoRobo.A);
    }

    private RegistroLogMelhorado Girar()
    {
        _direcaoRobo = _direcaoRobo.GirarDireita();
        return LerSensores(EComandoRobo.G);
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
        return LerSensores(EComandoRobo.P);
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
        return LerSensores(EComandoRobo.E);
    }

    private RegistroLogMelhorado LerSensores(EComandoRobo comando)
    {
        if (_humanoColetado && comando != EComandoRobo.P)
        {
            var sensorEsq = LerSensorEsquerdo();
            var sensorDir = LerSensorDireito();
            var sensorFrente = LerSensorFrente();

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


}
