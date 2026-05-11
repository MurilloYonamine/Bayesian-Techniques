namespace Bayesian_Techniques {
    public interface Personagem {
        string Nome { get; set; }

        int VidaMaxima { get; set; }
        int VidaAtual { get; set; }

        // === Tendências ===
        int StatusAtaque { get; set; }
        int StatusDefesa { get; set; }
        int StatusEsquiva { get; set; }
        int StatusContraAtaque { get; set; }
        int StatusMovimentacao { get; set; }

        bool EstaFlanqueando { get; set; }
    }
}
