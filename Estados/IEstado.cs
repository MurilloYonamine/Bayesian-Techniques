namespace Bayesian_Techniques {
    public interface IEstado {
        Task Executar(GerenciadorEstados manager);
    }
}