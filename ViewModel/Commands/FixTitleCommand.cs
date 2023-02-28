using System;
using System.Windows.Input;

namespace Radio_Leech.ViewModel.Commands
{
    public class FixTitleCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        readonly LeechVM VM;
        public FixTitleCommand(LeechVM vm) => VM = vm;

        public bool CanExecute(object? parameter) => VM.SelectedSong != null;
        public void Execute(object? parameter) => VM.FixTitle();
    }
}
