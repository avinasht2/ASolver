using System;
using System.Windows.Input;

namespace PriceTicker
{
    public class RelayCommand : ICommand
    {
        private event EventHandler canExecuteChangedHandler;

        private Predicate<object> canExecutePredicate;
        private Action<object> executeAction;

        private bool autoRequery = false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="executeAction"></param>
        /// <param name="canExecutePredicate"></param>
        /// <param name="autoRequery"></param>
        public RelayCommand(Action<object> executeAction, Predicate<object> canExecutePredicate = null, bool autoRequery = true)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("The executeAction must be specified");
            }

            this.executeAction = executeAction;
            this.canExecutePredicate = canExecutePredicate;
            this.autoRequery = autoRequery;
        }

        /// <summary>
        /// Implements the <see cref="ICommand.CanExecuteChanged"/> event handler.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.autoRequery)
                {
                    CommandManager.RequerySuggested += value;
                }
                else
                {
                    this.canExecuteChangedHandler += value;
                }
            }

            remove
            {
                if (this.autoRequery)
                {
                    CommandManager.RequerySuggested -= value;
                }
                else
                {
                    this.canExecuteChangedHandler -= value;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            bool canExecute = false;
            try
            {
                canExecute = (this.canExecutePredicate == null) ? true : this.canExecutePredicate(parameter);
            }
            catch (Exception)
            {
            }

            return canExecute;
        }

        /// <summary>
        /// Implements <see cref="ICommand.Execute"/>.  Invoked by the command binding to execute the command behavior.
        /// </summary>
        /// <param name="parameter">A parameter passed from the command binding. This value may be null.</param>
        public void Execute(object parameter)
        {
            this.executeAction(parameter);
        }
    }
}