using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RoboVisualizador
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Uso: dotnet run <arquivo_mapa.txt>");
                Console.WriteLine("Exemplo: dotnet run \"../RoboSalvamento.Tests/TestData/3x5.txt\"");
                return;
            }

            var arquivoMapa = args[0];

            if (!File.Exists(arquivoMapa))
            {
                Console.WriteLine("Arquivo de mapa nao encontrado!");
                return;
            }

            try
            {
                // Executar o RoboSalvamento para gerar o CSV
                Console.WriteLine("🤖 Executando RoboSalvamento...");
                var arquivoCsv = ExecutarRoboSalvamento(arquivoMapa);
                
                if (arquivoCsv == null)
                {
                    Console.WriteLine("❌ Falha ao executar RoboSalvamento!");
                    return;
                }

                Console.WriteLine("✅ CSV gerado com sucesso!");
                Console.WriteLine("🎬 Iniciando animação...");
                Thread.Sleep(1000);

                // Executar a animação
                var visualizador = new VisualizadorRobo(arquivoMapa, arquivoCsv);
                visualizador.ExecutarAnimacao();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
        }

        private static string? ExecutarRoboSalvamento(string arquivoMapa)
        {
            try
            {
                // Caminho para o executável do RoboSalvamento
                var roboSalvamentoPath = Path.Combine("..", "RoboSalvamento", "bin", "Debug", "net8.0", "RoboSalvamento.exe");
                
                if (!File.Exists(roboSalvamentoPath))
                {
                    Console.WriteLine($"❌ Executável do RoboSalvamento não encontrado em: {roboSalvamentoPath}");
                    return null;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = roboSalvamentoPath,
                    Arguments = $"\"{arquivoMapa}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                {
                    Console.WriteLine("❌ Falha ao iniciar o processo RoboSalvamento");
                    return null;
                }

                // Aguardar a conclusão
                process.WaitForExit();

                // Ler saída para debug
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"❌ RoboSalvamento falhou com código {process.ExitCode}");
                    if (!string.IsNullOrEmpty(error))
                        Console.WriteLine($"Erro: {error}");
                    return null;
                }

                // O CSV é gerado no mesmo diretório do arquivo de mapa
                var arquivoCsv = Path.ChangeExtension(arquivoMapa, ".csv");
                
                if (!File.Exists(arquivoCsv))
                {
                    Console.WriteLine($"❌ Arquivo CSV não foi gerado: {arquivoCsv}");
                    return null;
                }

                return arquivoCsv;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao executar RoboSalvamento: {ex.Message}");
                return null;
            }
        }
    }

    public class VisualizadorRobo
    {
        private char[,] _mapa;
        private int _linhas, _colunas;
        private List<RegistroLog> _log;
        private Posicao _entrada, _humano;

        public VisualizadorRobo(string arquivoMapa, string arquivoLog)
        {
            CarregarMapa(arquivoMapa);
            CarregarLog(arquivoLog);
        }

        private void CarregarMapa(string arquivo)
        {
            var linhas = File.ReadAllLines(arquivo);
            _linhas = linhas.Length;
            _colunas = linhas[0].Length;
            _mapa = new char[_linhas, _colunas];

            for (int i = 0; i < _linhas; i++)
            {
                for (int j = 0; j < _colunas; j++)
                {
                    _mapa[i, j] = linhas[i][j];
                    if (_mapa[i, j] == 'E') _entrada = new Posicao(i, j);
                    if (_mapa[i, j] == '@') _humano = new Posicao(i, j);
                }
            }
        }

        private void CarregarLog(string arquivo)
        {
            _log = new List<RegistroLog>();
            var linhas = File.ReadAllLines(arquivo);

            foreach (var linha in linhas)
            {
                var partes = linha.Split(',');
                if (partes.Length >= 5)
                {
                    _log.Add(new RegistroLog
                    {
                        Comando = partes[0],
                        SensorEsquerdo = partes[1],
                        SensorDireito = partes[2],
                        SensorFrente = partes[3],
                        EstadoCarga = partes[4]
                    });
                }
            }
        }

        public void ExecutarAnimacao()
        {
            Console.Clear();
            Console.WriteLine("ANIMACAO DO ROBO DE SALVAMENTO");
            Console.WriteLine("================================");
            Console.WriteLine("Pressione qualquer tecla para iniciar...");
            Console.ReadKey();

            var posicaoRobo = _entrada;
            var direcaoRobo = DeterminarDirecaoInicial();

            for (int i = 0; i < _log.Count; i++)
            {
                var registro = _log[i];
                
                if (registro.Comando == "A")
                {
                    posicaoRobo = Avancar(posicaoRobo, direcaoRobo);
                }
                else if (registro.Comando == "G")
                {
                    direcaoRobo = Girar(direcaoRobo);
                }

                DesenharMapa(posicaoRobo, direcaoRobo, registro, i + 1);
                Thread.Sleep(300);
            }

            Console.WriteLine("\nMISSAO CONCLUIDA!");
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        private void DesenharMapa(Posicao posicaoRobo, EDirecao direcao, RegistroLog registro, int comandoNum)
        {
            Console.Clear();
            Console.WriteLine($"COMANDO {comandoNum}: {registro.Comando}");
            Console.WriteLine($"Posicao: ({posicaoRobo.Linha}, {posicaoRobo.Coluna}) | Direcao: {direcao}");
            Console.WriteLine($"Carga: {registro.EstadoCarga}");
            Console.WriteLine("================================");

            for (int i = 0; i < _linhas; i++)
            {
                for (int j = 0; j < _colunas; j++)
                {
                    if (i == posicaoRobo.Linha && j == posicaoRobo.Coluna)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(ObterSimboloRobo(direcao));
                    }
                    else
                    {
                        Console.ForegroundColor = ObterCorCaractere(_mapa[i, j]);
                        Console.Write(ObterSimboloMapa(_mapa[i, j]));
                    }
                }
                Console.WriteLine();
            }

            Console.ResetColor();
            Console.WriteLine("================================");
            Console.WriteLine($"Sensores: E:{registro.SensorEsquerdo} | F:{registro.SensorFrente} | D:{registro.SensorDireito}");
        }

        private char ObterSimboloRobo(EDirecao direcao)
        {
            return direcao switch
            {
                EDirecao.Norte => '^',
                EDirecao.Leste => '>',
                EDirecao.Sul => 'v',
                EDirecao.Oeste => '<',
                _ => 'O'
            };
        }

        private char ObterSimboloMapa(char c)
        {
            return c switch
            {
                'X' => '#',
                '.' => '.',
                'E' => 'E',
                '@' => '@',
                _ => c
            };
        }

        private ConsoleColor ObterCorCaractere(char c)
        {
            return c switch
            {
                'X' => ConsoleColor.Red,
                '.' => ConsoleColor.White,
                'E' => ConsoleColor.Green,
                '@' => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };
        }

        private Posicao Avancar(Posicao posicao, EDirecao direcao)
        {
            return direcao switch
            {
                EDirecao.Norte => new Posicao(posicao.Linha - 1, posicao.Coluna),
                EDirecao.Leste => new Posicao(posicao.Linha, posicao.Coluna + 1),
                EDirecao.Sul => new Posicao(posicao.Linha + 1, posicao.Coluna),
                EDirecao.Oeste => new Posicao(posicao.Linha, posicao.Coluna - 1),
                _ => posicao
            };
        }

        private EDirecao Girar(EDirecao direcao)
        {
            return (EDirecao)(((int)direcao + 1) % 4);
        }

        private EDirecao DeterminarDirecaoInicial()
        {
            if (_entrada.Linha == 0) return EDirecao.Sul;
            if (_entrada.Linha == _linhas - 1) return EDirecao.Norte;
            if (_entrada.Coluna == 0) return EDirecao.Leste;
            return EDirecao.Oeste;
        }
    }

    public class Posicao
    {
        public int Linha { get; set; }
        public int Coluna { get; set; }

        public Posicao(int linha, int coluna)
        {
            Linha = linha;
            Coluna = coluna;
        }
    }

    public enum EDirecao
    {
        Norte = 0,
        Leste = 1,
        Sul = 2,
        Oeste = 3
    }

    public class RegistroLog
    {
        public string Comando { get; set; } = "";
        public string SensorEsquerdo { get; set; } = "";
        public string SensorDireito { get; set; } = "";
        public string SensorFrente { get; set; } = "";
        public string EstadoCarga { get; set; } = "";
    }
}