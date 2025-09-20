namespace RoboSalvamento.Core;

public class Posicao
{
    public Posicao(int linha, int coluna)
    {
        Linha = linha;
        Coluna = coluna;
    }

    public int Linha { get; private set; }
    public int Coluna { get; private set; }

    public override bool Equals(object? obj)
    {
        return obj is Posicao posicao && Linha == posicao.Linha && Coluna == posicao.Coluna;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Linha, Coluna);
    }

    public override string ToString()
    {
        return $"({Linha}, {Coluna})";
    }

}