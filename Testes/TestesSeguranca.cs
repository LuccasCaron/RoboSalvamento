using RoboSalvamento.Core;
using RoboSalvamento.Simulador;

namespace RoboSalvamento.Testes;

/// <summary>
/// Casos de teste para validar todos os requisitos de segurança do robô.
/// </summary>
public class TestesSeguranca
{
    public static void ExecutarTodosOsTestes()
    {
        Console.WriteLine("🧪 EXECUTANDO TESTES DE SEGURANÇA");
        Console.WriteLine("═══════════════════════════════════");

        try
        {
            TestarAlarmeColisaoParede();
            TestarAlarmeAtropelamentoHumano();
            TestarAlarmeBecoSemSaida();
            TestarAlarmeEjecaoSemHumano();
            TestarAlarmeColetaSemHumano();
            TestarValidacaoEntradaBorda();
            TestarValidacaoMapaValido();

            Console.WriteLine("\n✅ TODOS OS TESTES DE SEGURANÇA PASSARAM!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ FALHA NOS TESTES: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Teste 1: Alarme de colisão com paredes
    /// </summary>
    private static void TestarAlarmeColisaoParede()
    {
        Console.WriteLine("\n🚨 TESTE 1: Alarme de colisão com parede");
        
        var mapa = CriarMapaTesteColisao();
        var simulador = new SimuladorAmbienteVirtual(mapa);
        
        // Ligar o robô
        simulador.ExecutarComando(EComandoRobo.LIGAR);
        
        // Verificar se o robô está na posição correta
        Console.WriteLine($"   Posição do robô: {simulador.PosicaoRobo}");
        Console.WriteLine($"   Direção do robô: {simulador.DirecaoRobo}");
        
        // Tentar avançar para uma parede (deve dar erro)
        try
        {
            simulador.ExecutarComando(EComandoRobo.Avancar);
            throw new Exception("❌ FALHA: Deveria ter disparado alarme de colisão!");
        }
        catch (DomainException ex) when (ex.Message.Contains("ALARME DE COLISÃO"))
        {
            Console.WriteLine("   ✅ Alarme de colisão funcionando corretamente");
        }
    }

    /// <summary>
    /// Teste 2: Alarme de atropelamento de humano
    /// </summary>
    private static void TestarAlarmeAtropelamentoHumano()
    {
        Console.WriteLine("\n🚨 TESTE 2: Alarme de atropelamento de humano");
        
        var mapa = CriarMapaTesteAtropelamento();
        var simulador = new SimuladorAmbienteVirtual(mapa);
        
        // Ligar o robô
        simulador.ExecutarComando(EComandoRobo.LIGAR);
        
        // Tentar avançar para o humano (deve dar erro)
        try
        {
            simulador.ExecutarComando(EComandoRobo.Avancar);
            throw new Exception("❌ FALHA: Deveria ter disparado alarme de atropelamento!");
        }
        catch (DomainException ex) when (ex.Message.Contains("ALARME DE ATROPELAMENTO"))
        {
            Console.WriteLine("   ✅ Alarme de atropelamento funcionando corretamente");
        }
    }

    /// <summary>
    /// Teste 3: Alarme de beco sem saída após coleta
    /// </summary>
    private static void TestarAlarmeBecoSemSaida()
    {
        Console.WriteLine("\n🚨 TESTE 3: Alarme de beco sem saída");
        
        var mapa = CriarMapaTesteBecoSemSaida();
        var simulador = new SimuladorAmbienteVirtual(mapa);
        
        // Ligar o robô
        simulador.ExecutarComando(EComandoRobo.LIGAR);
        
        // Avançar para pegar o humano
        simulador.ExecutarComando(EComandoRobo.Avancar);
        simulador.ExecutarComando(EComandoRobo.PegarHumano);
        
        // Tentar avançar para um beco sem saída (deve dar erro)
        try
        {
            simulador.ExecutarComando(EComandoRobo.Avancar);
            throw new Exception("❌ FALHA: Deveria ter disparado alarme de beco sem saída!");
        }
        catch (DomainException ex) when (ex.Message.Contains("ALARME DE BECO SEM SAÍDA"))
        {
            Console.WriteLine("   ✅ Alarme de beco sem saída funcionando corretamente");
        }
    }

    /// <summary>
    /// Teste 4: Alarme de ejeção sem humano
    /// </summary>
    private static void TestarAlarmeEjecaoSemHumano()
    {
        Console.WriteLine("\n🚨 TESTE 4: Alarme de ejeção sem humano");
        
        var mapa = CriarMapaTesteSimples();
        var simulador = new SimuladorAmbienteVirtual(mapa);
        
        // Ligar o robô
        simulador.ExecutarComando(EComandoRobo.LIGAR);
        
        // Tentar ejetar sem ter coletado o humano (deve dar erro)
        try
        {
            simulador.ExecutarComando(EComandoRobo.EjetarHumano);
            throw new Exception("❌ FALHA: Deveria ter disparado alarme de ejeção sem humano!");
        }
        catch (DomainException ex) when (ex.Message.Contains("ALARME DE EJEÇÃO INVÁLIDA"))
        {
            Console.WriteLine("   ✅ Alarme de ejeção sem humano funcionando corretamente");
        }
    }

    /// <summary>
    /// Teste 5: Alarme de coleta sem humano à frente
    /// </summary>
    private static void TestarAlarmeColetaSemHumano()
    {
        Console.WriteLine("\n🚨 TESTE 5: Alarme de coleta sem humano");
        
        var mapa = CriarMapaTesteSimples();
        var simulador = new SimuladorAmbienteVirtual(mapa);
        
        // Ligar o robô
        simulador.ExecutarComando(EComandoRobo.LIGAR);
        
        // Tentar pegar humano sem ter humano à frente (deve dar erro)
        try
        {
            simulador.ExecutarComando(EComandoRobo.PegarHumano);
            throw new Exception("❌ FALHA: Deveria ter disparado alarme de coleta sem humano!");
        }
        catch (DomainException ex) when (ex.Message.Contains("ALARME DE COLETA INVÁLIDA"))
        {
            Console.WriteLine("   ✅ Alarme de coleta sem humano funcionando corretamente");
        }
    }

    /// <summary>
    /// Teste 6: Validação de entrada na borda
    /// </summary>
    private static void TestarValidacaoEntradaBorda()
    {
        Console.WriteLine("\n🚨 TESTE 6: Validação de entrada na borda");
        
        // Teste com entrada no meio (deve dar erro)
        try
        {
            var mapa = CriarMapaTesteEntradaInvalida();
            var simulador = new SimuladorAmbienteVirtual(mapa);
            throw new Exception("❌ FALHA: Deveria ter rejeitado entrada no meio!");
        }
        catch (DomainException ex) when (ex.Message.Contains("Entrada não está na borda"))
        {
            Console.WriteLine("   ✅ Validação de entrada na borda funcionando corretamente");
        }
    }

    /// <summary>
    /// Teste 7: Validação de mapa válido
    /// </summary>
    private static void TestarValidacaoMapaValido()
    {
        Console.WriteLine("\n🚨 TESTE 7: Validação de mapa válido");
        
        // Teste com múltiplas entradas (deve dar erro)
        try
        {
            var mapa = CriarMapaTesteMultiplasEntradas();
            throw new Exception("❌ FALHA: Deveria ter rejeitado múltiplas entradas!");
        }
        catch (DomainException ex) when (ex.Message.Contains("múltiplas entradas") || ex.Message.Contains("Encontradas"))
        {
            Console.WriteLine("   ✅ Validação de múltiplas entradas funcionando corretamente");
        }

        // Teste sem humano (deve dar erro)
        try
        {
            var mapa = CriarMapaTesteSemHumano();
            throw new Exception("❌ FALHA: Deveria ter rejeitado mapa sem humano!");
        }
        catch (DomainException ex) when (ex.Message.Contains("Nenhum humano"))
        {
            Console.WriteLine("   ✅ Validação de mapa sem humano funcionando corretamente");
        }
    }

    #region Métodos auxiliares para criar mapas de teste

    private static Mapa CriarMapaTesteColisao()
    {
        var arquivo = "teste_colisao.txt";
        var conteudo = """
            XEXX
            XXXX
            X@XX
            XXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        return new Mapa(arquivo);
    }

    private static Mapa CriarMapaTesteAtropelamento()
    {
        var arquivo = "teste_atropelamento.txt";
        var conteudo = """
            XEXX
            X@XX
            X.XX
            XXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        return new Mapa(arquivo);
    }

    private static Mapa CriarMapaTesteBecoSemSaida()
    {
        var arquivo = "teste_beco_sem_saida.txt";
        var conteudo = """
            XEXX
            X.XX
            X@XX
            XXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        return new Mapa(arquivo);
    }

    private static Mapa CriarMapaTesteSimples()
    {
        var arquivo = "teste_simples.txt";
        var conteudo = """
            XEXX
            X.XX
            X.XX
            X@XX
            XXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        return new Mapa(arquivo);
    }

    private static Mapa CriarMapaTesteEntradaInvalida()
    {
        var arquivo = "teste_entrada_invalida.txt";
        var conteudo = """
            XXXX
            XEXX
            X.XX
            X@XX
            XXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        return new Mapa(arquivo);
    }

    private static Mapa CriarMapaTesteMultiplasEntradas()
    {
        var arquivo = "teste_multiplas_entradas.txt";
        var conteudo = """
            XEXX
            X.XX
            XEXX
            X@XX
            XXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        return new Mapa(arquivo);
    }

    private static Mapa CriarMapaTesteSemHumano()
    {
        var arquivo = "teste_sem_humano.txt";
        var conteudo = """
            XEXX
            X.XX
            X.XX
            X.XX
            XXXX
            """;
        File.WriteAllText(arquivo, conteudo);
        return new Mapa(arquivo);
    }

    #endregion
}
