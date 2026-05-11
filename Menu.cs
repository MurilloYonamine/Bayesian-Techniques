namespace Bayesian_Techniques {
    public class Menu {
        private ConsoleKey _key;
        private string[] _opcoes;
        public Menu(string[] opcoes) {
            _opcoes = opcoes;
        }

        public void ShowMenu(out int escolha) {
            Console.CursorVisible = false;

            for (int i = 0; i < _opcoes.Length; i++) {
                Console.WriteLine();
            }

            int primeiraOpcao = Console.CursorTop - _opcoes.Length;
            escolha = 0;

            do {
                Console.SetCursorPosition(0, primeiraOpcao);
                for (int i = 0; i < _opcoes.Length; i++) {
                    string texto = i == escolha ? $"> {_opcoes[i]}" : $"  {_opcoes[i]}";
                    texto = texto.PadRight(Console.WindowWidth - 1);

                    if (i == escolha) {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(texto);
                        Console.ResetColor();
                    } else {
                        Console.WriteLine(texto);
                    }
                }

                _key = Console.ReadKey(true).Key;
                switch (_key) {
                    case ConsoleKey.UpArrow:
                        escolha = (escolha - 1 + _opcoes.Length) % _opcoes.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        escolha = (escolha + 1) % _opcoes.Length;
                        break;
                    case ConsoleKey.Enter:
                        _key = ConsoleKey.Enter;
                        break;
                }
            } while (_key != ConsoleKey.Enter);

            Console.CursorVisible = true;

            Console.SetCursorPosition(0, primeiraOpcao + _opcoes.Length);

            Console.Clear();
        }
    }
}
