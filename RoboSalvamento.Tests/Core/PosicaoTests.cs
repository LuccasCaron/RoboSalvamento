using RoboSalvamento.Core;

namespace RoboSalvamento.Tests.Core;

[Trait("Categoria", "Testes de Posição")]
public sealed class PosicaoTests
{
    #region Testes do Construtor

    [Fact]
    public void Construtor_DadosParametrosValidos_DeveDefinirPropriedadesCorretamente()
    {
        // arrange
        int linhaEsperada = 5;
        int colunaEsperada = 10;

        // action
        var posicao = new Posicao(linhaEsperada, colunaEsperada);

        // assert
        Assert.Equal(linhaEsperada, posicao.Linha);
        Assert.Equal(colunaEsperada, posicao.Coluna);
    }

    [Fact]
    public void Construtor_DadosParametrosNegativos_DeveDefinirPropriedadesCorretamente()
    {
        // arrange
        int linhaEsperada = -1;
        int colunaEsperada = -5;

        // action
        var posicao = new Posicao(linhaEsperada, colunaEsperada);

        // assert
        Assert.Equal(linhaEsperada, posicao.Linha);
        Assert.Equal(colunaEsperada, posicao.Coluna);
    }

    [Fact]
    public void Construtor_DadosParametrosZero_DeveDefinirPropriedadesCorretamente()
    {
        // arrange
        int linhaEsperada = 0;
        int colunaEsperada = 0;

        // action
        var posicao = new Posicao(linhaEsperada, colunaEsperada);

        // assert
        Assert.Equal(linhaEsperada, posicao.Linha);
        Assert.Equal(colunaEsperada, posicao.Coluna);
    }

    #endregion

    #region Testes do Método Equals

    [Fact]
    public void Equals_DadosObjetosIguais_DeveRetornarTrue()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        var posicao2 = new Posicao(3, 7);

        // action
        bool resultado = posicao1.Equals(posicao2);

        // assert
        Assert.True(resultado);
    }

    [Fact]
    public void Equals_DadosObjetosDiferentes_DeveRetornarFalse()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        var posicao2 = new Posicao(3, 8);

        // action
        bool resultado = posicao1.Equals(posicao2);

        // assert
        Assert.False(resultado);
    }

    [Fact]
    public void Equals_DadosObjetosComLinhasDiferentes_DeveRetornarFalse()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        var posicao2 = new Posicao(4, 7);

        // action
        bool resultado = posicao1.Equals(posicao2);

        // assert
        Assert.False(resultado);
    }

    [Fact]
    public void Equals_DadosObjetoNulo_DeveRetornarFalse()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        Posicao? posicao2 = null;

        // action
        bool resultado = posicao1.Equals(posicao2);

        // assert
        Assert.False(resultado);
    }

    [Fact]
    public void Equals_DadosObjetoDeTipoDiferente_DeveRetornarFalse()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        var objetoDiferente = "string";

        // action
        bool resultado = posicao1.Equals(objetoDiferente);

        // assert
        Assert.False(resultado);
    }

    #endregion

    #region Testes do Método GetHashCode

    [Fact]
    public void GetHashCode_DadosObjetosIguais_DeveRetornarHashCodeIgual()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        var posicao2 = new Posicao(3, 7);

        // action
        int hashCode1 = posicao1.GetHashCode();
        int hashCode2 = posicao2.GetHashCode();

        // assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_DadosObjetosDiferentes_DeveRetornarHashCodeDiferente()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        var posicao2 = new Posicao(3, 8);

        // action
        int hashCode1 = posicao1.GetHashCode();
        int hashCode2 = posicao2.GetHashCode();

        // assert
        Assert.NotEqual(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_DadosObjetosComValoresNegativos_DeveRetornarHashCodeConsistente()
    {
        // arrange
        var posicao1 = new Posicao(-3, -7);
        var posicao2 = new Posicao(-3, -7);

        // action
        int hashCode1 = posicao1.GetHashCode();
        int hashCode2 = posicao2.GetHashCode();

        // assert
        Assert.Equal(hashCode1, hashCode2);
    }

    #endregion

    #region Testes do Método ToString

    [Fact]
    public void ToString_DadosValoresPositivos_DeveRetornarFormatoCorreto()
    {
        // arrange
        var posicao = new Posicao(5, 10);
        string resultadoEsperado = "(5, 10)";

        // action
        string resultado = posicao.ToString();

        // assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void ToString_DadosValoresNegativos_DeveRetornarFormatoCorreto()
    {
        // arrange
        var posicao = new Posicao(-3, -7);
        string resultadoEsperado = "(-3, -7)";

        // action
        string resultado = posicao.ToString();

        // assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void ToString_DadosValoresZero_DeveRetornarFormatoCorreto()
    {
        // arrange
        var posicao = new Posicao(0, 0);
        string resultadoEsperado = "(0, 0)";

        // action
        string resultado = posicao.ToString();

        // assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    #endregion

    #region Testes de Igualdade com Operador ==

    [Fact]
    public void Igualdade_DadosObjetosIguais_DeveRetornarTrue()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        var posicao2 = new Posicao(3, 7);

        // action & assert
        Assert.True(posicao1.Equals(posicao2));
    }

    #endregion

    #region Testes de HashSet

    [Fact]
    public void HashSet_DadosObjetosIguais_DeveConsiderarComoMesmoElemento()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        var posicao2 = new Posicao(3, 7);
        var hashSet = new HashSet<Posicao>();

        // action
        hashSet.Add(posicao1);
        bool contemPosicao2 = hashSet.Contains(posicao2);

        // assert
        Assert.True(contemPosicao2);
        Assert.Single(hashSet);
    }

    [Fact]
    public void HashSet_DadosObjetosDiferentes_DeveConsiderarComoElementosDiferentes()
    {
        // arrange
        var posicao1 = new Posicao(3, 7);
        var posicao2 = new Posicao(3, 8);
        var hashSet = new HashSet<Posicao>();

        // action
        hashSet.Add(posicao1);
        hashSet.Add(posicao2);

        // assert
        Assert.Equal(2, hashSet.Count);
    }

    #endregion
}
