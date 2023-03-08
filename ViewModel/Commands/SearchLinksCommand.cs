using System;
using System.Windows.Input;

namespace Radio_Leech.ViewModel.Commands
{
    public class SearchLinksCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        readonly LeechVM VM;
        public SearchLinksCommand(LeechVM vm) => VM = vm;

        public bool CanExecute(object? parameter) => false;
        public void Execute(object? parameter) => VM.LookForLinksAsync();
    }
}
