using Bayesian_Techniques.Estados;

namespace Bayesian_Techniques {
    internal class Program {
        static async Task Main() {
            var gerenciador = new GerenciadorEstados();
            await gerenciador.Comecar(new Lobby());
        }
    }
}
