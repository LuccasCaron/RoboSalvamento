using RoboSalvamento.Core;
using RoboSalvamento.Extensions;
using RoboSalvamento.Simulador;

namespace RoboSalvamento.Robo;

public class AlgoritmoBFS
{
    private readonly SimuladorAmbienteVirtual _simulador;
    private readonly LogOperacaoMelhorado _log;
    private readonly HashSet<Posicao> _posicoesVisitadas;

    private Posicao _posicaoEntrada = null!;
    private Posicao _posicaoHumano = null!;

    public AlgoritmoBFS(SimuladorAmbienteVirtual simulador, LogOperacaoMelhorado log)
    {
        _simulador = simulador ?? throw new ArgumentNullException(nameof(simulador));
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _posicoesVisitadas = new HashSet<Posicao>();
    }

    /// <summary>
    /// Executa a missão completa de busca e salvamento usando BFS.
    /// </summary>
    public void ExecutarMissao()
    {
        try
        {

            LigarRobo();

            // Encontrar o humano usando BFS
            var caminhoParaHumano = BuscarHumanoComBFS();
            
            if (caminhoParaHumano != null && caminhoParaHumano.Count > 0)
            {
                
                // Executar o caminho para chegar próximo ao humano (sem atropelar)
                var caminhoSeguro = new List<Posicao>(caminhoParaHumano);
                caminhoSeguro.RemoveAt(caminhoSeguro.Count - 1); // Remove a posição do humano
                ExecutarCaminhoRapido(caminhoSeguro);
                
                // Girar para ficar de frente para o humano
                var direcaoParaHumano = CalcularDirecaoParaPosicao(_simulador.PosicaoRobo, _posicaoHumano);
                while (_simulador.DirecaoRobo != direcaoParaHumano)
                {
                    Girar();
                }
                
                // Pegar o humano
                PegarHumano();
                
                // Retornar usando o caminho reverso
                var caminhoRetorno = new List<Posicao>(caminhoParaHumano);
                caminhoRetorno.Reverse();
                caminhoRetorno.RemoveAt(0); // Remove posição atual (onde está o humano)
                
                ExecutarCaminhoRapido(caminhoRetorno);
                
                // Ejetar o humano
                EjetarHumano();
            }
            else
            {
                throw new DomainException("Humano não encontrado! Missão falhou.");
            }

        }
        catch (Exception)
        {
            throw;
        }
    }

    private void LigarRobo()
    {
        var registro = _simulador.ExecutarComando(EComandoRobo.LIGAR);
        _log.AdicionarRegistro(registro);

        _posicaoEntrada = new Posicao(_simulador.PosicaoRobo.Linha, _simulador.PosicaoRobo.Coluna);
        _posicoesVisitadas.Add(_posicaoEntrada);
    }

    private List<Posicao> BuscarHumanoComBFS()
    {
        var fila = new Queue<Posicao>();
        var visitados = new HashSet<Posicao>();
        var pai = new Dictionary<Posicao, Posicao>();
        var encontrado = false;

        fila.Enqueue(_posicaoEntrada);
        visitados.Add(_posicaoEntrada);

        while (fila.Count > 0 && !encontrado)
        {
            var posicaoAtual = fila.Dequeue();

            if (PosicaoEhHumano(posicaoAtual))
            {
                _posicaoHumano = posicaoAtual;
                encontrado = true;
                break;
            }

            var vizinhos = ObterVizinhosValidos(posicaoAtual);
            
            foreach (var vizinho in vizinhos)
            {
                if (!visitados.Contains(vizinho))
                {
                    visitados.Add(vizinho);
                    pai[vizinho] = posicaoAtual;
                    fila.Enqueue(vizinho);
                }
            }
        }

        if (!encontrado)
        {
            return new List<Posicao>();
        }

        var caminho = new List<Posicao>();
        var posicaoAtualCaminho = _posicaoHumano;
        
        while (posicaoAtualCaminho != null)
        {
            caminho.Add(posicaoAtualCaminho);
            pai.TryGetValue(posicaoAtualCaminho, out posicaoAtualCaminho);
        }

        caminho.Reverse();
        
        return caminho;
    }

    private bool PosicaoEhHumano(Posicao posicao)
    {
        var campoMapa = typeof(SimuladorAmbienteVirtual).GetField("_mapa", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var mapa = (Mapa)campoMapa!.GetValue(_simulador)!;
        return mapa.Labirinto[posicao.Linha, posicao.Coluna] == '@';
    }

    private List<Posicao> ObterVizinhosValidos(Posicao posicao)
    {
        var vizinhos = new List<Posicao>();
        var direcoes = new[] { EDirecao.Norte, EDirecao.Sul, EDirecao.Leste, EDirecao.Oeste };

        var campoMapa = typeof(SimuladorAmbienteVirtual).GetField("_mapa", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var mapa = (Mapa)campoMapa!.GetValue(_simulador)!;

        foreach (var direcao in direcoes)
        {
            var vizinho = direcao.ObterPosicaoFrente(posicao);
            
            if (vizinho.Linha >= 0 && vizinho.Linha < mapa.QuantidadeDeLinhas &&
                vizinho.Coluna >= 0 && vizinho.Coluna < mapa.QuantidadeDeColunas)
            {
                var caractere = mapa.Labirinto[vizinho.Linha, vizinho.Coluna];
                if (caractere != 'X')
                {
                    vizinhos.Add(vizinho);
                }
            }
        }

        return vizinhos;
    }


    private void ExecutarCaminhoRapido(List<Posicao> caminho)
    {
        for (int i = 1; i < caminho.Count; i++)
        {
            var posicaoAtual = _simulador.PosicaoRobo;
            var proximaPosicao = caminho[i];
            
            var direcaoNecessaria = CalcularDirecaoParaPosicao(posicaoAtual, proximaPosicao);
            
            while (_simulador.DirecaoRobo != direcaoNecessaria)
            {
                Girar();
            }
            
            Avancar();
        }
    }

    private void PegarHumano()
    {
        var registro = _simulador.ExecutarComando(EComandoRobo.P);
        _log.AdicionarRegistro(registro);
    }

    private void EjetarHumano()
    {
        var registro = _simulador.ExecutarComando(EComandoRobo.E);
        _log.AdicionarRegistro(registro);
    }

    private void Avancar()
    {
        var registro = _simulador.ExecutarComando(EComandoRobo.A);
        _log.AdicionarRegistro(registro);

        var novaPosicao = new Posicao(_simulador.PosicaoRobo.Linha, _simulador.PosicaoRobo.Coluna);
        _posicoesVisitadas.Add(novaPosicao);
    }

    private void Girar()
    {
        var registro = _simulador.ExecutarComando(EComandoRobo.G);
        _log.AdicionarRegistro(registro);
    }

    private EDirecao CalcularDirecaoParaPosicao(Posicao origem, Posicao destino)
    {
        var deltaLinha = destino.Linha - origem.Linha;
        var deltaColuna = destino.Coluna - origem.Coluna;

        if (Math.Abs(deltaLinha) > Math.Abs(deltaColuna))
        {
            if (deltaLinha < 0) return EDirecao.Norte;
            if (deltaLinha > 0) return EDirecao.Sul;
        }

        if (deltaColuna < 0) return EDirecao.Oeste;
        if (deltaColuna > 0) return EDirecao.Leste;

        return _simulador.DirecaoRobo;
    }
}
