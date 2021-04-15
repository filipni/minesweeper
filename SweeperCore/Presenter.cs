namespace SweeperCore
{
    public class Presenter
    {
        private readonly IView _view;
        private Game _game;

        public Presenter(IView view)
            => _view = view;

        public void CreateNewGame(int width, int height, int numberOfMines)
        {
            _game = new Game(width, height, numberOfMines);
            _view.Width = _game.BoardWidth;
            _view.Height = _game.BoardHeight;
            _view.ResetBoard();
            _view.State = _game.State;
        }

        public void HandleInput(Move move, Position position)
        {
            var updatedTiles = _game.HandleInput(move, position);
            updatedTiles.ForEach(tile => _view.UpdateTile(tile.Position, tile.Image));
            _view.State = _game.State;
        }
    }
}