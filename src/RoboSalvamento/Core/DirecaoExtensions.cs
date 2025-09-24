namespace RoboSalvamento.Core;

public static class DirecaoExtensions
{
    public static EDirecao GirarDireita(this EDirecao direcao)
    {
        return (EDirecao)(((int)direcao + 1) % 4);
    }

    public static Posicao ObterPosicaoFrente(this EDirecao direcao, Posicao posicaoAtual)
    {
        return direcao switch
        {
            EDirecao.Norte => new Posicao(posicaoAtual.Linha - 1, posicaoAtual.Coluna),
            EDirecao.Leste => new Posicao(posicaoAtual.Linha, posicaoAtual.Coluna + 1),
            EDirecao.Sul => new Posicao(posicaoAtual.Linha + 1, posicaoAtual.Coluna),
            EDirecao.Oeste => new Posicao(posicaoAtual.Linha, posicaoAtual.Coluna - 1),
            _ => throw new ArgumentException("Direção inválida")
        };
    }

    public static Posicao ObterPosicaoEsquerda(this EDirecao direcao, Posicao posicaoAtual)
    {
        var direcaoEsquerda = (EDirecao)(((int)direcao + 3) % 4); // Gira 3x para direita = 1x para esquerda
        return direcaoEsquerda.ObterPosicaoFrente(posicaoAtual);
    }

    public static Posicao ObterPosicaoDireita(this EDirecao direcao, Posicao posicaoAtual)
    {
        var direcaoDireita = direcao.GirarDireita();
        return direcaoDireita.ObterPosicaoFrente(posicaoAtual);
    }
}
