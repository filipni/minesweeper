using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ViewModel() => _presenter = new Presenter(this);

        public void CreateBoard(int width, int height)
            => Tiles = new ObservableCollection<TileImage>(new List<TileImage>(width * height));

        public void Show() => throw new System.NotImplementedException();

        public void UpdateTile(Position position, TileImage image)
            => Tiles[position.Row * _boardWidth + position.Column] = image;
    }
}
