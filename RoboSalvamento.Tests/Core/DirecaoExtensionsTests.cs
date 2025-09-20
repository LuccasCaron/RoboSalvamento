using RoboSalvamento.Core;

namespace RoboSalvamento.Tests.Core;

[Trait("Categoria", "Testes de Extensões de Direção")]
public sealed class DirecaoExtensionsTests
{
    #region Testes do Método GirarDireita

    [Fact]
    public void GirarDireita_DadoNorte_DeveRetornarLeste()
    {
        // arrange
        var direcao = EDirecao.Norte;

        // action
        var resultado = direcao.GirarDireita();

        // assert
        Assert.Equal(EDirecao.Leste, resultado);
    }

    [Fact]
    public void GirarDireita_DadoLeste_DeveRetornarSul()
    {
        // arrange
        var direcao = EDirecao.Leste;

        // action
        var resultado = direcao.GirarDireita();

        // assert
        Assert.Equal(EDirecao.Sul, resultado);
    }

    [Fact]
    public void GirarDireita_DadoSul_DeveRetornarOeste()
    {
        // arrange
        var direcao = EDirecao.Sul;

        // action
        var resultado = direcao.GirarDireita();

        // assert
        Assert.Equal(EDirecao.Oeste, resultado);
    }

    [Fact]
    public void GirarDireita_DadoOeste_DeveRetornarNorte()
    {
        // arrange
        var direcao = EDirecao.Oeste;

        // action
        var resultado = direcao.GirarDireita();

        // assert
        Assert.Equal(EDirecao.Norte, resultado);
    }

    [Fact]
    public void GirarDireita_DadoNorteComQuatroGiros_DeveRetornarNorte()
    {
        // arrange
        var direcao = EDirecao.Norte;

        // action
        var resultado = direcao.GirarDireita()
            .GirarDireita()
            .GirarDireita()
            .GirarDireita();

        // assert
        Assert.Equal(EDirecao.Norte, resultado);
    }

    #endregion

    #region Testes do Método ObterPosicaoFrente

