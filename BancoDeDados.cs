using Bayesian_Techniques.Estados;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bayesian_Techniques {
    public static class BancoDeDados {
        public static string RaizProjeto = Path.GetFullPath(
            Path.Combine(
                Directory.GetCurrentDirectory(),
                @"..\..\..\"
            )
        );

        // Centralized paths for data directories
        public static string BANCO_DE_DADOS = Path.Combine(RaizProjeto, "BancoDeDados");
        public static string IA_STATS = Path.Combine(BANCO_DE_DADOS, "IA stats");
        public static string BATALHAS = Path.Combine(BANCO_DE_DADOS, "batalhas");
        public static string JOGADORES = Path.Combine(BANCO_DE_DADOS, "jogadores");

        #region Métodos para IA Stats
        public static void SalvarIAStats(IAStatsBatalha stats, Jogador jogador) {

            string pasta = Path.Combine(
                IA_STATS,
                jogador.Nome
            );

            if (!Directory.Exists(pasta)) {
                Directory.CreateDirectory(pasta);
            }

            var options = new JsonSerializerOptions {
                WriteIndented = true,
                Converters = {
                    new JsonStringEnumConverter()
                }
            };

            string caminho = Path.Combine(
                pasta,
                $"{stats.BatalhaId}.json"
            );

            string json = JsonSerializer.Serialize(stats, options);

            File.WriteAllText(caminho, json);

            Console.WriteLine($"IA Stats da batalha '{stats.BatalhaId}' salvas com sucesso.\n");
        }

        public static List<string> ListarIAStats(Jogador jogador) {

            string pasta = Path.Combine(
                IA_STATS,
                jogador.Nome
            );

            if (!Directory.Exists(pasta)) {
                return new List<string>();
            }

            var arquivos = Directory.GetFiles(pasta, "*.json");

            var stats = new List<string>();

            foreach (var arquivo in arquivos) {
                stats.Add(Path.GetFileNameWithoutExtension(arquivo));
            }

            return stats;
        }

        public static List<IAStatsBatalha> CarregarIAStats(Jogador jogador) {

            string pasta = Path.Combine(
                IA_STATS,
                jogador.Nome
            );

            if (!Directory.Exists(pasta)) {
                return new List<IAStatsBatalha>();
            }

            var arquivos = Directory.GetFiles(pasta, "*.json");

            var lista = new List<IAStatsBatalha>();

            foreach (var arquivo in arquivos) {

                string json = File.ReadAllText(arquivo);

                var options = new JsonSerializerOptions {
                    Converters = {
                        new JsonStringEnumConverter()
                    }
                };

                var stats = JsonSerializer.Deserialize<IAStatsBatalha>(json, options);

                if (stats != null) lista.Add(stats);
            }

            return lista;
        }
        #endregion

        #region Métodos para batalhas
        public static void SalvarBatalhas(Batalha batalha, Jogador jogador) {
            string pasta = Path.Combine(
                        BATALHAS,
                        jogador.Nome
                );

            if (!Directory.Exists(pasta)) {
                Directory.CreateDirectory(pasta);
            }

            var options = new JsonSerializerOptions {
                WriteIndented = true,
                Converters = {
                    new JsonStringEnumConverter()
                }
            };

            string caminho = Path.Combine(
                pasta,
                $"{batalha.Nome}.json"
            );

            string json = JsonSerializer.Serialize(
                batalha,
                options
            );

            File.WriteAllText(caminho, json);

            Console.WriteLine($"Batalha '{batalha.Nome}' salva com sucesso.\n");
        }

        public static List<string> ListarBatalhas(Jogador jogador) {
            string pasta = Path.Combine(
                BATALHAS,
                jogador.Nome
            );

            if (!Directory.Exists(pasta)) {
                return new List<string>();
            }

            var arquivos = Directory.GetFiles(pasta, "*.json");
            var batalhas = new List<string>();

            foreach (var arquivo in arquivos) {
                batalhas.Add(Path.GetFileNameWithoutExtension(arquivo));
            }

            return batalhas;
        }

        public static List<Batalha> CarregarBatalhas(Jogador jogador) {
            string pasta = Path.Combine(
                BATALHAS,
                jogador.Nome
            );

            if (!Directory.Exists(pasta)) {
                return new List<Batalha>();
            }

            var arquivos = Directory.GetFiles(pasta, "*.json");
            var batalhas = new List<Batalha>();
            foreach (var arquivo in arquivos) {
                string json = File.ReadAllText(arquivo);
                var options = new JsonSerializerOptions {
                    Converters = {
                        new JsonStringEnumConverter()
                    }
                };
                var batalha = JsonSerializer.Deserialize<Batalha>(json, options);
                if (batalha != null) batalhas.Add(batalha);
            }

            return batalhas;
        }

        #endregion

        #region Métodos para jogadores
        public static void SalvarJogador(Jogador jogador) {
            string pasta = JOGADORES;

            if (!Directory.Exists(pasta)) {
                Directory.CreateDirectory(pasta);
            }

            string caminho = Path.Combine(pasta, $"{jogador.Nome}.json");
            string json = JsonSerializer.Serialize(jogador);
            File.WriteAllText(caminho, json);

            string pastaIA = Path.Combine(IA_STATS, jogador.Nome);
            if (!Directory.Exists(pastaIA)) Directory.CreateDirectory(pastaIA);

            string pastaBatalhas = Path.Combine(BATALHAS, jogador.Nome);
            if (!Directory.Exists(pastaBatalhas)) Directory.CreateDirectory(pastaBatalhas);

            Console.WriteLine($"Jogador '{jogador.Nome}' salvo com sucesso.\n");
        }

        public static Jogador? CarregarJogador(string nome) {
            string pasta = JOGADORES;

            string caminho = Path.Combine(pasta, $"{nome}.json");
            if (!File.Exists(caminho)) {
                Console.WriteLine($"Jogador '{nome}' não encontrado.");
                return null;
            }

            string json = File.ReadAllText(caminho);
            var jogador = JsonSerializer.Deserialize<Jogador>(json);

            if (jogador != null) jogador.Carregando = false;

            Console.WriteLine($"Jogador '{nome}' carregado com sucesso.\n");

            return jogador;
        }

        public static List<string> ListarJogadores() {
            string pasta = JOGADORES;

            if (!Directory.Exists(pasta)) {
                return new List<string>();
            }

            var arquivos = Directory.GetFiles(pasta, "*.json");

            var jogadores = new List<string>();
            foreach (var arquivo in arquivos) {
                jogadores.Add(Path.GetFileNameWithoutExtension(arquivo));
            }

            return jogadores;
        }
        #endregion
    }
}
