using RoboSalvamento.Core;
using RoboSalvamento.Robo;
using RoboSalvamento.Simulador;

namespace RoboSalvamento.Tests.Robo;

[Trait("Categoria", "Testes de Algoritmo BFS")]
public sealed class AlgoritmoBFSTests
{
    #region Setup e Helpers

    private SimuladorAmbienteVirtual CriarSimuladorValido()
    {
        string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "3x5.txt");
        var mapa = new Mapa(caminhoArquivo);
        return new SimuladorAmbienteVirtual(mapa);
    }

    #endregion

    #region Testes do Construtor

    [Fact]
    public void Construtor_DadoSimuladorValido_DeveCriarAlgoritmoCorretamente()
    {
        // arrange
        var simulador = CriarSimuladorValido();
        var log = new LogOperacao("3x5.txt");

        // action
        var algoritmo = new AlgoritmoBFS(simulador, log);

        // assert
        Assert.NotNull(algoritmo);
    }

    [Fact]
    public void Construtor_DadoLogNulo_DeveLancarArgumentNullException()
    {
        // arrange
        var simulador = CriarSimuladorValido();

        // action & assert
        Assert.Throws<ArgumentNullException>(() => new AlgoritmoBFS(simulador, null!));
    }

    #endregion

    #region Testes de Execução da Missão

    [Fact]
    public void ExecutarMissao_DadoSimuladorValido_DeveExecutarMissaoComSucesso()
    {
        // arrange
        var simulador = CriarSimuladorValido();
        var log = new LogOperacao("3x5.txt");
        var algoritmo = new AlgoritmoBFS(simulador, log);

        // action
        algoritmo.ExecutarMissao();

        // assert
        // Se chegou até aqui sem exceção, a missão foi executada
        Assert.True(true);
    }

    [Fact]
    public void ExecutarMissao_DadoSimuladorValido_DeveGerarLogCorretamente()
    {
        // arrange
        var simulador = CriarSimuladorValido();
        var log = new LogOperacao("3x5.txt");
        var algoritmo = new AlgoritmoBFS(simulador, log);

        // action
        algoritmo.ExecutarMissao();
        log.SalvarArquivos();

        // assert
        Assert.True(File.Exists("3x5.csv"));
        var linhas = File.ReadAllLines("3x5.csv");
        Assert.NotEmpty(linhas);
        
        // Verificar se a primeira linha é LIGAR
        Assert.StartsWith("LIGAR,", linhas[0]);
        
        // Limpar arquivo de teste
        File.Delete("3x5.csv");
    }

    [Fact]
    public void ExecutarMissao_DadoSimuladorValido_DeveExecutarMultiplasVezes()
    {
        // arrange
        var simulador = CriarSimuladorValido();
        var log = new LogOperacao("3x5.txt");
        var algoritmo = new AlgoritmoBFS(simulador, log);

        // action & assert - deve executar sem exceção
        algoritmo.ExecutarMissao();
        algoritmo.ExecutarMissao();
        algoritmo.ExecutarMissao();
        
        Assert.True(true);
    }

    [Fact]
    public void ExecutarMissao_DadoSimuladorValido_DeveExecutarEmTempoRazoavel()
    {
        // arrange
        var simulador = CriarSimuladorValido();
        var log = new LogOperacao("3x5.txt");
        var algoritmo = new AlgoritmoBFS(simulador, log);
        var tempoInicio = DateTime.Now;

        // action
        algoritmo.ExecutarMissao();
        var tempoFim = DateTime.Now;
        var duracao = tempoFim - tempoInicio;

        // assert
        Assert.True(duracao.TotalSeconds < 10, "Algoritmo deve executar em menos de 10 segundos");
    }

    #endregion
}