    [Fact]
    public void ObterPosicaoFrente_DadoNorte_DeveRetornarPosicaoAcima()
    {
        // arrange
        var direcao = EDirecao.Norte;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(4, 5);

        // action
        var resultado = direcao.ObterPosicaoFrente(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoFrente_DadoLeste_DeveRetornarPosicaoADireita()
    {
        // arrange
        var direcao = EDirecao.Leste;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(5, 6);

        // action
        var resultado = direcao.ObterPosicaoFrente(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoFrente_DadoSul_DeveRetornarPosicaoAbaixo()
    {
        // arrange
        var direcao = EDirecao.Sul;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(6, 5);

        // action
        var resultado = direcao.ObterPosicaoFrente(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoFrente_DadoOeste_DeveRetornarPosicaoAEsquerda()
    {
        // arrange
        var direcao = EDirecao.Oeste;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(5, 4);

        // action
        var resultado = direcao.ObterPosicaoFrente(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoFrente_DadoPosicaoZeroZero_DeveRetornarPosicaoCorreta()
    {
        // arrange
        var direcao = EDirecao.Norte;
        var posicaoAtual = new Posicao(0, 0);
        var posicaoEsperada = new Posicao(-1, 0);

        // action
        var resultado = direcao.ObterPosicaoFrente(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    #endregion

    #region Testes do Método ObterPosicaoEsquerda

    [Fact]
    public void ObterPosicaoEsquerda_DadoNorte_DeveRetornarPosicaoAOeste()
    {
        // arrange
        var direcao = EDirecao.Norte;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(5, 4);

        // action
        var resultado = direcao.ObterPosicaoEsquerda(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoEsquerda_DadoLeste_DeveRetornarPosicaoAoNorte()
    {
        // arrange
        var direcao = EDirecao.Leste;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(4, 5);

        // action
        var resultado = direcao.ObterPosicaoEsquerda(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoEsquerda_DadoSul_DeveRetornarPosicaoALeste()
    {
        // arrange
        var direcao = EDirecao.Sul;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(5, 6);

        // action
        var resultado = direcao.ObterPosicaoEsquerda(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoEsquerda_DadoOeste_DeveRetornarPosicaoAoSul()
    {
        // arrange
        var direcao = EDirecao.Oeste;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(6, 5);

        // action
        var resultado = direcao.ObterPosicaoEsquerda(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    #endregion

    #region Testes do Método ObterPosicaoDireita

    [Fact]
    public void ObterPosicaoDireita_DadoNorte_DeveRetornarPosicaoALeste()
    {
        // arrange
        var direcao = EDirecao.Norte;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(5, 6);

        // action
        var resultado = direcao.ObterPosicaoDireita(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoDireita_DadoLeste_DeveRetornarPosicaoAoSul()
    {
        // arrange
        var direcao = EDirecao.Leste;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(6, 5);

        // action
        var resultado = direcao.ObterPosicaoDireita(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoDireita_DadoSul_DeveRetornarPosicaoAOeste()
    {
        // arrange
        var direcao = EDirecao.Sul;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(5, 4);

        // action
        var resultado = direcao.ObterPosicaoDireita(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ObterPosicaoDireita_DadoOeste_DeveRetornarPosicaoAoNorte()
    {
        // arrange
        var direcao = EDirecao.Oeste;
        var posicaoAtual = new Posicao(5, 5);
        var posicaoEsperada = new Posicao(4, 5);

        // action
        var resultado = direcao.ObterPosicaoDireita(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    #endregion

    #region Testes de Consistência

    [Fact]
    public void Consistencia_DadoNorte_EsquerdaDeveSerOpostoDeDireita()
    {
        // arrange
        var direcao = EDirecao.Norte;
        var posicaoAtual = new Posicao(5, 5);

        // action
        var esquerda = direcao.ObterPosicaoEsquerda(posicaoAtual);
        var direita = direcao.ObterPosicaoDireita(posicaoAtual);

        // assert
        // Esquerda deve ser oeste (5,4) e direita deve ser leste (5,6)
        Assert.Equal(new Posicao(5, 4), esquerda);
        Assert.Equal(new Posicao(5, 6), direita);
    }

    [Fact]
    public void Consistencia_DadoLeste_EsquerdaDeveSerOpostoDeDireita()
    {
        // arrange
        var direcao = EDirecao.Leste;
        var posicaoAtual = new Posicao(5, 5);

        // action
        var esquerda = direcao.ObterPosicaoEsquerda(posicaoAtual);
        var direita = direcao.ObterPosicaoDireita(posicaoAtual);

        // assert
        // Esquerda deve ser norte (4,5) e direita deve ser sul (6,5)
        Assert.Equal(new Posicao(4, 5), esquerda);
        Assert.Equal(new Posicao(6, 5), direita);
    }

    [Fact]
    public void Consistencia_DadoSul_EsquerdaDeveSerOpostoDeDireita()
    {
        // arrange
        var direcao = EDirecao.Sul;
        var posicaoAtual = new Posicao(5, 5);

        // action
        var esquerda = direcao.ObterPosicaoEsquerda(posicaoAtual);
        var direita = direcao.ObterPosicaoDireita(posicaoAtual);

        // assert
        // Esquerda deve ser leste (5,6) e direita deve ser oeste (5,4)
        Assert.Equal(new Posicao(5, 6), esquerda);
        Assert.Equal(new Posicao(5, 4), direita);
    }

    [Fact]
    public void Consistencia_DadoOeste_EsquerdaDeveSerOpostoDeDireita()
    {
        // arrange
        var direcao = EDirecao.Oeste;
        var posicaoAtual = new Posicao(5, 5);

        // action
        var esquerda = direcao.ObterPosicaoEsquerda(posicaoAtual);
        var direita = direcao.ObterPosicaoDireita(posicaoAtual);

        // assert
        // Esquerda deve ser sul (6,5) e direita deve ser norte (4,5)
        Assert.Equal(new Posicao(6, 5), esquerda);
        Assert.Equal(new Posicao(4, 5), direita);
    }

    #endregion

    #region Testes de Valores Limite

    [Fact]
    public void ValoresLimite_DadoPosicaoNegativa_DeveCalcularCorretamente()
    {
        // arrange
        var direcao = EDirecao.Norte;
        var posicaoAtual = new Posicao(-1, -1);
        var posicaoEsperada = new Posicao(-2, -1);

        // action
        var resultado = direcao.ObterPosicaoFrente(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    [Fact]
    public void ValoresLimite_DadoPosicaoMaxima_DeveCalcularCorretamente()
    {
        // arrange
        var direcao = EDirecao.Leste;
        var posicaoAtual = new Posicao(int.MaxValue - 1, int.MaxValue - 1);
        var posicaoEsperada = new Posicao(int.MaxValue - 1, int.MaxValue);

        // action
        var resultado = direcao.ObterPosicaoFrente(posicaoAtual);

        // assert
        Assert.Equal(posicaoEsperada, resultado);
    }

    #endregion
}
