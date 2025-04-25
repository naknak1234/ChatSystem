using System;
using System.Windows.Input;

namespace ChatSystem.Commands
{
    public class ChatCommand : ICommand
    {
        private readonly Action _execute;
        public event EventHandler CanExecuteChanged;
        public ChatCommand(Action execute) => _execute = execute;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute();
    }
}