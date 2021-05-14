using System;
using System.Collections.Generic;
using System.Linq;

namespace SweeperCore
{
    internal class Board
    {
        public IEnumerable<Tile> Tiles
            => _positions.Select(position => GetTile(position));

        private readonly IEnumerable<Position> _positions;
        private readonly Tile[,] _grid;
        private readonly int _width;
        private readonly int _height;

        public Board(int width, int height, int numberOfMines, Position startingPosition)
        {
            _width = width;
            _height = height;
            _grid = new Tile[height, width];
            _positions = GetPositionsInArea(new Position(0, 0), width, height);

            InitializeTiles(startingPosition, numberOfMines);
        }

        public IEnumerable<Position> GetPositionsInArea(Position areaStart, int width, int height)
        {
            var columns = Enumerable.Range(areaStart.Column, width);
            var rows = Enumerable.Range(areaStart.Row, height);
            return rows.SelectMany(row => GetRowPositions(row, width, columns));
        }

        private IEnumerable<Position> GetRowPositions(int row, int width, IEnumerable<int> columns)
            => Enumerable.Repeat(row, width)
                .Zip(columns, (x, y) => new Position(x, y));

        public void InitializeTiles(Position startingPosition, int numberOfMines)
        {
            var random = new Random();
            var mines =
                _positions
                .Where(x => x != startingPosition)
                .OrderBy(x => random.Next())
                .Take(numberOfMines)
                .ToList();

            _positions
                .ToList()
                .ForEach(position => CreateTile(position, mines));
        }

        private void CreateTile(Position position, List<Position> mines)
        {
            var adjacentMines = GetAdjacentPositions(position).Intersect(mines).Count();
            var hasMine = mines.Contains(position);
            var tile = new Tile(position, hasMine, adjacentMines);
            SetTile(tile);
        }

        public bool TryGetTile(Position position, out Tile tile)
        {
            tile = PositionOutOfBounds(position) ? null : GetTile(position);
            return tile != null;
        }

        private Tile GetTile(Position position)
            => _grid[position.Row, position.Column];

        public void SetTile(Tile tile)
            => _grid[tile.Position.Row, tile.Position.Column] = tile;

        public IEnumerable<Tile> GetAdjacentTiles(Tile tile)
            => GetAdjacentPositions(tile.Position).Select(position => GetTile(position));

        private IEnumerable<Position> GetAdjacentPositions(Position position)
            => GetPositionsInArea(new Position(position.Row - 1, position.Column - 1), 3, 3)
                .Where(neighbour => !PositionOutOfBounds(neighbour) && neighbour != position);

        private bool PositionOutOfBounds(Position position)
            => position.Row < 0
                || position.Row > _height - 1
                || position.Column < 0
                || position.Column > _width - 1;
    }
}