using System;
using System.Diagnostics;

namespace Bayesian_Techniques {
    public static class Combate {
        private static Random _random = new Random();

        public static void ResolverTurno(Personagem p1, Acao a1, Personagem p2, Acao a2) {
            if (a1 == Acao.Ataque) {
                LidarAtaque(p1, p2, a2);
            }

            if (a2 == Acao.Ataque) {
                LidarAtaque(p2, p1, a1);
            }

            if (a1 == Acao.Movimento) {
                if (p1.EstaFlanqueando) {
                    Console.WriteLine($"{p1.Nome} já está em posição vantajosa!");
                } else {
                    p1.EstaFlanqueando = true;
                    Console.WriteLine($"{p1.Nome} está circulando para flanquear!");
                }
            } else if (p1.EstaFlanqueando && a1 != Acao.Ataque && a1 != Acao.Movimento) {
                p1.EstaFlanqueando = false;
            }

            if (a2 == Acao.Movimento) {
                if (p2.EstaFlanqueando) {
                    Console.WriteLine($"{p2.Nome} já está em posição vantajosa!");
                } else {
                    p2.EstaFlanqueando = true;
                    Console.WriteLine($"{p2.Nome} está circulando para flanquear!");
                }
            } else if (p2.EstaFlanqueando && a2 != Acao.Ataque && a2 != Acao.Movimento) {
                p2.EstaFlanqueando = false;
            }
        }

        private static void LidarAtaque(Personagem atacante, Personagem defensor, Acao defesa) {
            double dano = atacante.StatusAtaque;

            if (atacante.EstaFlanqueando) {
                dano = Math.Ceiling(dano * 1.5);
                Console.WriteLine($"{atacante.Nome} realizou um Ataque Furtivo Crítico!");
                atacante.EstaFlanqueando = false;
            }

            Console.WriteLine($"{atacante.Nome} quer causar {dano} de dano!");

            switch (defesa) {
                case Acao.Defesa:
                    dano -= defensor.StatusDefesa;
                    Console.WriteLine($"{defensor.Nome} defendeu!");
                    if (dano <= 0) dano = 0;
                    break;

                case Acao.Esquiva:
                    int chance = _random.Next(0, 11);

                    Debug.WriteLine($"Chance de esquiva: {chance} (0-10), Esquiva do defensor: {defensor.StatusEsquiva}");

                    if (chance <= defensor.StatusEsquiva) {
                        Console.WriteLine($"{defensor.Nome} esquivou!");
                        return;
                    }
                    break;

                case Acao.ContraAtaque:
                    Console.WriteLine($"{defensor.Nome} contra-atacou!");

                    atacante.VidaAtual -= defensor.StatusContraAtaque;
                    Console.WriteLine($"{atacante.Nome} recebeu {defensor.StatusContraAtaque} de dano no contra-ataque!");

                    dano = 0;
                    break;
            }

            int danoInt = Math.Max(0, (int)dano);
            defensor.VidaAtual -= danoInt;

            if (defensor.VidaAtual < 0) defensor.VidaAtual = 0;

            Console.WriteLine($"{defensor.Nome} recebeu {danoInt} de dano");
        }
    }
}
