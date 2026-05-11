namespace Bayesian_Techniques {
    public static class Decisao {
        public static Acao Iniciar(Personagem personagem) {
            Random random = new Random();

            var pesos = new Dictionary<Acao, int>
            {
                { Acao.Ataque, personagem.StatusAtaque },
                { Acao.Defesa, personagem.StatusDefesa },
                { Acao.Esquiva, personagem.StatusEsquiva },
                { Acao.ContraAtaque, personagem.StatusContraAtaque },
                { Acao.Movimento, personagem.StatusMovimentacao }
            };

            int soma = 0;
            foreach (var peso in pesos.Values) {
                soma += peso;
            }

            int sorteio = random.Next(0, soma);

            int acumulado = 0;

            foreach (var item in pesos) {
                acumulado += item.Value;

                if (sorteio <= acumulado) return item.Key;
            }

            return Acao.Ataque;
        }

        public static Acao Iniciar(Acao acao) {
            return acao;
        }
    }
}
