# 🤖 Robô de Salvamento

Sistema embarcado para robô de salvamento que busca e resgata humanos perdidos em labirintos usando algoritmo BFS.

## 🚀 Como Executar

```bash
# Clonar o repositório
git clone https://github.com/LuccasCaron/RoboSalvamento.git
cd RoboSalvamento/src/RoboSalvamento

# Executar o programa
dotnet run "caminho/para/arquivo.txt"
```

## 📋 Exemplo

```bash
dotnet run "mapas/10x20semerro.txt"
```

O programa gera automaticamente um arquivo CSV com o log da missão.

## ✅ Testes

```bash
cd ../RoboSalvamento.Tests
dotnet test
```

## 🎬 RoboVisualizador

O projeto **RoboVisualizador** é um projeto em .NET 9 que executa o projeto RoboSalvamento automaticamente e realiza uma animação no mapa de acordo com os logs gerados, permitindo visualizar exatamente o que o robô faz durante a missão. (É um extra que utilizamos para debug) o projeto principal esta no RoboSalvamento.

### Como usar o Visualizador:

```bash
cd src/RoboVisualizador
dotnet run "caminho/para/arquivo.txt"
```

### Funcionalidades do Visualizador:

- ✅ **Execução automática**: Executa o RoboSalvamento automaticamente para gerar o CSV
- ✅ **Animação em tempo real**: Mostra o robô se movendo pelo labirinto
- ✅ **Informações detalhadas**: Posição, direção e estado da carga
- ✅ **Sensores**: Exibe leituras dos sensores em tempo real
- ✅ **Interface colorida**: Cores diferentes para cada elemento do mapa
- ✅ **Símbolos direcionais**: Robô com símbolos direcionais (^ > v <)
