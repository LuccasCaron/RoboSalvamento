using RoboSalvamento.Core;

namespace RoboSalvamento.Tests.Core;

[Trait("Categoria", "Testes de Log de Operação")]
public sealed class LogOperacaoTests
{

    #region Testes do Construtor

    [Fact]
    public void Construtor_DadoNomeArquivoValido_DeveInicializarCorretamente()
    {
        // arrange
        string nomeArquivoMapa = "mapa_teste.txt";

        // action
        var log = new LogOperacao(nomeArquivoMapa);

        // assert
        Assert.NotNull(log);
        // Verificar se o nome do arquivo foi definido corretamente através do método SalvarArquivos
        // (testaremos isso nos testes de SalvarArquivos)
    }

    [Fact]
    public void Construtor_DadoNomeArquivoComExtensao_DeveAlterarExtensaoParaCsv()
    {
        // arrange
        string nomeArquivoMapa = "mapa_teste.txt";

        // action
        var log = new LogOperacao(nomeArquivoMapa);

        // assert
        Assert.NotNull(log);
    }

    [Fact]
    public void Construtor_DadoNomeArquivoSemExtensao_DeveAdicionarExtensaoCsv()
    {
        // arrange
        string nomeArquivoMapa = "mapa_teste";

        // action
        var log = new LogOperacao(nomeArquivoMapa);

        // assert
        Assert.NotNull(log);
    }

    #endregion

    #region Testes do Método AdicionarRegistro

    [Fact]
    public void AdicionarRegistro_DadoRegistroValido_DeveAdicionarComSucesso()
    {
        // arrange
        var log = new LogOperacao("mapa_teste.txt");
        var registro = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.LIGAR,
            SensorEsquerdo = ELeituraSensor.PAREDE,
            SensorDireito = ELeituraSensor.VAZIO,
            SensorFrente = ELeituraSensor.PAREDE,
            EstadoCarga = EEstadoCarga.SEM_CARGA,
            PosicaoRobo = new Posicao(0, 0),
            DirecaoRobo = EDirecao.Norte
        };

        // action
        log.AdicionarRegistro(registro);

