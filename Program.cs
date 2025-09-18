using RoboSalvamento;
using RoboSalvamento.Core;
using RoboSalvamento.Robo;
using RoboSalvamento.Simulador;

Console.WriteLine("🤖 ROBÔ DE SALVAMENTO - SISTEMA COMPLETO");
Console.WriteLine("═══════════════════════════════════════════");
Console.WriteLine("Desenvolvido por: [Luccas Caron] - [2023100247] e [Leonardo Lemes] - [2023103996]");
Console.WriteLine("Disciplina: Serviços Cognitivos - Atividade Discente Supervisionada 1");
Console.WriteLine("Professor: Mozart Hasse");
Console.WriteLine("═══════════════════════════════════════════");

// Pedir caminho do arquivo interativamente para debug
string arquivoMapa;
if (args.Length == 0)
{
    Console.WriteLine("📁 Digite o caminho do arquivo de mapa:");
    Console.Write("> ");
    arquivoMapa = Console.ReadLine() ?? "";
}
else
{
    arquivoMapa = args[0];
}


if (!File.Exists(arquivoMapa))
{
    Console.WriteLine($"❌ ERRO: Arquivo '{arquivoMapa}' não encontrado!");
    Console.WriteLine("Pressione qualquer tecla para sair...");
    Console.ReadKey();
    return;
}

try
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
    cts.Token.Register(() => Console.WriteLine("\n⏰ TIMEOUT: Operação cancelada após 5 segundos!"));

    ProcessarArquivoMapa(arquivoMapa, cts.Token);

    Console.WriteLine("\n✅ MISSÃO CONCLUÍDA COM SUCESSO!");
}
catch (OperationCanceledException)
{
    Console.WriteLine("\n⏰ OPERAÇÃO CANCELADA: Timeout excedido!");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ ERRO: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();

static void ProcessarArquivoMapa(string arquivoMapa, CancellationToken cancellationToken = default)
{
    try
    {
        var mapa = new Mapa(arquivoMapa);
        var simulador = new SimuladorAmbienteVirtual(mapa);
        var log = new LogOperacaoMelhorado(arquivoMapa);
        var algoritmo = new AlgoritmoBuscaInteligenteV2(simulador, log);

        Console.WriteLine("🗺️ MAPA CARREGADO:");
        mapa.ExibirMapa();

        Console.WriteLine("\n🤖 INICIANDO MISSÃO DE SALVAMENTO...");
        algoritmo.ExecutarMissao();

        Console.WriteLine("\n📊 RESULTADOS DA MISSÃO:");
        log.ExibirResumo();
        log.ExibirCaminhoPercorrido();

        log.SalvarArquivos();

        Console.WriteLine("\n✅ MISSÃO CONCLUÍDA COM SUCESSO!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ ERRO AO PROCESSAR ARQUIVO: {ex.Message}");
        throw;
    }
}
