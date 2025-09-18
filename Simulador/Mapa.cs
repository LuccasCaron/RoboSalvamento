using RoboSalvamento.Core;

namespace RoboSalvamento;

public class Mapa
{
    public Mapa(string caminhoDoArquivo)
    {
        CarregarArquivo(caminhoDoArquivo);
    }

    public char[,] Labirinto { get; private set; } = null!;
    public int QuantidadeDeLinhas { get; private set; }
    public int QuantidadeDeColunas { get; private set; }
    public Posicao Entrada { get; private set; } = null!;
    public Posicao Humano { get; private set; } = null!;

    private void CarregarArquivo(string caminhoDoArquivo)
    {
        string[] linhasDoArquivo = File.ReadAllLines(caminhoDoArquivo);

        if (linhasDoArquivo.Length == 0)
            throw new DomainException("Arquivo vazio!");

        if (linhasDoArquivo[0].Length == 0)
            throw new DomainException("Primeira linha vazia!");

        QuantidadeDeLinhas = linhasDoArquivo.Length;
        QuantidadeDeColunas = linhasDoArquivo[0].Length;
        Labirinto = new char[QuantidadeDeLinhas, QuantidadeDeColunas];

        int entradasEncontradas = 0;
        int humanosEncontrados = 0;

        for (int i = 0; i < QuantidadeDeLinhas; i++)
        {
            if (linhasDoArquivo[i].Length != QuantidadeDeColunas)
                throw new DomainException($"Linha {i} tem tamanho diferente! Esperado: {QuantidadeDeColunas}, Encontrado: {linhasDoArquivo[i].Length}");

            for (int j = 0; j < QuantidadeDeColunas; j++)
            {
                char caractere = linhasDoArquivo[i][j];
                Labirinto[i, j] = caractere;
                if (caractere == 'E')
                {
                    entradasEncontradas++;
                    Entrada = new Posicao(i, j);
                }
                else if (caractere == '@')
                {
                    humanosEncontrados++;
                    Humano = new Posicao(i, j);
                }
            }
        }

        if (entradasEncontradas == 0)
            throw new DomainException("Labirinto inválido: Nenhuma entrada (E) encontrada!");

        if (entradasEncontradas > 1)
            throw new DomainException($"Labirinto inválido: Encontradas {entradasEncontradas} entradas, deve haver apenas 1!");

        if (humanosEncontrados == 0)
            throw new DomainException("Labirinto inválido: Nenhum humano (@) encontrado!");

        if (humanosEncontrados > 1)
            throw new DomainException($"Labirinto inválido: Encontrados {humanosEncontrados} humanos, deve haver apenas 1!");
    }


    public void ExibirMapa()
    {
        Console.WriteLine("🗺️  MAPA CARREGADO:");
        Console.WriteLine("═══════════════════");

        for (int i = 0; i < QuantidadeDeLinhas; i++)
        {
            // Número da linha para facilitar debug
            Console.Write($"{i:D2} │ ");

            // Exibe cada caractere com cores
            for (int j = 0; j < QuantidadeDeColunas; j++)
            {
                char caractere = Labirinto[i, j];
                ExibirCaractereColorido(caractere);
            }

            Console.WriteLine(); 
        }

        Console.WriteLine("═══════════════════");
    }

    static void ExibirCaractereColorido(char c)
    {
        switch (c)
        {
            case 'X': // Parede
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write('█'); // Bloco sólido
                break;

            case '.': // Espaço livre
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write('·'); // Ponto pequeno
                break;

            case 'E': // Entrada
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write('E');
                break;

            case '@': // Humano
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write('@');
                break;

            case ' ': // Espaço vazio (se houver)
                Console.Write(' ');
                break;

            default: // Caractere desconhecido
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write('?');
                break;
        }

        Console.ResetColor(); // Volta cor normal
    }

}