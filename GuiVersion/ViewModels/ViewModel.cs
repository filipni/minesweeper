using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GuiVersion.Commands;
using SweeperCore;

namespace GuiVersion.ViewModels
{
    public class ViewModel : IView, INotifyPropertyChanged
    {
        public ObservableCollection<TileViewModel> Tiles { get; } = new ObservableCollection<TileViewModel>();
        public NewGameCommand NewGameCommand { get; }
        public GameState State { get; set; }

        private int _width;
        public int Width
        {
            get => _width;
            private set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private int _height;
        public int Height
        {
            get => _height;
            private set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        private Presenter _presenter;

        public ViewModel()
        {
            _presenter = new Presenter(this);
            _presenter.CreateNewGame(10, 10, 20);
            NewGameCommand = new NewGameCommand(this);
        }

        public void CreateBoard(int width, int height)
        {
            Tiles.Clear();

            Width = width;
            Height = height;

            var numberOfTiles = width * height;
            for (var i = 0; i < numberOfTiles; i++)
            {
                var row = i / width;
                var column = i % width;
                Tiles.Add(new TileViewModel(TileImage.Hidden, new Position(row, column), this));
            }
        }

        public void NewGameClicked() => _presenter.CreateNewGame(10, 10, 20);

        public void HandleTileClicked(Position position)
            => _presenter.HandleInput(Move.Reveal, position);

        public void Show() => throw new System.NotImplementedException();

        public void UpdateTile(Position position, TileImage image)
            => Tiles[position.Row * Width + position.Column].Image = image;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
