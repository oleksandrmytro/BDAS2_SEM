using System;
using System.Windows.Input;

namespace BDAS2_SEM.Commands
{
    // Třída implementující příkaz RelayCommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        // Událost informující o změně stavu příkazu (CanExecute)
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Konstruktor příkazu RelayCommand
        // Parametry:
        // - execute: Akce, která bude provedena při spuštění příkazu
        // - canExecute: Podmínka, která určuje, zda je příkaz povolen
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        // Určuje, zda může být příkaz proveden
        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        // Spustí zadanou akci příkazu
        public void Execute(object parameter) => execute(parameter);
    }

    // Generická verze RelayCommand<T>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Predicate<T> canExecute;

        // Událost informující o změně stavu příkazu (CanExecute)
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Konstruktor generického příkazu RelayCommand<T>
        // Parametry:
        // - execute: Akce, která bude provedena při spuštění příkazu
        // - canExecute: Podmínka, která určuje, zda je příkaz povolen
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        // Určuje, zda může být příkaz proveden
        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;

            if (parameter == null && typeof(T).IsValueType)
                return canExecute(default);

            return canExecute((T)parameter);
        }

        // Spustí zadanou akci příkazu
        public void Execute(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
                execute(default);
            else
                execute((T)parameter);
        }
    }

    // Asynchronní příkaz pro provádění úloh na pozadí
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object, Task> _executeAsync;
        private readonly Func<object, bool> _canExecute;

        // Konstruktor asynchronního příkazu
        // Parametry:
        // - executeAsync: Funkce, která se spustí asynchronně
        // - canExecute: Podmínka, která určuje, zda je příkaz povolen
        public AsyncRelayCommand(Func<object, Task> executeAsync, Func<object, bool> canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        // Určuje, zda může být příkaz proveden
        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        // Spustí zadanou funkci příkazu asynchronně
        public async void Execute(object parameter)
        {
            await _executeAsync(parameter);
        }

        // Událost informující o změně stavu příkazu (CanExecute)
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
