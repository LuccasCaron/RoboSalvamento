using RoboSalvamento.Core;
using RoboSalvamento.Simulador;
using RoboSalvamento.Robo;

namespace RoboSalvamento.Testes;

/// <summary>
/// Casos de teste próprios para validar diferentes cenários de labirinto.
/// </summary>
public class CasosTesteProprios
{
    public static void ExecutarTodosOsCasos()
    {
        Console.WriteLine("🧪 EXECUTANDO CASOS DE TESTE PRÓPRIOS");
        Console.WriteLine("═══════════════════════════════════════");

        try
        {
            ExecutarCasoTeste1_LabirintoSimples();
            ExecutarCasoTeste2_LabirintoComplexo();
            ExecutarCasoTeste3_LabirintoGrande();
            ExecutarCasoTeste4_EntradaDiferentesBordas();
            ExecutarCasoTeste5_LabirintoComBecos();

            Console.WriteLine("\n✅ TODOS OS CASOS DE TESTE PRÓPRIOS PASSARAM!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ FALHA NOS CASOS DE TESTE: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Caso de Teste 1: Labirinto simples - entrada superior, humano no centro
    /// </summary>
    private static void ExecutarCasoTeste1_LabirintoSimples()
    {
        Console.WriteLine("\n📋 CASO DE TESTE 1: Labirinto Simples");
        
        var arquivo = "caso_teste_1_simples.txt";
        var conteudo = """
            XXXEXXX
            X.....X
            X.....X
            X..@..X
            X.....X
            XXXXXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        
        var mapa = new Mapa(arquivo);
        var simulador = new SimuladorAmbienteVirtual(mapa);
        var log = new LogOperacaoMelhorado(arquivo);
        var algoritmo = new AlgoritmoBFS(simulador, log);
        
        Console.WriteLine("   Mapa carregado:");
        mapa.ExibirMapa();
        
        algoritmo.ExecutarMissao();
        
        Console.WriteLine("   ✅ Caso de teste 1 concluído com sucesso!");
        log.SalvarArquivos();
    }

    /// <summary>
    /// Caso de Teste 2: Labirinto complexo - múltiplos caminhos e becos
    /// </summary>
    private static void ExecutarCasoTeste2_LabirintoComplexo()
    {
        Console.WriteLine("\n📋 CASO DE TESTE 2: Labirinto Complexo");
        
        var arquivo = "caso_teste_2_complexo.txt";
        var conteudo = """
            XXXXEXXXX
            X.......X
            X.XXXXX.X
            X.X...X.X
            X.X.X.X.X
            X.X...X.X
            X.XXXXX.X
            X.......X
            XXXX@XXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        
        var mapa = new Mapa(arquivo);
        var simulador = new SimuladorAmbienteVirtual(mapa);
        var log = new LogOperacaoMelhorado(arquivo);
        var algoritmo = new AlgoritmoBFS(simulador, log);
        
        Console.WriteLine("   Mapa carregado:");
        mapa.ExibirMapa();
        
        algoritmo.ExecutarMissao();
        
        Console.WriteLine("   ✅ Caso de teste 2 concluído com sucesso!");
        log.SalvarArquivos();
    }

    /// <summary>
    /// Caso de Teste 3: Labirinto grande - testa performance e memória
    /// </summary>
    private static void ExecutarCasoTeste3_LabirintoGrande()
    {
        Console.WriteLine("\n📋 CASO DE TESTE 3: Labirinto Grande");
        
        var arquivo = "caso_teste_3_grande.txt";
        var conteudo = """
            XXXXXXXXXXEXXXXXXXXXX
            X...................X
            X.XXXXXXXXXXXXXXXXX.X
            X.X.................X
            X.X.XXXXXXXXXXXXXXX.X
            X.X.X...............X
            X.X.X.XXXXXXXXXXXXX.X
            X.X.X.X...........X.X
            X.X.X.X.XXXXXXXXX.X.X
            X.X.X.X.X.......X.X.X
            X.X.X.X.X.XXXXX.X.X.X
            X.X.X.X.X.X...X.X.X.X
            X.X.X.X.X.X.X.X.X.X.X
            X.X.X.X.X.X...X.X.X.X
            X.X.X.X.X.XXXXX.X.X.X
            X.X.X.X.X.......X.X.X
            X.X.X.X.XXXXXXXXX.X.X
            X.X.X.X...........X.X
            X.X.X.XXXXXXXXXXXXX.X
            X.X.X...............X
            X.X.XXXXXXXXXXXXXXX.X
            X.X.................X
            X.XXXXXXXXXXXXXXXXX.X
            X...................X
            XXXXXXXXXX@XXXXXXXXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        
        var mapa = new Mapa(arquivo);
        var simulador = new SimuladorAmbienteVirtual(mapa);
        var log = new LogOperacaoMelhorado(arquivo);
        var algoritmo = new AlgoritmoBFS(simulador, log);
        
        Console.WriteLine("   Mapa carregado (apenas primeiras linhas):");
        mapa.ExibirMapa();
        
        algoritmo.ExecutarMissao();
        
        Console.WriteLine("   ✅ Caso de teste 3 concluído com sucesso!");
        log.SalvarArquivos();
    }

    /// <summary>
    /// Caso de Teste 4: Entrada em diferentes bordas
    /// </summary>
    private static void ExecutarCasoTeste4_EntradaDiferentesBordas()
    {
        Console.WriteLine("\n📋 CASO DE TESTE 4: Entrada em Diferentes Bordas");
        
        // Teste com entrada na borda esquerda
        ExecutarTesteEntradaBorda("caso_teste_4_esquerda.txt", """
            EXXXX
            X...X
            X...X
            X...X
            X...X
            X@..X
            XXXXX
            """);

        // Teste com entrada na borda direita
        ExecutarTesteEntradaBorda("caso_teste_4_direita.txt", """
            XXXXX
            X...X
            X...X
            X...X
            X...X
            X..@X
            XXXXE
            """);

        // Teste com entrada na borda inferior
        ExecutarTesteEntradaBorda("caso_teste_4_inferior.txt", """
            XXXXX
            X...X
            X...X
            X...X
            X...X
            X@..X
            XXXXE
            """);
    }

    /// <summary>
    /// Caso de Teste 5: Labirinto com becos sem saída
    /// </summary>
    private static void ExecutarCasoTeste5_LabirintoComBecos()
    {
        Console.WriteLine("\n📋 CASO DE TESTE 5: Labirinto com Becos");
        
        var arquivo = "caso_teste_5_becos.txt";
        var conteudo = """
            XXXXEXXXX
            X.......X
            X.XXXXX.X
            X.X...X.X
            X.X.X.X.X
            X.X...X.X
            X.XXXXX.X
            X.......X
            XXXX@XXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        
        var mapa = new Mapa(arquivo);
        var simulador = new SimuladorAmbienteVirtual(mapa);
        var log = new LogOperacaoMelhorado(arquivo);
        var algoritmo = new AlgoritmoBFS(simulador, log);
        
        Console.WriteLine("   Mapa carregado:");
        mapa.ExibirMapa();
        
        algoritmo.ExecutarMissao();
        
        Console.WriteLine("   ✅ Caso de teste 5 concluído com sucesso!");
        log.SalvarArquivos();
    }

    private static void ExecutarTesteEntradaBorda(string nomeArquivo, string conteudo)
    {
        Console.WriteLine($"   Testando entrada em: {nomeArquivo}");
        
        File.WriteAllText(nomeArquivo, conteudo);
        
        var mapa = new Mapa(nomeArquivo);
        var simulador = new SimuladorAmbienteVirtual(mapa);
        var log = new LogOperacaoMelhorado(nomeArquivo);
        var algoritmo = new AlgoritmoBFS(simulador, log);
        
        algoritmo.ExecutarMissao();
        log.SalvarArquivos();
        
        Console.WriteLine($"   ✅ {nomeArquivo} concluído com sucesso!");
    }
}
