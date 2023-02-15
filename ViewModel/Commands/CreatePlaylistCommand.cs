using System;
using System.Windows.Input;

namespace Radio_Leech.ViewModel.Commands
{
    public class CreatePlaylistCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        readonly LeechVM VM;
        public CreatePlaylistCommand(LeechVM vm) => VM = vm;

        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => VM.CreatePlaylist();
    }
}
