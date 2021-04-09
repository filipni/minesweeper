namespace minesweeper
{
    record Tile
    {
        public Position Position { get; init; }
        public TileState State { get; init; }
        public bool HasMine { get; init; }
        public int AdjacentMines { get; init; }
    }
}