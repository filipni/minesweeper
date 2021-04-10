namespace minesweeper
{
    interface IPresenter
    {
        void CreateNewGame(int width, int height, int numberOfMines);
        void HandleInput(Action action, Position position);
    }
}