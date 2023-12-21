using System;
using System.Windows.Input;

namespace EquipmentRandomTesting.Tools;

public class ObservableCommand<T> : ICommand
{
    private readonly Action<T> action;

    public event EventHandler? CanExecuteChanged;

    public ObservableCommand(Action<T> action)
    {
        this.action = action;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        action((T)parameter!);
    }
}