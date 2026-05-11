using System.Text.Json.Serialization;

namespace Bayesian_Techniques {
    public class Jogador : Personagem {
        private string _nome = "Jogador";
        private int _vidaAtual;

        public string Nome {
            get => _nome;
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    Console.WriteLine("Nome não pode ser vazio. Definindo para 'Jogador'.");
                    value = "Jogador";
                }

                if (char.IsLower(value[0])) {
                    Console.WriteLine("Nome deve começar com letra maiúscula. Corrigindo...");
                    value = char.ToUpper(value[0]) + value.Substring(1);
                }

                Console.WriteLine($"Nome definido para: {value}\n");
                _nome = value;
            }
        }
        public int VidaMaxima { get; set; }

        [JsonIgnore]
        public int VidaAtual {
            get => _vidaAtual;
            set {
                _vidaAtual = Math.Max(0, value);
            }
        }

        public int StatusAtaque { get; set; }
        public int StatusDefesa { get; set; }
        public int StatusEsquiva { get; set; }
        public int StatusContraAtaque { get; set; }
        public int StatusMovimentacao { get; set; }

        public bool Carregando { get; set; } = false;

        public bool EstaFlanqueando { get; set; } = false;

        public Jogador() {
            Carregando = false;
        }
        public void PrepararParaBatalha() {
            VidaAtual = VidaMaxima;
        }
        public void Salvar() {
            if (!Carregando) {
                BancoDeDados.SalvarJogador(this);
            }
        }
    }
}