namespace minesweeper
{
    record Position
    {
        public int Row { get; }
        public int Column { get; }
        public Position(int row, int column) => (Row, Column) = (row, column);
    }
}