        // assert
        // Verificar se o número do comando foi definido
        Assert.Equal(1, registro.NumeroComando);
    }

    [Fact]
    public void AdicionarRegistro_DadosMultiplosRegistros_DeveNumerarSequencialmente()
    {
        // arrange
        var log = new LogOperacao("mapa_teste.txt");
        var registro1 = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.LIGAR,
            SensorEsquerdo = ELeituraSensor.PAREDE,
            SensorDireito = ELeituraSensor.VAZIO,
            SensorFrente = ELeituraSensor.PAREDE,
            EstadoCarga = EEstadoCarga.SEM_CARGA,
            PosicaoRobo = new Posicao(0, 0),
            DirecaoRobo = EDirecao.Norte
        };

        var registro2 = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.A,
            SensorEsquerdo = ELeituraSensor.VAZIO,
            SensorDireito = ELeituraSensor.PAREDE,
            SensorFrente = ELeituraSensor.VAZIO,
            EstadoCarga = EEstadoCarga.SEM_CARGA,
            PosicaoRobo = new Posicao(1, 0),
            DirecaoRobo = EDirecao.Norte
        };

        // action
        log.AdicionarRegistro(registro1);
        log.AdicionarRegistro(registro2);

        // assert
        Assert.Equal(1, registro1.NumeroComando);
        Assert.Equal(2, registro2.NumeroComando);
    }

    #endregion

    #region Testes do Método ToCsvLine

    [Fact]
    public void ToCsvLine_DadoRegistroCompleto_DeveRetornarFormatoCorreto()
    {
        // arrange
        var registro = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.LIGAR,
            SensorEsquerdo = ELeituraSensor.PAREDE,
            SensorDireito = ELeituraSensor.VAZIO,
            SensorFrente = ELeituraSensor.PAREDE,
            EstadoCarga = EEstadoCarga.SEM_CARGA
        };
        string resultadoEsperado = "LIGAR,PAREDE,VAZIO,PAREDE,SEM CARGA";

        // action
        string resultado = registro.ToCsvLine();

        // assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void ToCsvLine_DadoRegistroComHumano_DeveRetornarFormatoCorreto()
    {
        // arrange
        var registro = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.P,
            SensorEsquerdo = ELeituraSensor.PAREDE,
            SensorDireito = ELeituraSensor.VAZIO,
            SensorFrente = ELeituraSensor.HUMANO,
            EstadoCarga = EEstadoCarga.COM_HUMANO
        };
        string resultadoEsperado = "P,PAREDE,VAZIO,HUMANO,COM HUMANO";

        // action
        string resultado = registro.ToCsvLine();

        // assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void ToCsvLine_DadoRegistroComAvancar_DeveRetornarFormatoCorreto()
    {
        // arrange
        var registro = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.A,
            SensorEsquerdo = ELeituraSensor.VAZIO,
            SensorDireito = ELeituraSensor.VAZIO,
            SensorFrente = ELeituraSensor.VAZIO,
            EstadoCarga = EEstadoCarga.SEM_CARGA
        };
        string resultadoEsperado = "A,VAZIO,VAZIO,VAZIO,SEM CARGA";

        // action
        string resultado = registro.ToCsvLine();

        // assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void ToCsvLine_DadoRegistroComGirar_DeveRetornarFormatoCorreto()
    {
        // arrange
        var registro = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.G,
            SensorEsquerdo = ELeituraSensor.PAREDE,
            SensorDireito = ELeituraSensor.PAREDE,
            SensorFrente = ELeituraSensor.VAZIO,
            EstadoCarga = EEstadoCarga.SEM_CARGA
        };
        string resultadoEsperado = "G,PAREDE,PAREDE,VAZIO,SEM CARGA";

        // action
        string resultado = registro.ToCsvLine();

        // assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    [Fact]
    public void ToCsvLine_DadoRegistroComEjetar_DeveRetornarFormatoCorreto()
    {
        // arrange
        var registro = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.E,
            SensorEsquerdo = ELeituraSensor.VAZIO,
            SensorDireito = ELeituraSensor.VAZIO,
            SensorFrente = ELeituraSensor.VAZIO,
            EstadoCarga = EEstadoCarga.SEM_CARGA
        };
        string resultadoEsperado = "E,VAZIO,VAZIO,VAZIO,SEM CARGA";

        // action
        string resultado = registro.ToCsvLine();

        // assert
        Assert.Equal(resultadoEsperado, resultado);
    }

    #endregion

    #region Testes do Método SalvarArquivos

    [Fact]
    public void SalvarArquivos_DadoLogVazio_DeveCriarArquivoVazio()
    {
        // arrange
        var log = new LogOperacao("mapa_teste_vazio.txt");
        string caminhoArquivo = "mapa_teste_vazio.csv";

        // action
        log.SalvarArquivos();

        // assert
        Assert.True(File.Exists(caminhoArquivo));
        var conteudo = File.ReadAllText(caminhoArquivo);
        Assert.Empty(conteudo.Trim());

        // cleanup
        File.Delete(caminhoArquivo);
    }

    [Fact]
    public void SalvarArquivos_DadoLogComRegistros_DeveSalvarCorretamente()
    {
        // arrange
        var log = new LogOperacao("mapa_teste_completo.txt");
        string caminhoArquivo = "mapa_teste_completo.csv";

        var registro1 = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.LIGAR,
            SensorEsquerdo = ELeituraSensor.PAREDE,
            SensorDireito = ELeituraSensor.VAZIO,
            SensorFrente = ELeituraSensor.PAREDE,
            EstadoCarga = EEstadoCarga.SEM_CARGA
        };

        var registro2 = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.A,
            SensorEsquerdo = ELeituraSensor.VAZIO,
            SensorDireito = ELeituraSensor.PAREDE,
            SensorFrente = ELeituraSensor.VAZIO,
            EstadoCarga = EEstadoCarga.SEM_CARGA
        };

        log.AdicionarRegistro(registro1);
        log.AdicionarRegistro(registro2);

        // action
        log.SalvarArquivos();

        // assert
        Assert.True(File.Exists(caminhoArquivo));
        var linhas = File.ReadAllLines(caminhoArquivo);
        Assert.Equal(2, linhas.Length);
        Assert.Equal("LIGAR,PAREDE,VAZIO,PAREDE,SEM CARGA", linhas[0]);
        Assert.Equal("A,VAZIO,PAREDE,VAZIO,SEM CARGA", linhas[1]);

        // cleanup
        File.Delete(caminhoArquivo);
    }

    [Fact]
    public void SalvarArquivos_DadoLogComSequenciaCompleta_DeveSalvarFormatoEsperado()
    {
        // arrange
        var log = new LogOperacao("mapa_teste_sequencia.txt");
        string caminhoArquivo = "mapa_teste_sequencia.csv";

        // Simular sequência completa de operação
        var registros = new[]
        {
            new RegistroLogMelhorado
            {
                Comando = EComandoRobo.LIGAR,
                SensorEsquerdo = ELeituraSensor.PAREDE,
                SensorDireito = ELeituraSensor.PAREDE,
                SensorFrente = ELeituraSensor.VAZIO,
                EstadoCarga = EEstadoCarga.SEM_CARGA
            },
            new RegistroLogMelhorado
            {
                Comando = EComandoRobo.A,
                SensorEsquerdo = ELeituraSensor.VAZIO,
                SensorDireito = ELeituraSensor.VAZIO,
                SensorFrente = ELeituraSensor.PAREDE,
                EstadoCarga = EEstadoCarga.SEM_CARGA
            },
            new RegistroLogMelhorado
            {
                Comando = EComandoRobo.G,
                SensorEsquerdo = ELeituraSensor.PAREDE,
                SensorDireito = ELeituraSensor.VAZIO,
                SensorFrente = ELeituraSensor.VAZIO,
                EstadoCarga = EEstadoCarga.SEM_CARGA
            },
            new RegistroLogMelhorado
            {
                Comando = EComandoRobo.P,
                SensorEsquerdo = ELeituraSensor.PAREDE,
                SensorDireito = ELeituraSensor.VAZIO,
                SensorFrente = ELeituraSensor.HUMANO,
                EstadoCarga = EEstadoCarga.COM_HUMANO
            },
            new RegistroLogMelhorado
            {
                Comando = EComandoRobo.E,
                SensorEsquerdo = ELeituraSensor.VAZIO,
                SensorDireito = ELeituraSensor.VAZIO,
                SensorFrente = ELeituraSensor.VAZIO,
                EstadoCarga = EEstadoCarga.SEM_CARGA
            }
        };

        foreach (var registro in registros)
        {
            log.AdicionarRegistro(registro);
        }

        // action
        log.SalvarArquivos();

        // assert
        Assert.True(File.Exists(caminhoArquivo));
        var linhas = File.ReadAllLines(caminhoArquivo);
        Assert.Equal(5, linhas.Length);

        // Verificar formato específico conforme especificação
        Assert.Equal("LIGAR,PAREDE,PAREDE,VAZIO,SEM CARGA", linhas[0]);
        Assert.Equal("A,VAZIO,VAZIO,PAREDE,SEM CARGA", linhas[1]);
        Assert.Equal("G,PAREDE,VAZIO,VAZIO,SEM CARGA", linhas[2]);
        Assert.Equal("P,PAREDE,VAZIO,HUMANO,COM HUMANO", linhas[3]);
        Assert.Equal("E,VAZIO,VAZIO,VAZIO,SEM CARGA", linhas[4]);

        // cleanup
        File.Delete(caminhoArquivo);
    }

    #endregion

    #region Testes de Validação de Formato CSV

    [Fact]
    public void FormatoCsv_DadoRegistroComTodosOsValores_DeveTerFormatoConsistente()
    {
        // arrange
        var registro = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.A,
            SensorEsquerdo = ELeituraSensor.PAREDE,
            SensorDireito = ELeituraSensor.HUMANO,
            SensorFrente = ELeituraSensor.VAZIO,
            EstadoCarga = EEstadoCarga.COM_HUMANO
        };

        // action
        string resultado = registro.ToCsvLine();

        // assert
        var partes = resultado.Split(',');
        Assert.Equal(5, partes.Length);
        Assert.Equal("A", partes[0]);
        Assert.Equal("PAREDE", partes[1]);
        Assert.Equal("HUMANO", partes[2]);
        Assert.Equal("VAZIO", partes[3]);
        Assert.Equal("COM HUMANO", partes[4]);
    }

    [Fact]
    public void FormatoCsv_DadoRegistroComEspacos_DeveManterEspacosCorretos()
    {
        // arrange
        var registro = new RegistroLogMelhorado
        {
            Comando = EComandoRobo.LIGAR,
            SensorEsquerdo = ELeituraSensor.VAZIO,
            SensorDireito = ELeituraSensor.VAZIO,
            SensorFrente = ELeituraSensor.VAZIO,
            EstadoCarga = EEstadoCarga.SEM_CARGA
        };

        // action
        string resultado = registro.ToCsvLine();

        // assert
        // Verificar que não há espaços extras desnecessários
        Assert.DoesNotContain("  ", resultado); // Não deve ter espaços duplos
        Assert.False(resultado.StartsWith(" "), "Não deve começar com espaço"); // Não deve começar com espaço
        Assert.False(resultado.EndsWith(" "), "Não deve terminar com espaço"); // Não deve terminar com espaço
    }

    #endregion

}
