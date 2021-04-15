﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using GuiVersion.Commands;
using SweeperCore;

namespace GuiVersion.ViewModels
{
    public class ViewModel : IView, INotifyPropertyChanged
    {
        public ObservableCollection<TileViewModel> Tiles { get; } = new ObservableCollection<TileViewModel>();
        public NewGameCommand NewGameCommand { get; }

        private GameState _state;
        public GameState State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged(nameof(GameIsRunning));
                OnPropertyChanged(nameof(GameIsWon));
            }
        }

        public bool GameIsRunning => _state == GameState.Running;
        public bool GameIsWon => _state == GameState.Won;

        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private int _height;
        public int Height
        {
            get => _height;
            set
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

        public void ResetBoard()
        {
            Tiles.Clear();
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    Tiles.Add(new TileViewModel(TileImage.Hidden, new Position(i, j), this));
                }
            }
        }

        public void NewGameClicked() => _presenter.CreateNewGame(10, 10, 20);

        public void HandleTileClicked(Position position)
            => _presenter.HandleInput(Move.Reveal, position);

        internal void HandleTileRightClicked(Position position, TileImage image)
        {
            switch (image)
            {
                case TileImage.Hidden:
                    _presenter.HandleInput(Move.Flag, position);
                    break;
                case TileImage.Flagged:
                    _presenter.HandleInput(Move.Question, position);
                    break;
                case TileImage.Questioned:
                    _presenter.HandleInput(Move.Reset, position);
                    break;
            }
        }

        public void UpdateTile(Position position, TileImage image)
            => Tiles[position.Row * Width + position.Column].Image = image;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
