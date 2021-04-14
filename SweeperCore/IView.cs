namespace SweeperCore
{
    public interface IView
    {
        GameState State { set; }

        void CreateBoard(int width, int height);
        void Show();
        void UpdateTile(Position position, TileImage image);
    }
}