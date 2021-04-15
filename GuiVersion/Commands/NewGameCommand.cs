using System;
using System.Windows.Input;
using GuiVersion.ViewModels;
using SweeperCore;

namespace GuiVersion.Commands
{
    public class NewGameCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly ViewModel _viewModel;

        public NewGameCommand(ViewModel vm) => _viewModel = vm;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _viewModel.NewGameClicked();
    }
}
