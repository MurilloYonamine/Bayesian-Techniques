namespace Bayesian_Techniques.Estados {
    public class MenuCriacao : IEstado {
        public async Task Executar(GerenciadorEstados manager) {
            if (manager.JogadorAtual == null) {
                manager.JogadorAtual = new Jogador();
                manager.JogadorAtual.Carregando = false; // Garantir que jogador criado localmente possa ser salvo
            }
            var jogador = manager.JogadorAtual;

            while (true) {
                Console.WriteLine("O que você quer configurar?");
                string[] opcoes = new string[] {
                    "Definir Nome",
                    "Definir Vida",
                    "Definir Ataque",
                    "Definir Defesa",
                    "Definir Esquiva",
                    "Definir Contra-tacar",
                    "Definir Movimentar",
                    "Randomizar atributos",
                    "Olhar atributos atuais",
                    "Iniciar batalha",
                    "Voltar"
                };

                var menu = new Menu(opcoes);
                menu.ShowMenu(out int escolha);

                switch (escolha) {
                    case 0: LidarNome(jogador); break;
                    case 1: LidarVida(jogador); break;
                    case 2: LidarAtaque(jogador); break;
                    case 3: LidarDefesa(jogador); break;
                    case 4: LidarEsquiva(jogador); break;
                    case 5: LidarContraAtaque(jogador); break;
                    case 6: LidarMovimentacao(jogador); break;
                    case 7: RandomizarAtributos(jogador); break;
                    case 8: OlharAtributos(jogador); break;
                    case 9:
                        jogador.Salvar();
                        PythonStatsLauncher.Abrir(manager.JogadorAtual.Nome);
                        manager.MudarEstado(new Batalha());
                        return;
                    case 10:
                        PythonStatsLauncher.Fechar();
                        manager.MudarEstado(new Lobby());
                        return;
                    default: Console.WriteLine("Opção inválida."); break;
                }
            }
        }

        #region Configurações de atributos do jogador
        private void LidarNome(Jogador jogador) {
            Console.Clear();

            if (jogador.Nome == null) {
                Console.Write("Digite o nome do jogador: ");
                jogador.Nome = Console.ReadLine() ?? "Jogador";
                Console.Clear();
                return;
            }

            Console.WriteLine($"O nome atual é: {jogador.Nome}");
            Console.WriteLine("Deseja alterar o nome? ");

            var menu = new Menu(new string[] { "Sim", "Não" });
            menu.ShowMenu(out int escolha);

            switch (escolha) {
                case 0:
                    Console.Write("Digite o novo nome do jogador: ");
                    jogador.Nome = Console.ReadLine() ?? jogador.Nome;
                    Console.Clear();
                    Console.WriteLine($"Nome atualizado para: {jogador.Nome}");
                    break;
                case 1: Console.WriteLine("Mantendo o nome atual."); break;
            }
            Console.WriteLine();
        }

        private void LidarVida(Jogador jogador) {
            ConfigurarAtributo(
                "Vida",
                () => jogador.VidaMaxima,
                valor => jogador.VidaMaxima = valor,
                jogador,
                min: 1,
                max: 100
            );
            jogador.VidaAtual = jogador.VidaMaxima;
        }

        private void LidarAtaque(Jogador jogador) {
            ConfigurarAtributo(
                "Ataque",
                () => jogador.StatusAtaque,
                valor => jogador.StatusAtaque = valor,
                jogador
            );
        }

        private void LidarDefesa(Jogador jogador) {
            ConfigurarAtributo(
                "Defesa",
                () => jogador.StatusDefesa,
                valor => jogador.StatusDefesa = valor,
                jogador
            );
        }

        private void LidarEsquiva(Jogador jogador) {
            ConfigurarAtributo(
                "Esquiva",
                () => jogador.StatusEsquiva,
                valor => jogador.StatusEsquiva = valor,
                jogador
            );
        }

        private void LidarContraAtaque(Jogador jogador) {
            ConfigurarAtributo(
                "Contra-ataque",
                () => jogador.StatusContraAtaque,
                valor => jogador.StatusContraAtaque = valor,
                jogador
            );
        }

        private void LidarMovimentacao(Jogador jogador) {
            ConfigurarAtributo(
                "Movimentação",
                () => jogador.StatusMovimentacao,
                valor => jogador.StatusMovimentacao = valor,
                jogador
            );
        }

        private void ConfigurarAtributo(string nomeAtributo, Func<int> getter, Action<int> setter, Jogador jogador, int min = 0, int max = 10) {
            Console.Clear();

            int valorAtual = getter();

            if (valorAtual == 0) {
                Console.Write($"Digite o valor de {nomeAtributo} do jogador ({min}-{max}): ");

                if (int.TryParse(Console.ReadLine(), out int valor)) setter(valor);
                Console.Clear();
                return;
            }

            Console.WriteLine($"O {nomeAtributo} atual é: {valorAtual}");
            Console.WriteLine("Deseja alterar o valor? ");

            var menu = new Menu(new string[] { "Sim", "Não" });
            menu.ShowMenu(out int escolha);

            switch (escolha) {
                case 0:
                    Console.Write($"Digite o novo valor de {nomeAtributo} do jogador ({min}-{max}): ");
                    if (int.TryParse(Console.ReadLine(), out int novoValor)) {
                        setter(novoValor);
                        Console.Clear();
                        Console.WriteLine($"{nomeAtributo} atualizado para: {novoValor}");
                    }
                    break;
                case 1: Console.WriteLine($"Mantendo o {nomeAtributo} atual."); break;
                default: Console.WriteLine("Opção inválida. Mantendo o valor atual."); break;
            }

            Console.WriteLine();
        }

        private void RandomizarAtributos(Jogador jogador) {
            var random = new Random();
            jogador.VidaMaxima = random.Next(1, 101);
            jogador.StatusAtaque = random.Next(0, 11);
            jogador.StatusDefesa = random.Next(0, 11);
            jogador.StatusEsquiva = random.Next(0, 11);
            jogador.StatusContraAtaque = random.Next(0, 11);
            jogador.StatusMovimentacao = random.Next(0, 11);

            Console.WriteLine("Atributos do jogador foram randomizados!\n");
            OlharAtributos(jogador);
        }

        private void OlharAtributos(Jogador jogador) {
            Console.Clear();

            Console.WriteLine($"Atributos atuais de {jogador.Nome ?? "jogador"}:");
            Console.WriteLine($"Vida: {jogador.VidaMaxima}");
            Console.WriteLine($"Ataque: {jogador.StatusAtaque}");
            Console.WriteLine($"Defesa: {jogador.StatusDefesa}");
            Console.WriteLine($"Esquiva: {jogador.StatusEsquiva}");
            Console.WriteLine($"Contra-ataque: {jogador.StatusContraAtaque}");
            Console.WriteLine($"Movimentação: {jogador.StatusMovimentacao}\n");
        }
        #endregion
    }
}
