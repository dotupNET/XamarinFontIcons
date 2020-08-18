using dotup.Validation;
using System;
using System.Windows.Input;

namespace dotup.Binding.Commands
{
	/// <summary>
	/// Implementation of ICommand
	/// </summary>
	public class Command<T> : ICommand
	{
		private readonly Action<T> execute;
		private readonly Func<T, bool> canExecute;

		/// <summary>
		/// Command that takes an action to execute.
		/// </summary>
		/// <param name="execute">Action to execute.</param>
		public Command(Action<T> execute) :
			this(execute, _ => true)
		{ }

		/// <summary>
		/// Command that takes an action to execute.
		/// </summary>
		/// <param name="execute">Action to execute.</param>
		/// <param name="canExecute">Function to determine if can execute.</param>
		public Command(Action<T> execute, Func<T, bool> canExecute)
		{
			Validator.MustNotNull(execute, nameof(execute));
			Validator.MustNotNull(canExecute, nameof(canExecute));
			this.execute = execute;
			this.canExecute = canExecute;
		}

		/// <summary>
		/// Invoke the CanExecute method to determine if it can be executed.
		/// </summary>
		/// <param name="parameter">Parameter to test and pass to CanExecute.</param>
		/// <returns>If it can be executed.</returns>
		public bool CanExecute(object parameter)
		{
			return canExecute?.Invoke((T)parameter) ?? true;
		}

		/// <summary>
		/// Event handler raised when CanExecute changes.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add => WeakEventManager.Current.AddEventHandler(value);
			remove => WeakEventManager.Current.RemoveEventHandler(value);
		}

		/// <summary>
		/// Execute the command with or without a parameter.
		/// </summary>
		/// <param name="parameter">Parameter to pass to execute method.</param>
		public void Execute(object parameter)
		{
			execute((T)parameter);
		}

		/// <summary>
		/// Manually raise a CanExecuteChanged event.
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			WeakEventManager.Current.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
		}
	}
}