using System;
using System.Text.RegularExpressions;

namespace minesweeper
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(10, 10, 20);

            while (game.State == GameState.Running)
            {
                Console.WriteLine("Reveal position: ");
                var input = System.Console.ReadLine();
                var match = Regex.Match(input, @"(\d+) (\d+)");
                if (match.Success)
                {
                    var row = int.Parse(match.Groups[1].Value);
                    var column = int.Parse(match.Groups[2].Value);
                    var position = new Position(row, column);
                    game.RevealTile(position);
                    game.PrintBoard();
                }
            }
        }
    }
}
