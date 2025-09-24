using RoboSalvamento.Core;
using RoboSalvamento.Simulador;

namespace RoboSalvamento.Tests.Simulador;

[Trait("Categoria", "Testes de Mapa")]
public sealed class MapaTests
{
    #region Testes do Construtor com Arquivo Válido

    [Fact]
    public void Construtor_DadoArquivoValido_DeveCarregarMapaCorretamente()
    {
        // arrange
        string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "3x5.txt");

        // action
        var mapa = new Mapa(caminhoArquivo);

        // assert
        Assert.NotNull(mapa);
        Assert.NotNull(mapa.Labirinto);
        Assert.Equal(7, mapa.QuantidadeDeLinhas);
        Assert.Equal(11, mapa.QuantidadeDeColunas);
        Assert.NotNull(mapa.Entrada);
        Assert.NotNull(mapa.Humano);
    }

    [Fact]
    public void Construtor_DadoArquivoValido_DeveDefinirEntradaCorretamente()
    {
        // arrange
        string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "3x5.txt");

        // action
        var mapa = new Mapa(caminhoArquivo);

        // assert
        Assert.Equal(0, mapa.Entrada.Linha);
        Assert.Equal(1, mapa.Entrada.Coluna);
        Assert.Equal('E', mapa.Labirinto[mapa.Entrada.Linha, mapa.Entrada.Coluna]);
    }

    [Fact]
    public void Construtor_DadoArquivoValido_DeveDefinirHumanoCorretamente()
    {
        // arrange
        string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "3x5.txt");

        // action
        var mapa = new Mapa(caminhoArquivo);

        // assert
        Assert.Equal(5, mapa.Humano.Linha);
        Assert.Equal(9, mapa.Humano.Coluna);
        Assert.Equal('@', mapa.Labirinto[mapa.Humano.Linha, mapa.Humano.Coluna]);
    }

    [Fact]
    public void Construtor_DadoArquivoValido_DeveDefinirLabirintoCorretamente()
    {
        // arrange
        string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "3x5.txt");

        // action
        var mapa = new Mapa(caminhoArquivo);

        // assert
        Assert.NotNull(mapa.Labirinto);
        Assert.Equal(7, mapa.Labirinto.GetLength(0));
        Assert.Equal(11, mapa.Labirinto.GetLength(1));
        Assert.Equal('E', mapa.Labirinto[0, 1]);
        Assert.Equal('@', mapa.Labirinto[5, 9]);
    }

    #endregion

    #region Testes de Propriedades

    [Fact]
    public void Propriedades_DadoMapaValido_DeveRetornarValoresCorretos()
    {
        // arrange
        string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "3x5.txt");
        var mapa = new Mapa(caminhoArquivo);

        // assert
        Assert.Equal(7, mapa.QuantidadeDeLinhas);
        Assert.Equal(11, mapa.QuantidadeDeColunas);
        Assert.NotNull(mapa.Entrada);
        Assert.NotNull(mapa.Humano);
        Assert.NotNull(mapa.Labirinto);
    }

    [Fact]
    public void Labirinto_DadoMapaValido_DeveTerDimensoesCorretas()
    {
        // arrange
        string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "3x5.txt");
        var mapa = new Mapa(caminhoArquivo);

        // assert
        Assert.Equal(7, mapa.Labirinto.GetLength(0));
        Assert.Equal(11, mapa.Labirinto.GetLength(1));
    }

    #endregion

    #region Testes de Validação de Entrada

    [Fact]
    public void Construtor_DadoMapaValido_EntradaEDeveEstarNaBorda()
    {
        // arrange
        string caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "3x5.txt");

        // action
        var mapa = new Mapa(caminhoArquivo);

        // assert
        Assert.True(mapa.Entrada.Linha == 0 || mapa.Entrada.Linha == mapa.QuantidadeDeLinhas - 1 ||
                   mapa.Entrada.Coluna == 0 || mapa.Entrada.Coluna == mapa.QuantidadeDeColunas - 1);
    }

    #endregion

}