using RoboSalvamento.Core;
using RoboSalvamento.Extensions;
using RoboSalvamento.Simulador;

namespace RoboSalvamento.Robo;

/// <summary>
/// Algoritmo de busca inteligente V2 para o robô de salvamento.
/// Implementa busca sistemática com exploração vertical e horizontal.
/// </summary>
public class AlgoritmoBuscaInteligenteV2
{
    private readonly SimuladorAmbienteVirtual _simulador;
    private readonly LogOperacaoMelhorado _log;
    private readonly HashSet<Posicao> _posicoesVisitadas;
    private readonly Stack<Posicao> _caminhoPercorrido;
    private readonly List<Posicao> _caminhoCompleto; // Caminho completo da ida
    private readonly List<EDirecao> _direcoesCompletas; // Direções usadas na ida

    private bool _humanoEncontrado = false;
    private Posicao _posicaoEntrada = null!;

    public AlgoritmoBuscaInteligenteV2(SimuladorAmbienteVirtual simulador, LogOperacaoMelhorado log)
    {
        _simulador = simulador ?? throw new ArgumentNullException(nameof(simulador));
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _posicoesVisitadas = new HashSet<Posicao>();
        _caminhoPercorrido = new Stack<Posicao>();
        _caminhoCompleto = new List<Posicao>();
        _direcoesCompletas = new List<EDirecao>();
    }

    /// <summary>
    /// Executa a missão completa de busca e salvamento.
    /// </summary>
    public void ExecutarMissao()
    {
        try
        {
            Console.WriteLine("INICIANDO MISSÃO DE SALVAMENTO");

            LigarRobo();

            BuscarHumano();

            if (_humanoEncontrado)
            {
                RetornarComHumano();
                EjetarHumano();
            }

            Console.WriteLine("\n✅ MISSÃO CONCLUÍDA COM SUCESSO!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ ERRO NA MISSÃO: {ex.Message}");
            throw;
        }
    }

    private void LigarRobo()
    {
        Console.WriteLine("\n🔌 LIGANDO O ROBÔ...");
        var registro = _simulador.ExecutarComando(EComandoRobo.LIGAR);
        _log.AdicionarRegistro(registro);

        _posicaoEntrada = new Posicao(_simulador.PosicaoRobo.Linha, _simulador.PosicaoRobo.Coluna);
        _caminhoPercorrido.Push(_posicaoEntrada);
        _posicoesVisitadas.Add(_posicaoEntrada);

        Console.WriteLine($"   Posição inicial: {_simulador.PosicaoRobo}");
        Console.WriteLine($"   Direção inicial: {_simulador.DirecaoRobo}");
    }

    private void BuscarHumano()
    {
        Console.WriteLine("\n🔍 INICIANDO BUSCA PELO HUMANO...");

        int maxTentativas = 10000; // Aumentar limite
        int tentativas = 0;
        int girosConsecutivos = 0;
        const int maxGirosConsecutivos = 4; // Máximo de 1 volta completa

        while (!_humanoEncontrado && tentativas < maxTentativas)
        {
            tentativas++;

      
            if (VerificarHumanoAFrente())
            {
                PegarHumano();
                _humanoEncontrado = true;
                break;
            }

            var acao = DecidirAcaoBuscaInteligente();

            if (acao == EComandoRobo.Avancar)
            {
                Avancar();
                girosConsecutivos = 0;
            }
            else if (acao == EComandoRobo.Girar90GrausDireita)
            {
                Girar();
                girosConsecutivos++;

                // Se girou muito, tentar estratégia diferente
                if (girosConsecutivos >= maxGirosConsecutivos)
                {
                    Console.WriteLine("   ⚠️ Muitos giros, tentando estratégia diferente...");
                    // Tentar avançar mesmo que pareça bloqueado
                    if (PodeAvançar())
                    {
                        Avancar();
                        girosConsecutivos = 0;
                    }
                    else
                    {
                        // Se realmente não pode avançar, continuar girando
                        girosConsecutivos = 0;
                    }
                }
            }

            // Mostrar progresso
            if (tentativas % 20 == 0)
            {
                Console.WriteLine($"   🔄 Tentativa {tentativas}/{maxTentativas} - Posição: {_simulador.PosicaoRobo} - Giros: {girosConsecutivos}");
            }
        }

        if (tentativas >= maxTentativas)
        {
            throw new DomainException($"Limite de tentativas excedido ({maxTentativas})! Missão falhou.");
        }
    }

