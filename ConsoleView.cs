using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace minesweeper
{
    class ConsoleView : IView
    {
        public GameState State { get; set; }

        private TileImage[,] _grid;
        private IPresenter _presenter;
        private Dictionary<TileImage, char> _symbolTable = new Dictionary<TileImage, char>
        {
            [TileImage.Hidden]      = '#',
            [TileImage.Cleared]     = '.',
            [TileImage.Mine]        = '*',
            [TileImage.Exploded]    = 'X',
            [TileImage.Flagged]     = 'P',
            [TileImage.Questioned]  = '?',
            [TileImage.One]         = '1',
            [TileImage.Two]         = '2',
            [TileImage.Three]       = '3',
            [TileImage.Four]        = '4',
            [TileImage.Five]        = '5',
            [TileImage.Six]         = '6',
            [TileImage.Seven]       = '7',
            [TileImage.Eight]       = '8'
        };

        public ConsoleView()
            => _presenter = new Presenter(this);

        public void CreateGrid(int width, int height)
            => _grid = new TileImage[height, width];

        public void UpdateTile(Position position, TileImage image)
            => _grid[position.Row, position.Column] = image;

        public void Show()
        {
            var running = true;
            while (running)
            {
                running = Run();
            }
        }

        private bool Run()
        {
            StartGame();

            if (_grid is null)
            {
                Console.WriteLine("Something went wrong.");
                return false;
            }

            while (State == GameState.Running)
            {
                PrintBoard();
                (Action action, Position position) = GetRevealPosition();
                _presenter.HandleInput(action, position);
            }

            PrintBoard();

            if (State == GameState.Won)
            {
                Console.WriteLine("Congratulations, you won!");
            }
            else
            {
                System.Console.WriteLine("Sorry, you lost.");
            }

            Console.Write("Try again? (y) ");
            var input = Console.ReadLine();
            Console.WriteLine();

            return input == "y";
        }

        private void StartGame()
        {
            Console.WriteLine("Welcome to Minesweeper!");
            var inputs = GetInput("Enter game settings ({width} {height} {mines}): ", @"(\d+) (\d+) (\d+)");

            var width = int.Parse(inputs[0]);
            var height = int.Parse(inputs[1]);
            var numberOfMines = int.Parse(inputs[2]);

            _presenter.CreateNewGame(width, height, numberOfMines);
        }

        private (Action, Position) GetRevealPosition()
        {
            var inputs = GetInput("Reveal position ({row} {column}): ", @"(\d+) (\d+)");

            var row = int.Parse(inputs[0]);
            var column = int.Parse(inputs[1]);
            var position = new Position(row, column);

            return (Action.Reveal, position);
        }

        private List<string> GetInput(string query, string inputFormat)
        {
            Match match = null;
            var correctInput = false;

            while (!correctInput)
            {
                Console.Write(query);
                var input = System.Console.ReadLine();

                match = Regex.Match(input, inputFormat);
                correctInput = match.Success;
            }

            var inputs = new List<string>();
            for (int i = 1; i < match.Groups.Count; i++)
            {
                inputs.Add(match.Groups[i].Value);
            }

            return inputs;
        }

        private void PrintBoard()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    var symbol = _symbolTable[_grid[i, j]]; 
                    sb.Append(symbol);
                } 
                sb.Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());
        }
    }
}