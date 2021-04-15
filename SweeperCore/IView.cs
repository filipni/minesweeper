namespace SweeperCore
{
    public interface IView
    {
        GameState State { set; }
        public int Width { get; set; }
        public int Height { get; set; }

        void ResetBoard();
        void UpdateTile(Position position, TileImage image);
    }
}