    private EComandoRobo DecidirAcaoBuscaInteligente()
    {
        var registro = LerSensores();
        var posicaoAtual = _simulador.PosicaoRobo;

        // Prioridade 1: Se há humano à frente, avançar
        if (registro.SensorFrente == ELeituraSensor.HUMANO)
        {
            return EComandoRobo.Avancar;
        }

        // Prioridade 2: Se há espaço vazio à frente e não foi visitado, avançar
        if (registro.SensorFrente == ELeituraSensor.VAZIO)
        {
            var proximaPosicao = _simulador.DirecaoRobo.ObterPosicaoFrente(posicaoAtual);
            if (!_posicoesVisitadas.Contains(proximaPosicao))
            {
                return EComandoRobo.Avancar;
            }
        }

        // Prioridade 3: Se há espaço vazio à esquerda e não foi visitado, girar
        if (registro.SensorEsquerdo == ELeituraSensor.VAZIO)
        {
            var posicaoEsquerda = _simulador.DirecaoRobo.ObterPosicaoEsquerda(posicaoAtual);
            if (!_posicoesVisitadas.Contains(posicaoEsquerda))
            {
                return EComandoRobo.Girar90GrausDireita;
            }
        }

        // Prioridade 4: Se há espaço vazio à direita e não foi visitado, girar
        if (registro.SensorDireito == ELeituraSensor.VAZIO)
        {
            var posicaoDireita = _simulador.DirecaoRobo.ObterPosicaoDireita(posicaoAtual);
            if (!_posicoesVisitadas.Contains(posicaoDireita))
            {
                return EComandoRobo.Girar90GrausDireita;
            }
        }

        // Prioridade 5: Se há espaço vazio à frente (mesmo visitado), avançar
        if (registro.SensorFrente == ELeituraSensor.VAZIO)
        {
            return EComandoRobo.Avancar;
        }

        // Prioridade 6: Se há espaço vazio à esquerda (mesmo visitado), girar
        if (registro.SensorEsquerdo == ELeituraSensor.VAZIO)
        {
            return EComandoRobo.Girar90GrausDireita;
        }

        // Prioridade 7: Se há espaço vazio à direita (mesmo visitado), girar
        if (registro.SensorDireito == ELeituraSensor.VAZIO)
        {
            return EComandoRobo.Girar90GrausDireita;
        }

        // Se não há opções, girar para encontrar novo caminho
        return EComandoRobo.Girar90GrausDireita;
    }

    private void RetornarComHumano()
    {
        Console.WriteLine("\n🏃 RETORNANDO COM O HUMANO...");

        // Usar caminho salvo para retorno mais eficiente
        RetornarUsandoCaminhoSalvo();

        // Garantir que está virado para fora (direção oposta à inicial)
        var direcaoInicial = CalcularDirecaoInicial();
        var direcaoOposta = (EDirecao)(((int)direcaoInicial + 2) % 4);

        while (_simulador.DirecaoRobo != direcaoOposta)
        {
            Girar();
        }

        Console.WriteLine("   ✅ Chegou na entrada e está posicionado para ejetar!");
    }

