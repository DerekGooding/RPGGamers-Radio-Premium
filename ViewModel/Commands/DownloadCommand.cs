using System;
using System.Threading;
using System.Windows.Input;

namespace Radio_Leech.ViewModel.Commands
{
    public class DownloadCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        readonly LeechVM VM;
        public DownloadCommand(LeechVM vm) => VM = vm;

        public bool CanExecute(object? parameter) => VM.SelectedSong != null;
        public void Execute(object? parameter) => new Thread(async () =>
        {
            await LeechVM.SaveSong(VM.SelectedSong);
        }).Start();
    }
}
