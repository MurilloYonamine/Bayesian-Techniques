namespace Bayesian_Techniques.Estados {
    public class Batalha : IEstado {
        private ModoBatalha _modoAtual;

        public string Nome { get; private set; }
        public List<Turno> Turnos { get; set; }

        int Delay { get; set; } = 100;

        public struct Turno {
            public int Numero { get; set; }
            public string Jogador { get; set; }
            public Acao AcaoJogador { get; set; }
            public string Inimigo { get; set; }
            public Acao AcaoInimigo { get; set; }
            public int VidaJogador { get; set; }
            public int VidaInimigo { get; set; }
        }

        public struct ResultadoTurno {
            public Acao AcaoJogador { get; set; }
            public Acao AcaoInimigo { get; set; }
            public int VidaJogadorAntes { get; set; }
            public int VidaInimigoAntes { get; set; }
        }

        public enum ModoBatalha {
            None = 0,
            Normal,
            Automatico
        }

        public Batalha() {
            Nome = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            Turnos = new List<Turno>();
        }

        public async Task Executar(GerenciadorEstados manager) {
            var jogador = manager.JogadorAtual ?? new Jogador();
            _modoAtual = EscolherModo(manager);

            if (_modoAtual == ModoBatalha.Automatico) {
                Console.Write("Quantas batalhas automáticas deseja executar? (digite 0 para infinito): ");
                if (!int.TryParse(Console.ReadLine(), out int repeticoes) || repeticoes < 0) repeticoes = 0;
                await Executar(jogador, manager, _modoAtual, repeticoes);
                return;
            }

            await Executar(jogador, manager, _modoAtual);
        }

        public async Task Executar(Jogador jogador, GerenciadorEstados manager, ModoBatalha modo) {
            await Executar(jogador, manager, modo, -1);
        }

        public async Task Executar(Jogador jogador, GerenciadorEstados manager, ModoBatalha modo, int repeticoes = -1) {
            Console.Clear();

            if (modo == ModoBatalha.Automatico) {
                await LoopAutomatizado(jogador, manager, repeticoes == -1 ? 0 : repeticoes);
                return;
            }

            var inimigo = CriarInimigo(jogador);

            Preparar(jogador, inimigo);
            await IniciarNarrativa(jogador, inimigo);

            await LoopNormal(jogador, inimigo);

            ExibirResultado(jogador, inimigo);
            Finalizar(jogador, inimigo);

            await MenuFinal(jogador, manager);
        }

        private ModoBatalha EscolherModo(GerenciadorEstados manager) {
            Console.WriteLine("Escolha o modo de batalha:");

            var menu = new Menu(new string[] {
                "Normal",
                "Automático",
                "Sair"
            });

            menu.ShowMenu(out int escolha);

            switch(escolha) {
                case 0: return ModoBatalha.Normal;
                case 1: return ModoBatalha.Automatico;
                case 2:
                    manager?.MudarEstado(null);
                    return ModoBatalha.None;
                default:
                    Console.WriteLine("Escolha inválida, iniciando modo Normal...");
                    return ModoBatalha.Normal;
            }

        }
        private async Task LoopAutomatizado(Jogador jogador, GerenciadorEstados manager, int repeticoes) {
            int execucao = 0;

            // repeticoes == 0 => infinito
            while (repeticoes == 0 || execucao < repeticoes) {
                execucao++;

                // preparar nova simulação
                Nome = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                Turnos = new List<Turno>();

                var inimigo = CriarInimigo(jogador);
                Preparar(jogador, inimigo);
                await IniciarNarrativa(jogador, inimigo);

                int turno = 1;
                Acao ultima = Acao.None;

                while (jogador.VidaAtual > 0 && inimigo.VidaAtual > 0) {
                    Console.WriteLine($"========= Turno {turno} =========");

                    var previsao = await Prever(inimigo, jogador, ultima, turno);
                    var resolucao = await ResolverTurno(jogador, inimigo, previsao.acao, previsao.confianca);

                    AtualizarStats(inimigo, jogador, resolucao, previsao.acao);
                    MostrarStatus(jogador, inimigo);
                    RegistrarTurno(turno, jogador, inimigo, resolucao);

                    ultima = resolucao.AcaoJogador;
                    turno++;

                    await Task.Delay(Delay);
                }

                ExibirResultado(jogador, inimigo);
                Finalizar(jogador, inimigo);

                Console.WriteLine($"[SIMULAÇÃO {execucao} CONCLUÍDA]");
                Console.WriteLine();

                if (repeticoes != 0 && execucao >= repeticoes) break;
            }

            Console.WriteLine("Todas as simulações solicitadas foram concluídas.\n");
            await MenuFinal(jogador, manager);
        }

        private async Task LoopNormal(Jogador jogador, Inimigo inimigo) {
            int turno = 1;
            Acao ultima = Acao.None;

            while (jogador.VidaAtual > 0 && inimigo.VidaAtual > 0) {
                Console.WriteLine($"========= Turno {turno} =========");

                var previsao = await Prever(inimigo, jogador, ultima, turno);
                var resolucao = await ResolverTurno(jogador, inimigo, previsao.acao, previsao.confianca);

                AtualizarStats(inimigo, jogador, resolucao, previsao.acao);
                MostrarStatus(jogador, inimigo);
                RegistrarTurno(turno, jogador, inimigo, resolucao);

                ultima = resolucao.AcaoJogador;
                turno++;

                await Task.Delay(Delay);
            }
        }

        private async Task ReiniciarAutomatico(Jogador jogador, GerenciadorEstados manager) {
            Console.Clear();
            Console.WriteLine("Nova batalha automática...");

            await Task.Delay(Delay);

            await new Batalha().Executar(jogador, manager, ModoBatalha.Automatico);
        }

        private Inimigo CriarInimigo(Jogador jogador) {
            int vida = jogador.VidaMaxima - jogador.VidaMaxima / 4;
            return new Inimigo(jogador) {
                VidaMaxima = vida,
                VidaAtual = vida
            };
        }

        private void Preparar(Jogador jogador, Inimigo inimigo) {
            jogador.PrepararParaBatalha();
            inimigo.PrepararParaBatalha();
        }

        private async Task IniciarNarrativa(Jogador jogador, Inimigo inimigo) {
            Console.WriteLine($"Batalha entre {jogador.Nome} e {inimigo.Nome}!");

            await Task.Delay(Delay);
            Console.Clear();
        }

        private async Task<(Acao acao, float confianca)> Prever(Inimigo inimigo, Jogador jogador, Acao anterior, int turno) {
            Console.WriteLine($"{inimigo.Nome} analisando...");

            var (acao, conf) = inimigo.PreverAcao(jogador, anterior, turno);

            if (acao == Acao.None) {
                Console.WriteLine($"{inimigo.Nome} não tem dados suficientes para prever, escolhendo aleatoriamente...");
                await Task.Delay(Delay);
            } else {
                Console.WriteLine($"{inimigo.Nome} prevê {acao}");
            }

            await Task.Delay(Delay);
            Console.WriteLine();

            return acao == Acao.None ? (Decisao.Iniciar(inimigo), 0f) : (acao, conf);
        }

        private async Task<ResultadoTurno> ResolverTurno(Jogador jogador, Inimigo inimigo, Acao prevista, float conf) {
            var acaoJogador = Decisao.Iniciar(jogador);
            var acaoInimigo = Decisao.Iniciar(inimigo.EscolherRespostaParaPrevisao(prevista, conf, inimigo.EstaFlanqueando));

            int vidaJogador = jogador.VidaAtual;
            int vidaInimigo = inimigo.VidaAtual;

            Combate.ResolverTurno(jogador, acaoJogador, inimigo, acaoInimigo);

            await Task.Delay(Delay);

            return new ResultadoTurno {
                AcaoJogador = acaoJogador,
                AcaoInimigo = acaoInimigo,
                VidaJogadorAntes = vidaJogador,
                VidaInimigoAntes = vidaInimigo
            };
        }

        private void MostrarStatus(Jogador j, Inimigo i) {
            Console.WriteLine();
            Console.WriteLine($"{j.Nome}: {j.VidaAtual}/{j.VidaMaxima}");
            Console.WriteLine($"{i.Nome}: {i.VidaAtual}/{i.VidaMaxima}");
            Console.WriteLine();
        }

        private void RegistrarTurno(int num, Jogador jogador, Inimigo inimigo, ResultadoTurno resultado) {
            Turnos.Add(new Turno {
                Numero = num,
                Jogador = jogador.Nome,
                AcaoJogador = resultado.AcaoJogador,
                Inimigo = inimigo.Nome,
                AcaoInimigo = resultado.AcaoInimigo,
                VidaJogador = jogador.VidaAtual,
                VidaInimigo = inimigo.VidaAtual
            });
        }

        private void AtualizarStats(Inimigo inimigo, Jogador jogador, ResultadoTurno resultado, Acao prevista) {
            var perf = inimigo.Performance ??= new IAStatsBatalha();

            Incrementar(perf.AcoesIA, resultado.AcaoInimigo);
            Incrementar(perf.AcoesJogador, resultado.AcaoJogador);

            perf.Previsao.Total++;
            if (prevista == resultado.AcaoJogador)
                perf.Previsao.Acertos++;

            int danoIA = Math.Max(0, resultado.VidaJogadorAntes - jogador.VidaAtual);
            int danoJog = Math.Max(0, resultado.VidaInimigoAntes - inimigo.VidaAtual);

            perf.Dano.CausadoIA += danoIA;
            perf.Dano.RecebidoIA += danoJog;
            perf.Dano.CausadoJogador += danoJog;
            perf.Dano.RecebidoJogador += danoIA;
        }

        private void Incrementar(Dictionary<Acao, int> dict, Acao acao) {
            if (!dict.ContainsKey(acao)) dict[acao] = 0;

            dict[acao]++;
        }

        private void ExibirResultado(Jogador j, Inimigo i) {
            Console.WriteLine(
                j.VidaAtual > 0
                ? $"{j.Nome} venceu"
                : $"{i.Nome} venceu"
            );
        }

        private void Finalizar(Jogador jogador, Inimigo inimigo) {
            var perf = inimigo.Performance ?? new IAStatsBatalha();

            perf.BatalhaId = Nome;
            perf.Resultado = jogador.VidaAtual > 0 ? "Derrota" : "Vitória";

            perf.Metricas.PrecisaoPrevisao =
                perf.Previsao.Total > 0
                ? (double)perf.Previsao.Acertos / perf.Previsao.Total
                : 0;

            BancoDeDados.SalvarBatalhas(this, jogador);
            BancoDeDados.SalvarIAStats(perf, jogador);
        }

        private async Task MenuFinal(Jogador j, GerenciadorEstados manager) {
            Console.WriteLine("Jogar novamente?");

            var menu = new Menu(new string[] {
                "Sim",
                "Não",
                "Trocar Jogador"
            });

            menu.ShowMenu(out int escolha);

            switch (escolha) {
                case 0:
                    await new Batalha().Executar(j, manager, _modoAtual);
                    break;
                case 1:
                    manager?.MudarEstado(null);
                    break;
                case 2:
                    manager?.MudarEstado(new Lobby());
                    break;
            }
        }
    }
}