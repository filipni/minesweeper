namespace minesweeper
{
    class Presenter : IPresenter
    {
        private IView _view;
        private Game _game;

        public Presenter(IView view)
            => _view = view;

        public void CreateNewGame(int width, int height, int numberOfMines)
        {
            _game = new Game(width, height, numberOfMines);
            _view.CreateBoard(_game.BoardWidth, _game.BoardHeight);
            _view.State = _game.State;
        }

        public void HandleInput(Action action, Position position)
        {
            var updatedTiles = _game.HandleInput(action, position);
            updatedTiles.ForEach(tile => _view.UpdateTile(tile.Position, tile.Image));
            _view.State = _game.State;
        }
    }
}