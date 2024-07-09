using System;
using System.Threading.Tasks;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Func<Task> _executeAsync;
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Func<Task> executeAsync, Func<bool> canExecute = null)
    {
        _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        _canExecute = canExecute;
    }

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public async void Execute(object parameter)
    {
        if (_executeAsync != null)
        {
            await _executeAsync();
        }
        else
        {
            _execute();
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
