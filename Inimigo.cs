using System;
using System.Collections.Generic;
using System.Linq;
using Bayesian_Techniques.Estados;

namespace Bayesian_Techniques {
    public class Inimigo : Personagem {
        private int _vidaAtual;

        private static Random _rnd = new Random();

        public string Nome { get; set; }
        public int VidaMaxima { get; set; }
        public int VidaAtual {
            get => _vidaAtual;
            set {
                _vidaAtual = Math.Max(0, value);
            }
        }

        // === Tendências ===
        public int StatusAtaque { get; set; }
        public int StatusDefesa { get; set; }
        public int StatusEsquiva { get; set; }
        public int StatusContraAtaque { get; set; }
        public int StatusMovimentacao { get; set; }

        public bool EstaFlanqueando { get; set; } = false;

        public IAStatsBatalha Performance { get; set; }

        List<Batalha> _batalhasLista;

        public Inimigo(Jogador jogador) {
            Nome = "Inimigo";
            VidaMaxima = 100;
            VidaAtual = VidaMaxima;
            StatusAtaque = 5;
            StatusDefesa = 5;
            StatusEsquiva = 5;
            StatusContraAtaque = 5;
            StatusMovimentacao = 5;

            Performance = new IAStatsBatalha();

            _batalhasLista = BancoDeDados.CarregarBatalhas(jogador);
        }

        public void PrepararParaBatalha() {
            VidaAtual = VidaMaxima;
            EstaFlanqueando = false;
        }

        public (Acao acao, float confianca) PreverAcao(Jogador jogador, Acao acaoAnterior, int proximoTurno) {
            // A = a acao futura do jogador (jogador vai atacar)
            // B = evidencia observada (ultima acao foi ataque)
            // A | B = qual a posibilidade de A dado B (ataque proximo | ataque anterior)
            // P(A) (prior) = probabilidade inicial de acontecer
            // P(B | A) (likelihood) = probabilidade de observar a evidencia dado que A aconteceu

            // P(A | B) = (P(B | A) * P(A)) / P(B)

            if (_batalhasLista.Count == 0) return (Acao.None, 0.0f);

            float resultadoAtaque = Bayes.Calcular(Acao.Ataque, acaoAnterior, _batalhasLista);
            float resultadoDefesa = Bayes.Calcular(Acao.Defesa, acaoAnterior, _batalhasLista);
            float resultadoEsquiva = Bayes.Calcular(Acao.Esquiva, acaoAnterior, _batalhasLista);
            float resultadoContraAtaque = Bayes.Calcular(Acao.ContraAtaque, acaoAnterior, _batalhasLista);
            float resultadoMovimentacao = Bayes.Calcular(Acao.Movimento, acaoAnterior, _batalhasLista);

            Dictionary<Acao, float> probabilidades = new Dictionary<Acao, float> {
                { Acao.Ataque, resultadoAtaque },
                { Acao.Defesa, resultadoDefesa },
                { Acao.Esquiva, resultadoEsquiva },
                { Acao.ContraAtaque, resultadoContraAtaque },
                { Acao.Movimento, resultadoMovimentacao }
            };

            float soma = probabilidades.Values.Sum();

            if (soma <= 0) return (Acao.Ataque, 0f);

            var melhorPrevisao = probabilidades.OrderByDescending(kv => kv.Value).First();

            Acao escolhida = melhorPrevisao.Key;
            float confianca = melhorPrevisao.Value / soma;

            return (escolhida, confianca);
        }

        public Acao EscolherRespostaParaPrevisao(Acao previsao, float confianca, bool estaFlanqueando) {
            if (estaFlanqueando) {
                return Acao.Ataque;
            }

            if (confianca > 0.45f) {
                switch (previsao) {
                    case Acao.Ataque: return Acao.ContraAtaque;
                    case Acao.Defesa: return Acao.Ataque;
                    case Acao.Esquiva: return Acao.Ataque;
                    case Acao.ContraAtaque: return Acao.Defesa;
                    case Acao.Movimento: return Acao.Ataque;
                }
            }

            var peso = new Dictionary<Acao, int>();

            switch (previsao) {
                case Acao.Ataque:
                    peso[Acao.Defesa] = Math.Max(0, StatusDefesa);
                    peso[Acao.Esquiva] = Math.Max(0, StatusEsquiva);
                    peso[Acao.ContraAtaque] = Math.Max(0, StatusContraAtaque);
                    peso[Acao.Movimento] = Math.Max(1, StatusMovimentacao);
                    peso[Acao.Ataque] = Math.Max(1, StatusAtaque / 4);
                    break;

                case Acao.ContraAtaque:
                    // Zera pesos de Ataque e ContraAtaque e dá peso alto em Movimento
                    peso[Acao.Ataque] = 0;
                    peso[Acao.ContraAtaque] = 0;
                    peso[Acao.Movimento] = Math.Max(2, StatusMovimentacao * 3);
                    peso[Acao.Defesa] = Math.Max(1, StatusDefesa);
                    peso[Acao.Esquiva] = Math.Max(1, StatusEsquiva);
                    break;

                case Acao.Defesa:
                    // A IA sabe que é seguro atacar, então o foco é 100% agressivo.
                    peso[Acao.Ataque] = Math.Max(1, StatusAtaque); // Garante que terá peso
                    peso[Acao.Defesa] = 0;
                    peso[Acao.Esquiva] = 0;
                    peso[Acao.ContraAtaque] = 0;
                    peso[Acao.Movimento] = 0;
                    break;

                case Acao.Esquiva:
                    // A IA sabe que é seguro atacar, então o foco é 100% agressivo.
                    peso[Acao.Ataque] = Math.Max(1, StatusAtaque); // Garante que terá peso
                    peso[Acao.Defesa] = 0;
                    peso[Acao.Esquiva] = 0;
                    peso[Acao.ContraAtaque] = 0;
                    peso[Acao.Movimento] = 0;
                    break;

                case Acao.Movimento:
                    // Se o jogador vai se mover, a IA tem uma distribuição normal, mas priorizando alcançá-lo ou bater
                    peso[Acao.Ataque] = Math.Max(0, StatusAtaque);
                    peso[Acao.Movimento] = Math.Max(0, StatusMovimentacao);
                    peso[Acao.Defesa] = Math.Max(1, StatusDefesa / 2);
                    peso[Acao.Esquiva] = Math.Max(1, StatusEsquiva / 2);
                    peso[Acao.ContraAtaque] = Math.Max(1, StatusContraAtaque / 2);
                    break;

                default:
                    peso[Acao.Ataque] = Math.Max(1, StatusAtaque);
                    peso[Acao.Defesa] = Math.Max(1, StatusDefesa);
                    peso[Acao.Esquiva] = Math.Max(1, StatusEsquiva);
                    peso[Acao.ContraAtaque] = Math.Max(1, StatusContraAtaque);
                    peso[Acao.Movimento] = Math.Max(1, StatusMovimentacao);
                    break;
            }

            int total = peso.Values.Sum();
            if (total <= 0) {
                return Decisao.Iniciar(previsao);
            }

            int rolada = _rnd.Next(0, total);
            int contador = 0;
            foreach (var kv in peso) {
                contador += kv.Value;
                if (rolada < contador) return kv.Key;
            }

            return Decisao.Iniciar(previsao);
        }
    }
}
