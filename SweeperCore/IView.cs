namespace SweeperCore
{
    public interface IView
    {
        GameState State { set; }
        public int Width { set; }
        public int Height { set; }

        void ResetBoard();
        void UpdateTile(Position position, TileImage image);
    }
}