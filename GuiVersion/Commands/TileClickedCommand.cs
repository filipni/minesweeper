using System;
using System.Windows.Input;
using GuiVersion.ViewModels;
using SweeperCore;

namespace GuiVersion.Commands
{
    public class TileClickedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly ViewModel _viewModel;

        public TileClickedCommand(ViewModel vm) => _viewModel = vm;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is Position position)
            {
                _viewModel.HandleTileClicked(position);
            }
        }
    }
}
