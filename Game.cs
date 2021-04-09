using System;
using System.Collections.Generic;
using System.Linq;

namespace minesweeper
{
    class Game
    {
        public GameState State { get; private set; } = GameState.Running;

        private Board _board;
        private bool _isFirstAction = true;
        private int _width;
        private int _height;
        private int _numberOfMines;
        private int _tilesLeftToReveal;

        public Game(int width, int height, int numberOfMines)
        {
            var numberOfTiles = width * height;
            if (numberOfMines > numberOfTiles)
            {
                throw new System.ArgumentException("Number of mines cannot exceed the number of tiles.", nameof(numberOfMines));
            }

            _width = width;
            _height = height;
            _numberOfMines = numberOfMines;
            _tilesLeftToReveal = numberOfTiles - numberOfMines;
        }

        public List<Tile> HandleInput(Action action, Position position)
        {
            if (_isFirstAction)
            {
                _board = new Board(_width, _height, _numberOfMines, position);
                _isFirstAction = false;
            }

            var tileFound = _board.TryGetTile(position, out var tile);
            if (!tileFound || State != GameState.Running)
            {
                return new List<Tile>();
            }

            return action switch
            {
                Action.Reveal => RevealTile(tile),
                _ => new List<Tile>()
            };
        }

        private List<Tile> RevealTile(Tile tile)
        {
            if (!CanBeRevealed(tile))
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
                    if (CanBeRevealed(neighbour) && !queue.Contains(neighbour))
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

        private bool CanBeRevealed(Tile tile)
            => tile.State != TileState.Flagged
                && tile.State != TileState.Questioned
                && tile.State != TileState.Revealed;

        public void PrintBoard()
            => Console.WriteLine(_board);
    }
}