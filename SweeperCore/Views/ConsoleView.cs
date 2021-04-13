using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SweeperCore.Views
{
    public class ConsoleView : IView
    {
        public GameState State { get; set; }

        private int _boardWidth;
        private int _boardHeight;
        private TileImage[,] _board;
        private readonly IPresenter _presenter;
        private readonly Dictionary<TileImage, char> _symbolTable = new Dictionary<TileImage, char>
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
                var (move, position) = GetAction();
                _presenter.HandleInput(move, position);
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

        private (Move, Position) GetAction()
        {
            var moveString = string.Empty;
            var row = int.MaxValue;
            var column = int.MaxValue;

            while (row < 0 || row >= _boardHeight || column < 0 || column >= _boardWidth)
            {
                var inputs = GetInput("Choose action ({.|p|?|#} {row} {column}): ", @"^(\.|p|\?|#) (\d{1,3}) (\d{1,3})$");
                moveString = inputs[0];
                row = int.Parse(inputs[1]) - 1; // - 1 since grid starts indexing from 1
                column = int.Parse(inputs[2]) - 1;
            }

            var position = new Position(row, column);
            var move = moveString switch
            {
                "." => Move.Reveal,
                "p" => Move.Flag,
                "?" => Move.Question,
                "#" => Move.Reset
            };

            return (move, position);
        }

        private static List<string> GetInput(string query, string inputFormat)
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
            for (var i = 1; i < match.Groups.Count; i++)
            {
                inputs.Add(match.Groups[i].Value);
            }

            return inputs;
        }

        private void PrintBoard()
        {
            var rowIndices = Enumerable.Range(1, _boardHeight).Select(x => x.ToString());
            var maxIndexLength = rowIndices.Max(x => x.Length);

            var sb = new StringBuilder();
            sb.Append(CreateIndexHeader(maxIndexLength));

            for (var i = 0; i < _boardHeight; i++)
            {
                var indexToShow = i + 1;
                sb.Append(new string(' ', maxIndexLength - indexToShow.ToString().Length));
                sb.Append(indexToShow);
                sb.Append("|");

                for (var j = 0; j < _boardWidth; j++)
                {
                    var symbol = _symbolTable[_board[i, j]]; 
                    sb.Append(symbol);
                } 

                sb.Append("|");
                sb.Append(indexToShow);

                sb.Append(Environment.NewLine);
            }

            sb.Append(CreateIndexHeader(maxIndexLength, true));
            Console.WriteLine(sb.ToString());
        }

        private string CreateIndexHeader(int paddingLength, bool reverseOrder = false)
        {
            var columnIndices = Enumerable.Range(1, _boardWidth).Select(x => x.ToString());
            var numberOfHeaderRows = columnIndices.Max(x => x.Length);

            var headerRows = new List<string>();
            var padding = new string(' ', paddingLength);

            for (var i = numberOfHeaderRows; i > 0; i--)
            {
                var sb = new StringBuilder(padding);
                sb.Append(' ');

                columnIndices
                    .Select(x => x.Length >= i ? x[^i] : ' ')
                    .ToList()
                    .ForEach(x => sb.Append(x));

                sb.Append(Environment.NewLine);
                headerRows.Add(sb.ToString());
            }
            
            var border = new string('-', columnIndices.Count());
            headerRows.Add($"{padding}+{border}+{Environment.NewLine}");

            if (reverseOrder)
            {
                headerRows.Reverse();
            }

            return string.Concat(headerRows);
        }
    }
}