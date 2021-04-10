using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace minesweeper
{
    class ConsoleView : IView
    {
        public GameState State { get; set; }

        private int _boardWidth;
        private int _boardHeight;
        private TileImage[,] _board;
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

        public void CreateBoard(int width, int height)
        {
            _boardWidth = width;
            _boardHeight = height;
            _board = new TileImage[height, width];
        }

        public void UpdateTile(Position position, TileImage image)
            => _board[position.Row, position.Column] = image;

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

            if (_board is null)
            {
                Console.WriteLine("Something went wrong.");
                return false;
            }

            while (State == GameState.Running)
            {
                PrintBoard();
                (Action action, Position position) = GetAction();
                _presenter.HandleInput(action, position);
            }

            PrintBoard();

            var resultMessage = State == GameState.Won
                ? "Congratulations, you won!"
                : "Sorry, you lost.";
            Console.WriteLine(resultMessage);

            Console.Write("Try again? (y) ");
            var input = Console.ReadLine();
            Console.WriteLine();

            return input == "y";
        }

        private void StartGame()
        {
            Console.WriteLine("Welcome to Minesweeper!");
            var inputs = GetInput("Enter game settings ({width} {height} {mines}): ", @"^(\d{1,3}) (\d{1,3}) (\d{1,3})$");

            var width = int.Parse(inputs[0]);
            var height = int.Parse(inputs[1]);
            var numberOfMines = int.Parse(inputs[2]);

            _presenter.CreateNewGame(width, height, numberOfMines);
        }

        private (Action, Position) GetAction()
        {
            var actionString = string.Empty;
            var row = int.MaxValue;
            var column = int.MaxValue;

            while (row >= _boardHeight || column >= _boardWidth)
            {
                var inputs = GetInput("Choose action ({.|p|?|#} {row} {column}): ", @"^(\.|p|\?|#) (\d{1,3}) (\d{1,3})$");
                actionString = inputs[0];
                row = int.Parse(inputs[1]);
                column = int.Parse(inputs[2]);
            }

            var position = new Position(row, column);
            var action = actionString switch
            {
                "." => Action.Reveal,
                "p" => Action.Flag,
                "?" => Action.Question,
                "#" => Action.Reset
            };

            return (action, position);
        }

        private List<string> GetInput(string query, string inputFormat)
        {
            Match match = null;
            var correctInput = false;

            while (!correctInput)
            {
                Console.Write(query);
                var input = Console.ReadLine();

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

            for (int i = 0; i < _boardHeight; i++)
            {
                for (int j = 0; j < _boardWidth; j++)
                {
                    var symbol = _symbolTable[_board[i, j]]; 
                    sb.Append(symbol);
                } 
                sb.Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());
        }
    }
}