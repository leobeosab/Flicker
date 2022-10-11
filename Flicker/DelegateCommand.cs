using System;
using System.Windows.Input;

namespace Flicker;

public class DelegateCommand : ICommand
{
    public Action? CommandAction { get; init; }
    private Func<bool>? CanExecuteFunc => null;

    public void Execute(object? parameter)
    {
        CommandAction?.Invoke();
    }

    public bool CanExecute(object? parameter)
    {
        return CanExecuteFunc == null || CanExecuteFunc();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}    