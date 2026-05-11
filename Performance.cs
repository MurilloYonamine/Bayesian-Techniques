using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bayesian_Techniques {
    public class IAStatsBatalha {
        public string BatalhaId { get; set; }
        public string Resultado { get; set; }

        public Dictionary<Acao, int> AcoesIA { get; set; } = new();
        public Dictionary<Acao, int> AcoesJogador { get; set; } = new();

        public PrevisaoStats Previsao { get; set; } = new();
        public DanoStats Dano { get; set; } = new();
        public MetricStats Metricas { get; set; } = new();

        public IAStatsBatalha() {
            BatalhaId = Guid.NewGuid().ToString();

            foreach (var acao in new[] { Acao.Ataque, Acao.Defesa, Acao.Esquiva, Acao.ContraAtaque, Acao.Movimento }) {
                AcoesIA[acao] = 0;
                AcoesJogador[acao] = 0;
            }
        }
    }

    public class PrevisaoStats {
        public int Total { get; set; }
        public int Acertos { get; set; }
    }

    public class DanoStats {
        public int CausadoIA { get; set; }
        public int RecebidoIA { get; set; }

        // also expose damage from perspective of jogador
        public int CausadoJogador { get; set; }
        public int RecebidoJogador { get; set; }
    }

    public class MetricStats {
        public double PrecisaoPrevisao { get; set; }
    }
}
