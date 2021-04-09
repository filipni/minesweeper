using System;
using System.Collections.Generic;
using System.Linq;

namespace minesweeper
{
    class Game
    {
        public GameState State { get; private set; } = GameState.Running;

        private Board _board;
        private bool _isFirstReveal = true;
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

        public List<Tile> RevealTile(Position position)
        {
            if (State != GameState.Running)
            {
                return new List<Tile>();
            }

            if (_isFirstReveal)
            {
                _board = new Board(_width, _height, _numberOfMines, position);
                _isFirstReveal = false;
            }

            var tileFound = _board.GetTile(position, out var chosenTile);
            if (!tileFound || !CanBeRevealed(chosenTile))
            {
                return new List<Tile>();
            }

            if (chosenTile.HasMine)
            {
                State = GameState.Lost;

                var explodedTile = chosenTile with {State = TileState.Exploded};
                _board.SetTile(explodedTile.Position, explodedTile);

                var updatedTiles = _board.Tiles
                    .Where(tile => tile.State != TileState.Revealed && tile.Position != explodedTile.Position && tile.HasMine)
                    .Select(tile => tile with {State = TileState.Revealed})
                    .ToList();

                updatedTiles.ForEach(tile => _board.SetTile(tile.Position, tile));
                return updatedTiles.Append(explodedTile).ToList();
            }

            var revealedTiles = new List<Tile>();
            var queue = new Queue<Tile>();
            queue.Enqueue(chosenTile);

            while (queue.Any())
            {
                var tile = queue.Dequeue();
                var revealedTile = tile with {State = TileState.Revealed};

                revealedTiles.Add(revealedTile);
                _board.SetTile(revealedTile.Position, revealedTile);
                --_tilesLeftToReveal;

                if (_tilesLeftToReveal == 0)
                {
                    State = GameState.Won;
                    return revealedTiles;
                }

                if (tile.AdjacentMines != 0)
                {
                    continue;
                }

                var adjacentTiles = _board.GetAdjacentTiles(tile);
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

        private bool CanBeRevealed(Tile tile)
            => tile.State != TileState.Flagged
                && tile.State != TileState.Questioned
                && tile.State != TileState.Revealed;

        public void PrintBoard()
            => Console.WriteLine(_board);
    }
}