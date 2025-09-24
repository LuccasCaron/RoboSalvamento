using RoboSalvamento;
using RoboSalvamento.Core;
using RoboSalvamento.Robo;
using RoboSalvamento.Simulador;

if (args.Length == 0)
{
    return;
}

var arquivoMapa = args[0];

if (!File.Exists(arquivoMapa))
{
    return;
}

try
{
    var mapa = new Mapa(arquivoMapa);
    var simulador = new SimuladorAmbienteVirtual(mapa);
    var log = new LogOperacao(arquivoMapa);
    var algoritmo = new AlgoritmoBFS(simulador, log);

    algoritmo.ExecutarMissao();
    log.SalvarArquivos();
    
    Console.WriteLine("✅ MISSÃO CONCLUÍDA COM SUCESSO!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ ERRO: {ex.Message}");
}
