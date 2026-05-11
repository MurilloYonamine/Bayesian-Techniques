namespace Bayesian_Techniques {
    public class GerenciadorEstados {
        public Jogador? JogadorAtual { get; set; }

        private IEstado? _estadoAtual;

        public async Task Comecar(IEstado estadoInicial) {
            _estadoAtual = estadoInicial;
            while (_estadoAtual != null) {
                await _estadoAtual.Executar(this);
            }
        }

        public void MudarEstado(IEstado? novoEstado) {
            _estadoAtual = novoEstado;
        }
    }
}