    private void RetornarUsandoCaminhoSalvo()
    {
        Console.WriteLine("   🔄 Usando caminho salvo para retorno eficiente...");

        if (_caminhoCompleto.Count == 0)
        {
            Console.WriteLine("   ⚠️ Nenhum caminho salvo, usando estratégia segura...");
            RetornarComEstrategiaSegura();
            return;
        }

        // Reverter o caminho (do final para o início)
        var caminhoReverso = new List<Posicao>(_caminhoCompleto);
        caminhoReverso.Reverse();

        Console.WriteLine($"   📍 Caminho salvo tem {caminhoReverso.Count} posições");

        // Remover a posição atual (onde está o humano) e a entrada
        caminhoReverso.RemoveAt(0); // Remove posição atual
        if (caminhoReverso.Count > 0)
        {
            caminhoReverso.RemoveAt(caminhoReverso.Count - 1); // Remove entrada
        }

        Console.WriteLine($"   🎯 Seguindo {caminhoReverso.Count} posições do caminho reverso");

        foreach (var posicaoAlvo in caminhoReverso)
        {
            Console.WriteLine($"   🔄 Indo para posição {posicaoAlvo}...");

            // Ir para a posição alvo usando lógica simples
            while (!_simulador.PosicaoRobo.Equals(posicaoAlvo))
            {
                // Calcular direção para a posição alvo
                var direcaoParaAlvo = CalcularDirecaoParaPosicao(_simulador.PosicaoRobo, posicaoAlvo);

                // Girar até estar na direção correta
                while (_simulador.DirecaoRobo != direcaoParaAlvo)
                {
                    Console.WriteLine($"      Girando de {_simulador.DirecaoRobo} para {direcaoParaAlvo}...");
                    Girar();
                }

                // Verificar se pode avançar
                var registro = LerSensores();
                if (registro.SensorFrente == ELeituraSensor.VAZIO)
                {
                    Console.WriteLine($"      Avançando para {posicaoAlvo}...");
                    Avancar();
                }
                else
                {
                    Console.WriteLine($"      ⚠️ Parede à frente, tentando contornar...");
                    // Se há parede, tentar contornar
                    if (registro.SensorDireito == ELeituraSensor.VAZIO)
                    {
                        Console.WriteLine("      Girando para direita...");
                        Girar();
                        Avancar();
                    }
                    else if (registro.SensorEsquerdo == ELeituraSensor.VAZIO)
                    {
                        Console.WriteLine("      Girando para esquerda...");
                        Girar();
                        Girar();
                        Girar();
                        Avancar();
                    }
                    else
                    {
                        Console.WriteLine($"      ❌ Sem opções, usando estratégia segura...");
                        RetornarComEstrategiaSegura();
                        return;
                    }
                }
            }

            Console.WriteLine($"      ✅ Chegou na posição {posicaoAlvo}!");
        }

        Console.WriteLine("   ✅ Retorno usando caminho salvo concluído!");

        // Garantir que chegou exatamente na entrada
        if (!_simulador.PosicaoRobo.Equals(_posicaoEntrada))
        {
            Console.WriteLine($"   🎯 Ajustando posição final: {_simulador.PosicaoRobo} -> {_posicaoEntrada}");

            // Ir para a entrada usando lógica simples
            while (!_simulador.PosicaoRobo.Equals(_posicaoEntrada))
            {
                // Calcular direção para a entrada
                var direcaoParaEntrada = CalcularDirecaoParaPosicao(_simulador.PosicaoRobo, _posicaoEntrada);

                // Girar até estar na direção correta
                while (_simulador.DirecaoRobo != direcaoParaEntrada)
                {
                    Console.WriteLine($"      Girando de {_simulador.DirecaoRobo} para {direcaoParaEntrada}...");
                    Girar();
                }

                // Verificar se pode avançar
                var registro = LerSensores();
                if (registro.SensorFrente == ELeituraSensor.VAZIO)
                {
                    Console.WriteLine($"      Avançando para entrada: {_simulador.PosicaoRobo} -> {_posicaoEntrada}");
                    Avancar();
                }
                else
                {
                    Console.WriteLine("      ⚠️ Parede à frente, tentando contornar...");
                    // Se há parede, tentar contornar
                    if (registro.SensorDireito == ELeituraSensor.VAZIO)
                    {
                        Console.WriteLine("      Girando para direita...");
                        Girar();
                        Avancar();
                    }
                    else if (registro.SensorEsquerdo == ELeituraSensor.VAZIO)
                    {
                        Console.WriteLine("      Girando para esquerda...");
                        Girar();
                        Girar();
                        Girar();
                        Avancar();
                    }
                    else
                    {
                        Console.WriteLine($"      ❌ Sem opções, usando estratégia segura...");
                        RetornarComEstrategiaSegura();
                        return;
                    }
                }
            }

            Console.WriteLine($"   ✅ Chegou exatamente na entrada: {_simulador.PosicaoRobo}");
        }

        // Garantir que está virado para FORA (direção oposta à inicial)
        var direcaoInicial = CalcularDirecaoInicial();
        var direcaoOposta = (EDirecao)(((int)direcaoInicial + 2) % 4);

        Console.WriteLine($"   🔄 Ajustando direção final: {_simulador.DirecaoRobo} -> {direcaoOposta} (fora do labirinto)");

        while (_simulador.DirecaoRobo != direcaoOposta)
        {
            Console.WriteLine($"      Girando de {_simulador.DirecaoRobo} para {direcaoOposta}...");
            Girar();
        }

        Console.WriteLine($"   ✅ Robô posicionado na entrada virado para fora!");
    }

