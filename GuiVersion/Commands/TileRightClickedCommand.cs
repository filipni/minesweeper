using System;
using System.Windows.Input;
using GuiVersion.ViewModels;
using SweeperCore;

namespace GuiVersion.Commands
{
    public class TileRightClickedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly ViewModel _viewModel;
        private readonly TileViewModel _tileViewModel;

        public TileRightClickedCommand(ViewModel viewModel, TileViewModel tileViewModel)
            => (_viewModel, _tileViewModel) = (viewModel, tileViewModel);

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is TileViewModel tile)
            {
                _viewModel.HandleTileRightClicked(tile.Position, tile.Image);
            }
        }
    }
}
