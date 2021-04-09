using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace minesweeper
{
    class Board
    {
        public IEnumerable<Tile> Tiles { get; }

        private IEnumerable<Position> _positions;
        private Tile[,] _grid;
        private readonly int _width;
        private readonly int _height;


        public Board(int width, int height, int numberOfMines, Position startingPosition)
        {
            _width = width;
            _height = height;
            _grid = new Tile[height, width];

            _positions = GetPositionsInArea(new Position(0, 0), _width, _height);

            InitializeTiles(startingPosition, numberOfMines);

            Tiles = _positions.Select(position => { GetTile(position, out var tile); return tile; });
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
            var tile = new Tile(){Position = position, HasMine = hasMine, AdjacentMines = adjacentMines};
            SetTile(tile.Position, tile);
        }

        public bool GetTile(Position position, out Tile tile)
        {
            tile = PositionOutOfBounds(position)
                ? null
                :_grid[position.Row, position.Column];
            return tile != null;
        }

        public void SetTile(Position position, Tile tile)
            => _grid[position.Row, position.Column] = tile;

        public IEnumerable<Tile> GetAdjacentTiles(Tile tile)
            => GetAdjacentPositions(tile.Position).Select(position => { GetTile(position, out var tile); return tile; });

        private IEnumerable<Position> GetAdjacentPositions(Position position)
            => GetPositionsInArea(new Position(position.Row - 1, position.Column - 1), 3, 3)
                .Where(neighbour => !PositionOutOfBounds(neighbour) && neighbour != position);

        private bool PositionOutOfBounds(Position position)
            => position.Row < 0
                || position.Row > _height - 1
                || position.Column < 0
                || position.Column > _width - 1;

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < _height; i++)
            {
               for (int j = 0; j < _width; j++)
               {
                  var symbol = GetTileSymbol(_grid[i, j]); 
                  sb.Append(symbol);
               } 
               sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        private string GetTileSymbol(Tile tile)
        {
            if (tile.State == TileState.Exploded)
            {
                return "X";
            }

            if (tile.HasMine && tile.State == TileState.Revealed)
            {
                return "+";
            }

            if (tile.State == TileState.Revealed)
            {
                return tile.AdjacentMines != 0
                    ? tile.AdjacentMines.ToString()
                    : "-";
            }

            return "#";
        }
        
    }
}