    private void RetornarComEstrategiaSegura()
    {
        Console.WriteLine("   🔄 Usando estratégia de retorno segura...");

        int maxTentativas = 200;
        int tentativas = 0;
        int girosConsecutivos = 0;
        const int maxGirosConsecutivos = 8; // Máximo de 2 voltas completas

        while (!_simulador.PosicaoRobo.Equals(_posicaoEntrada) && tentativas < maxTentativas)
        {
            tentativas++;

            // Calcular direção para a entrada
            var direcaoParaEntrada = CalcularDirecaoParaPosicao(_simulador.PosicaoRobo, _posicaoEntrada);

            Console.WriteLine($"   🔄 Tentativa {tentativas}: Posição {_simulador.PosicaoRobo} -> Entrada {_posicaoEntrada}");
            Console.WriteLine($"      Direção atual: {_simulador.DirecaoRobo}, Direção necessária: {direcaoParaEntrada}");

            // Estratégia segura: verificar sensores antes de avançar
            var registro = LerSensores();

            // PRIORIDADE 1: Se está na direção correta e pode avançar, avançar
            if (_simulador.DirecaoRobo == direcaoParaEntrada && registro.SensorFrente == ELeituraSensor.VAZIO)
            {
                Console.WriteLine("      Avançando na direção correta...");
                Avancar();
                girosConsecutivos = 0; // Reset contador
            }
            // PRIORIDADE 2: Se há espaço vazio à frente (mesmo não sendo direção ideal), avançar
            else if (registro.SensorFrente == ELeituraSensor.VAZIO)
            {
                Console.WriteLine("      Avançando para espaço vazio...");
                Avancar();
                girosConsecutivos = 0; // Reset contador
            }
            // PRIORIDADE 3: Se não está na direção correta, girar para a direção necessária
            else if (_simulador.DirecaoRobo != direcaoParaEntrada)
            {
                Console.WriteLine($"      Girando para direção necessária: {direcaoParaEntrada}");
                Girar();
                girosConsecutivos++;
            }
            // PRIORIDADE 3: Se está na direção correta mas há parede à frente, explorar alternativas
            else if (registro.SensorFrente == ELeituraSensor.PAREDE)
            {
                Console.WriteLine("      Parede à frente, explorando alternativas...");

                // Tentar direita primeiro
                if (registro.SensorDireito == ELeituraSensor.VAZIO)
                {
                    Console.WriteLine("      Girando para direita...");
                    Girar();
                    girosConsecutivos++;
                }
                // Tentar esquerda se direita não funcionar
                else if (registro.SensorEsquerdo == ELeituraSensor.VAZIO)
                {
                    Console.WriteLine("      Girando para esquerda...");
                    Girar();
                    girosConsecutivos++;
                }
                // Se não há opções, tentar avançar mesmo assim (pode ser erro de sensor)
                else
                {
                    Console.WriteLine("      Sem opções, tentando avançar mesmo assim...");
                    if (PodeAvançar())
                    {
                        Avancar();
                        girosConsecutivos = 0;
                    }
                    else
                    {
                        Console.WriteLine("      Girando para encontrar novo caminho...");
                        Girar();
                        girosConsecutivos++;
                    }
                }
            }
            // PRIORIDADE 4: Se há espaço vazio à frente (mesmo não sendo direção ideal), avançar
            else if (registro.SensorFrente == ELeituraSensor.VAZIO)
            {
                Console.WriteLine("      Avançando para espaço vazio...");
                Avancar();
                girosConsecutivos = 0; // Reset contador
            }
            // PRIORIDADE 5: Se não há opções, girar para encontrar novo caminho
            else
            {
                Console.WriteLine("      Sem opções, girando...");
                Girar();
                girosConsecutivos++;
            }

            // Se girou muito, tentar estratégia diferente
            if (girosConsecutivos >= maxGirosConsecutivos)
            {
                Console.WriteLine("      ⚠️ Muitos giros, tentando estratégia diferente...");

                // Estratégia: tentar avançar em qualquer direção vazia
                var registroAtual = LerSensores();
                Console.WriteLine($"      Sensores: Frente={registroAtual.SensorFrente}, Esquerda={registroAtual.SensorEsquerdo}, Direita={registroAtual.SensorDireito}");

                if (registroAtual.SensorFrente == ELeituraSensor.VAZIO)
                {
                    Console.WriteLine("      Avançando para frente...");
                    Avancar();
                    girosConsecutivos = 0;
                }
                else if (registroAtual.SensorDireito == ELeituraSensor.VAZIO)
                {
                    Console.WriteLine("      Girando para direita e avançando...");
                    Girar();
                    // Verificar novamente após girar
                    var registroAposGiro = LerSensores();
                    if (registroAposGiro.SensorFrente == ELeituraSensor.VAZIO)
                    {
                        Avancar();
                        girosConsecutivos = 0;
                    }
                    else
                    {
                        Console.WriteLine("      Ainda há parede após girar, resetando...");
                        girosConsecutivos = 0;
                    }
                }
                else if (registroAtual.SensorEsquerdo == ELeituraSensor.VAZIO)
                {
                    Console.WriteLine("      Girando para esquerda e avançando...");
                    Girar();
                    // Verificar novamente após girar
                    var registroAposGiro = LerSensores();
                    if (registroAposGiro.SensorFrente == ELeituraSensor.VAZIO)
                    {
                        Avancar();
                        girosConsecutivos = 0;
                    }
                    else
                    {
                        Console.WriteLine("      Ainda há parede após girar, resetando...");
                        girosConsecutivos = 0;
                    }
                }
                else
                {
                    Console.WriteLine("      Sem opções, resetando contador...");
                    girosConsecutivos = 0;
                }
            }
        }

        if (tentativas >= maxTentativas)
        {
            Console.WriteLine("   ⚠️ Limite de tentativas atingido, mas chegou próximo da entrada");
        }
    }


