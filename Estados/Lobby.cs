namespace Bayesian_Techniques.Estados {
    public class Lobby : IEstado {
        public async Task Executar(GerenciadorEstados manager) {
            if (manager.JogadorAtual == null) manager.JogadorAtual = new Jogador();

            while (true) {
                Console.WriteLine("Bem vindo a simulação!");
                Console.WriteLine("Você deseja criar um novo jogador ou escolher um existente?\n");

                string[] opcoes = {
                    "Criar novo Jogador",
                    "Escolher Jogador existente",
                    "Sair"
                };

                var menu = new Menu(opcoes);
                menu.ShowMenu(out int escolha);

                switch (escolha) {
                    case 0:
                        manager.JogadorAtual = new Jogador();
                        manager.MudarEstado(new MenuCriacao());
                        return;
                    case 1:
                        EscolherJogadorExistente(manager);
                        if (manager.JogadorAtual?.Nome != null) {
                            PythonStatsLauncher.Abrir(manager.JogadorAtual.Nome);
                            manager.MudarEstado(new MenuCriacao());
                            return;
                        }
                        break;
                    case 2:
                        Sair();
                        break;
                }
            }
        }

        private void EscolherJogadorExistente(GerenciadorEstados manager) {
            var jogadores = BancoDeDados.ListarJogadores();

            if (jogadores.Count == 0) {
                Console.WriteLine("Nenhum jogador encontrado.");
                return;
            }

            Console.WriteLine("Jogadores disponíveis:");

            var menu = new Menu(jogadores.ToArray());
            menu.ShowMenu(out int escolha);

            bool escolhaValida = escolha >= 0 && escolha < jogadores.Count;

            if (escolhaValida) {
                string nomeSelecionado = jogadores[escolha];

                Jogador? carregado = BancoDeDados.CarregarJogador(nomeSelecionado);

                if (carregado != null) {
                    manager.JogadorAtual = carregado;
                } else {
                    Console.WriteLine("Erro ao carregar o jogador.");
                }
            }
        }

        private void Sair() {
            Console.WriteLine("Obrigado por jogar! Até a próxima!");
            PythonStatsLauncher.Fechar();
            Environment.Exit(0);
        }
    }
}
