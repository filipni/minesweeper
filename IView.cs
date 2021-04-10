namespace minesweeper
{
    interface IView
    {
        GameState State { set; }

        void CreateGrid(int width, int height);
        void Show();
        void UpdateTile(Position position, TileImage image);
    }
}