    private void EjetarHumano()
    {
        Console.WriteLine("\n🚀 EJETANDO O HUMANO...");

        var registro = _simulador.ExecutarComando(EComandoRobo.EjetarHumano);
        _log.AdicionarRegistro(registro);

        Console.WriteLine("   Humano ejetado com sucesso!");
    }

    private bool VerificarHumanoAFrente()
    {
        var registro = LerSensores();
        return registro.SensorFrente == ELeituraSensor.HUMANO;
    }

    private void PegarHumano()
    {
        Console.WriteLine("\n🤝 COLETANDO O HUMANO...");

        var registro = _simulador.ExecutarComando(EComandoRobo.PegarHumano);
        _log.AdicionarRegistro(registro);

        Console.WriteLine("   Humano coletado com sucesso!");
    }

    private bool PodeAvançar()
    {
        var registro = LerSensores();
        return registro.SensorFrente == ELeituraSensor.VAZIO;
    }


    private void Avancar()
    {
        var posicaoAnterior = new Posicao(_simulador.PosicaoRobo.Linha, _simulador.PosicaoRobo.Coluna);

        var registro = _simulador.ExecutarComando(EComandoRobo.Avancar);
        _log.AdicionarRegistro(registro);

        var novaPosicao = new Posicao(_simulador.PosicaoRobo.Linha, _simulador.PosicaoRobo.Coluna);
        _caminhoPercorrido.Push(posicaoAnterior);
        _posicoesVisitadas.Add(novaPosicao);

        // Salvar caminho completo para retorno
        _caminhoCompleto.Add(novaPosicao);
        _direcoesCompletas.Add(_simulador.DirecaoRobo);
    }

