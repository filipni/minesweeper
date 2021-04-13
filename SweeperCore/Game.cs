using System;
using System.Collections.Generic;
using System.Linq;

namespace SweeperCore
{
    internal class Game
    {
        public GameState State { get; private set; } = GameState.Running;
        public int BoardWidth { get; }
        public int BoardHeight { get; }

        private Board _board;
        private const int MinSide = 4;
        private const int MaxSide = 100;
        private bool _isFirstAction = true;
        private readonly int _numberOfMines;
        private int _tilesLeftToReveal;

        public Game(int width, int height, int numberOfMines)
        {
            width = Math.Clamp(width, MinSide, MaxSide);
            height = Math.Clamp(height, MinSide, MaxSide);

            var numberOfTiles = width * height;
            if (numberOfMines >= numberOfTiles)
            {
                numberOfMines = numberOfTiles - 1;
            }

            BoardWidth = width;
            BoardHeight = height;
            _numberOfMines = numberOfMines;
            _tilesLeftToReveal = numberOfTiles - numberOfMines;
        }

        public List<Tile> HandleInput(Move move, Position position)
        {
            if (_isFirstAction)
            {
                _board = new Board(BoardWidth, BoardHeight, _numberOfMines, position);
                _isFirstAction = false;
            }

            var tileFound = _board.TryGetTile(position, out var tile);
            if (!tileFound || State != GameState.Running)
            {
                return new List<Tile>();
            }

            return move switch
            {
                Move.Reveal => RevealTile(tile),
                Move.Flag => FlagTile(tile),
                Move.Question => QuestionTile(tile),
                Move.Reset => ResetTile(tile),
                _ => new List<Tile>()
            };
        }

        private List<Tile> FlagTile(Tile tile)
            => MarkTile(tile, TileState.Flagged);

        private List<Tile> QuestionTile(Tile tile)
            => MarkTile(tile, TileState.Questioned);

        private List<Tile> ResetTile(Tile tile)
            => MarkTile(tile, TileState.Hidden);

        private List<Tile> MarkTile(Tile tile, TileState marking)
        {
            if (tile.Cleared)
            {
                return new List<Tile>();
            }

            var markedTile = tile with {State = marking};
            _board.SetTile(markedTile);
            return new List<Tile> {markedTile};
        }

        private List<Tile> RevealTile(Tile tile)
        {
            if (tile.Cleared)
            {
                return new List<Tile>();
            }

            if (tile.HasMine)
            {
                State = GameState.Lost;

                var explodedTile = tile with {State = TileState.Exploded};
                _board.SetTile(explodedTile);

                var revealedMines = RevealHiddenMines();
                revealedMines.Add(explodedTile);
                return revealedMines;
            }

            var revealedTiles = BfsReveal(tile);
            _tilesLeftToReveal -= revealedTiles.Count();
            if (_tilesLeftToReveal == 0)
            {
                State = GameState.Won;
            }
            
            return revealedTiles;
        }

        private List<Tile> BfsReveal(Tile startTile)
        {
            var revealedTiles = new List<Tile>();
            var queue = new Queue<Tile>();
            queue.Enqueue(startTile);

            while (queue.Any())
            {
                var revealedTile = queue.Dequeue() with {State = TileState.Revealed};
                _board.SetTile(revealedTile);
                revealedTiles.Add(revealedTile);

                if (revealedTile.AdjacentMines != 0)
                {
                    continue;
                }

                var adjacentTiles = _board.GetAdjacentTiles(revealedTile);
                foreach (var neighbour in adjacentTiles)
                {
                    if (CanBeRevealedAutomatically(neighbour) && !queue.Contains(neighbour))
                    {
                        queue.Enqueue(neighbour);
                    }
                }
            }
            
            return revealedTiles;
        }

        private List<Tile> RevealHiddenMines()
        {
            var revealedMines = _board.Tiles
                .Where(tile => !tile.Cleared && tile.HasMine)
                .Select(tile => tile with {State = TileState.Revealed})
                .ToList();

            revealedMines.ForEach(tile => _board.SetTile(tile));
            return revealedMines;
        }

        private bool CanBeRevealedAutomatically(Tile tile)
            => !tile.Cleared && !tile.Marked;
    }
}