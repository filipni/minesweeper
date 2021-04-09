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
                _board.SetTile(explodedTile.Position, explodedTile);

                var revealedMines = RevealMines();
                revealedMines.Add(explodedTile);
                return revealedMines;
            }

            var revealedTiles = new List<Tile>();
            var queue = new Queue<Tile>();
            queue.Enqueue(tile);

            while (queue.Any())
            {
                var currentTile = queue.Dequeue();
                var revealedTile = currentTile with {State = TileState.Revealed};

                revealedTiles.Add(revealedTile);
                _board.SetTile(revealedTile.Position, revealedTile);
                --_tilesLeftToReveal;

                if (_tilesLeftToReveal == 0)
                {
                    State = GameState.Won;
                    return revealedTiles;
                }

                if (currentTile.AdjacentMines != 0)
                {
                    continue;
                }

                var adjacentTiles = _board.GetAdjacentTiles(currentTile);
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

        private List<Tile> RevealMines()
        {
            var updatedTiles = _board.Tiles
                .Where(tile => tile.State != TileState.Revealed && tile.State != TileState.Exploded && tile.HasMine)
                .Select(tile => tile with {State = TileState.Revealed})
                .ToList();

            updatedTiles.ForEach(tile => _board.SetTile(tile.Position, tile));
            return updatedTiles;
        }

        private bool CanBeRevealed(Tile tile)
            => tile.State != TileState.Flagged
                && tile.State != TileState.Questioned
                && tile.State != TileState.Revealed;

        public void PrintBoard()
            => Console.WriteLine(_board);
    }
}