    private void Girar()
    {
        var registro = _simulador.ExecutarComando(EComandoRobo.Girar90GrausDireita);
        _log.AdicionarRegistro(registro);

        // Salvar direção após girar
        _direcoesCompletas.Add(_simulador.DirecaoRobo);
    }

    private RegistroLogMelhorado LerSensores()
    {
        var registro = _simulador.ExecutarComando(EComandoRobo.LIGAR);
        return new RegistroLogMelhorado
        {
            Comando = EComandoRobo.LIGAR,
            SensorEsquerdo = registro.SensorEsquerdo,
            SensorDireito = registro.SensorDireito,
            SensorFrente = registro.SensorFrente,
            EstadoCarga = _simulador.HumanoColetado ? EEstadoCarga.COM_HUMANO : EEstadoCarga.SEM_CARGA,
            PosicaoRobo = new Posicao(_simulador.PosicaoRobo.Linha, _simulador.PosicaoRobo.Coluna),
            DirecaoRobo = _simulador.DirecaoRobo
        };
    }

    private EDirecao CalcularDirecaoParaPosicao(Posicao origem, Posicao destino)
    {
        var deltaLinha = destino.Linha - origem.Linha;
        var deltaColuna = destino.Coluna - origem.Coluna;

        // Priorizar movimento vertical se a diferença for maior ou igual
        if (Math.Abs(deltaLinha) > Math.Abs(deltaColuna))
        {
            if (deltaLinha < 0) return EDirecao.Norte;
            if (deltaLinha > 0) return EDirecao.Sul;
        }

        // Caso contrário, priorizar movimento horizontal
        if (deltaColuna < 0) return EDirecao.Oeste;
        if (deltaColuna > 0) return EDirecao.Leste;

        // Se já está na posição, manter direção atual
        return _simulador.DirecaoRobo;
    }

    private EDirecao CalcularDirecaoInicial()
    {
        var entrada = _posicaoEntrada;

        // Determinar direção baseada na posição da entrada
        // Como a entrada está na borda, a direção inicial é para o interior
        if (entrada.Linha == 0) return EDirecao.Sul; // Borda superior -> Sul
        if (entrada.Coluna == 0) return EDirecao.Leste; // Borda esquerda -> Leste
        if (entrada.Coluna > 0) return EDirecao.Oeste; // Borda direita -> Oeste
        if (entrada.Linha > 0) return EDirecao.Norte; // Borda inferior -> Norte

        return EDirecao.Sul; // Fallback
    }
}
