namespace DecisionTree.ViewModel
{
    using System;
    using System.Windows.Input;

    class DelegateCommand : ICommand
    {
        private bool _canExecuteCommand;

        public bool CanExecuteCommand   
        {
            get
            {
                return _canExecuteCommand;
            }
            set
            {
                _canExecuteCommand = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private readonly Action _action;

        public DelegateCommand(Action action)
        {
            _action = action;
            _canExecuteCommand = true;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter)
        {
            return _canExecuteCommand;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            _action();
        }

        public event EventHandler CanExecuteChanged;
    }
}
