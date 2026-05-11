using Bayesian_Techniques.Estados;

namespace Bayesian_Techniques {
    public static class Bayes {
        public static float Calcular(Acao acao, Acao acaoAnterior, List<Batalha> batalhas) {
            float prior = CalcularPrior(acao, batalhas);
            float likelihood = CalcularLikelihood(acao, acaoAnterior, batalhas);
            float evidence = CalcularEvidencia(acaoAnterior, batalhas);

            if (evidence == 0) {
                return prior;
            }

            return (float)(likelihood * prior) / evidence;
        }
        private static float CalcularPrior(Acao acao, List<Batalha> batalhas) {
            // P(A) = probabilidade inicial de acontecer (o que quer prever)
            int countAcao = 0;
            int countTotal = 0;

            foreach (var batalha in batalhas) {
                foreach (var turno in batalha.Turnos) {
                    if (turno.AcaoJogador == acao) {
                        countAcao++;
                    }
                    countTotal++;
                }
            }

            if (countTotal == 0) return 0;

            return (float)countAcao / countTotal;
        }
        private static float CalcularEvidencia(Acao acaoAnterior, List<Batalha> batalhas) {
            // P(B) = probabilidade de observar a evidencia (o que observou)
            int countAcaoAnterior = 0;
            int countTotal = 0;

            foreach (var batalha in batalhas) {
                foreach (var turno in batalha.Turnos) {
                    if (turno.AcaoJogador == acaoAnterior) {
                        countAcaoAnterior++;
                    }
                    countTotal++;
                }
            }

            if (countTotal == 0) return 0;
            return (float)countAcaoAnterior / countTotal;
        }

        private static float CalcularLikelihood(Acao acao, Acao AcaoAnterior, List<Batalha> batalhas) {
            // P(B | A) = probabilidade de observar a evidencia dado que A aconteceu

            int countAcao = 0;
            int countAcaoAnterior = 0;

            foreach (var batalha in batalhas) {
                for (int i = 0; i < batalha.Turnos.Count; i++) {
                    if (batalha.Turnos[i].AcaoJogador == acao) {

                        countAcao++;

                        if (i > 0 && batalha.Turnos[i - 1].AcaoJogador == AcaoAnterior) {
                            countAcaoAnterior++;
                        }
                    }
                }
            }

            if (countAcao == 0) return 0;

            return (float)countAcaoAnterior / countAcao;
        }
    }
}
