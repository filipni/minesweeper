namespace SweeperCore
{
    public interface IPresenter
    {
        void CreateNewGame(int width, int height, int numberOfMines);
        void HandleInput(Move move, Position position);
    }
}