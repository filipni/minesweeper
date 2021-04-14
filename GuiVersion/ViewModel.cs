using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using SweeperCore;

namespace GuiVersion
{
    class ViewModel : IView
    {
        public ObservableCollection<TileImage> Tiles;
        public GameState State { get; set; }

        private int _boardWidth;
        private int _boardHeight;
        private Presenter _presenter;

        public ViewModel()
        {
            _presenter = new Presenter(this);
            _presenter.CreateNewGame(10, 10, 10);
        }

        public void CreateBoard(int width, int height)
            => Tiles = new ObservableCollection<TileImage>(new List<TileImage>(width * height));

        public void Show() { return; }

        public void UpdateTile(Position position, TileImage image)
            => Tiles[position.Row * _boardWidth + position.Column] = image;
    }
}
