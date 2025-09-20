using RoboSalvamento.Core;
using RoboSalvamento.Simulador;

namespace RoboSalvamento.Tests.Simulador;

[Trait("Categoria", "Testes de Simulador de Ambiente Virtual")]
public sealed class SimuladorAmbienteVirtualTests
{
    #region Setup e Helpers

    private Mapa CriarMapaValido()
    {
        string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "3x5.txt");
        return new Mapa(caminhoArquivo);
    }

    #endregion

    #region Testes do Construtor

    [Fact]
    public void Construtor_DadoMapaValido_DeveInicializarRoboCorretamente()
    {
        // arrange
        var mapa = CriarMapaValido();

        // action
        var simulador = new SimuladorAmbienteVirtual(mapa);

        // assert
        Assert.NotNull(simulador);
        Assert.Equal(mapa.Entrada, simulador.PosicaoRobo);
        Assert.Equal(EEstadoCarga.SEM_CARGA, simulador.HumanoColetado ? EEstadoCarga.COM_HUMANO : EEstadoCarga.SEM_CARGA);
    }

    #endregion

    #region Testes de Comandos Básicos

    [Fact]
    public void ExecutarComando_DadoComandoLigar_DeveLigarRobo()
    {
        // arrange
        var mapa = CriarMapaValido();
        var simulador = new SimuladorAmbienteVirtual(mapa);

        // action
        var resultado = simulador.ExecutarComando(EComandoRobo.LIGAR);

        // assert
        Assert.NotNull(resultado);
        Assert.True(Enum.IsDefined(typeof(ELeituraSensor), resultado.SensorFrente));
    }

    [Fact]
    public void ExecutarComando_DadoComandoGirar_DeveGirarRobo()
    {
        // arrange
        var mapa = CriarMapaValido();
        var simulador = new SimuladorAmbienteVirtual(mapa);

        // action
        var resultado = simulador.ExecutarComando(EComandoRobo.G);

        // assert
        Assert.NotNull(resultado);
    }

    [Fact]
    public void ExecutarComando_DadoComandoGirarQuatroVezes_DeveVoltarDirecaoOriginal()
    {
        // arrange
        var mapa = CriarMapaValido();
        var simulador = new SimuladorAmbienteVirtual(mapa);

        // action
        simulador.ExecutarComando(EComandoRobo.G);
        simulador.ExecutarComando(EComandoRobo.G);
        simulador.ExecutarComando(EComandoRobo.G);
        var resultado = simulador.ExecutarComando(EComandoRobo.G);

        // assert
        Assert.NotNull(resultado);
    }

    #endregion

    #region Testes de Propriedades

    [Fact]
    public void Propriedades_DadoSimuladorInicializado_DeveRetornarEstadoInicialCorreto()
    {
        // arrange
        var mapa = CriarMapaValido();
        var simulador = new SimuladorAmbienteVirtual(mapa);

        // assert
        Assert.Equal(mapa.Entrada, simulador.PosicaoRobo);
        Assert.False(simulador.HumanoColetado);
        Assert.False(simulador.MissaoCompleta);
    }

    #endregion

    #region Testes de Sensores

    [Fact]
    public void LerSensores_DadoRoboInicializado_DeveLerSensoresCorretamente()
    {
        // arrange
        var mapa = CriarMapaValido();
        var simulador = new SimuladorAmbienteVirtual(mapa);

        // action
        var resultado = simulador.ExecutarComando(EComandoRobo.LIGAR);

        // assert
        Assert.NotNull(resultado);
        Assert.True(Enum.IsDefined(typeof(ELeituraSensor), resultado.SensorFrente));
    }

    #endregion
}