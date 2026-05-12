using System.Diagnostics;

namespace Bayesian_Techniques {
	public static class PythonStatsLauncher {
		private static Process? _process;
		private static bool _handlersRegistrados = false;
		private static string? _nomeJogadorAtual;

		private static void RegistrarHandlers() {
			if (_handlersRegistrados) return;

			try {
				AppDomain.CurrentDomain.ProcessExit += (s, e) => {
					Fechar();
				};
				_handlersRegistrados = true;
			} catch {
				// Se falhar ao registrar handlers, continua normalmente
			}
		}

		public static void Abrir(string nomeJogador) {
			RegistrarHandlers();
			Fechar();

			_nomeJogadorAtual = nomeJogador;
			string projetoRaiz = BancoDeDados.RaizProjeto;
			string script = Path.Combine(projetoRaiz, "main.py");

			if (!TemPythonInstalado()) {
				Console.WriteLine("Python nao encontrado. Continuando sem abrir as estatisticas em tempo real.");
				return;
			}

			Console.Write("Abrindo estatisticas");
			for (int i = 0; i < 6; i++) {
				Console.Write(".");
				Thread.Sleep(250);
			}
			Console.WriteLine();

            var startInfo = new ProcessStartInfo {
                FileName = "py",
                Arguments = $"\"{script}\" \"{nomeJogador}\"",
                WorkingDirectory = projetoRaiz,
                UseShellExecute = false, 
                CreateNoWindow = true  
            };

            _process = Process.Start(startInfo);
		}

		private static bool TemPythonInstalado() {
			return ExecutarTesteComando("py", "--version") || ExecutarTesteComando("python", "--version");
		}

		private static bool ExecutarTesteComando(string arquivo, string argumentos) {
			try {
				var teste = new ProcessStartInfo {
					FileName = arquivo,
					Arguments = argumentos,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

				using var processo = Process.Start(teste);
				if (processo == null) return false;

				if (!processo.WaitForExit(2000)) {
					return false;
				}

				return processo.ExitCode == 0;
			} catch {
				return false;
			}
		}

		public static void Fechar() {
			try {
				// Mata todos os processos Python/py em execução
				var processosPython = Process.GetProcessesByName("python");
				foreach (var p in processosPython) {
					try {
						p.Kill(true);
						p.Dispose();
					} catch { }
				}

				var processosPy = Process.GetProcessesByName("py");
				foreach (var p in processosPy) {
					try {
						p.Kill(true);
						p.Dispose();
					} catch { }
				}

				// Aguarda um pouco para os processos serem mortos
				Thread.Sleep(100);

				// Tenta matar a janela cmd também
				var processosCmd = Process.GetProcessesByName("cmd");
				foreach (var p in processosCmd) {
					try {
						// Verifica se é a janela da dashboard
						if (p.MainWindowTitle.Contains("IA Stats")) {
							p.Kill(true);
							p.Dispose();
						}
					} catch { }
				}

				// Tenta matar o processo rastreado
				if (_process != null && !_process.HasExited) {
					_process.Kill(true);
					_process.Dispose();
				}
			} catch {
				// Ignora falhas
			} finally {
				_process = null;
			}
		}
	}
}
