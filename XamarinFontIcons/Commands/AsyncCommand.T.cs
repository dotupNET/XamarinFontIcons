using dotup.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace dotup.Binding.Commands
{
	/// <summary>
	/// Implementation of a generic Async Command
	/// </summary>
	public class AsyncCommand<T> : ICommand // : IAsyncCommand<T>
	{

		readonly Func<T, Task> execute;
		readonly Func<T, bool> canExecute;
		readonly Action<Exception> onException;
		readonly bool continueOnCapturedContext;

		public AsyncCommand(
			Func<T, Task> execute,
			Action<Exception> onException = null,
			bool continueOnCapturedContext = false
		) :
			this(execute, _ => true, onException, continueOnCapturedContext)
		{ }

		/// <summary>
		/// Create a new AsyncCommand
		/// </summary>
		/// <param name="execute">Function to execute</param>
		/// <param name="canExecute">Function to call to determine if it can be executed</param>
		/// <param name="onException">Action callback when an exception occurs</param>
		/// <param name="continueOnCapturedContext">If the context should be captured on exception</param>
		public AsyncCommand(
			Func<T, Task> execute,
			Func<T, bool> canExecute,
			Action<Exception> onException = null,
			bool continueOnCapturedContext = false)
		{
			Validator.MustNotNull(execute, nameof(execute));
			Validator.MustNotNull(canExecute, nameof(canExecute));
			this.execute = execute;
			this.canExecute = canExecute;
			this.onException = onException;
			this.continueOnCapturedContext = continueOnCapturedContext;
		}

		/// <summary>
		/// Event triggered when Can Excecute changes.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { WeakEventManager.Current.AddEventHandler(value); }
			remove { WeakEventManager.Current.RemoveEventHandler(value); }
		}

		/// <summary>
		/// Invoke the CanExecute method and return if it can be executed.
		/// </summary>
		/// <param name="parameter">Parameter to pass to CanExecute.</param>
		/// <returns>If it can be executed</returns>
		public bool CanExecute(object parameter)
		{
			return canExecute?.Invoke((T)parameter) ?? true;
		}

		/// <summary>
		/// Execute the command async.
		/// </summary>
		/// <returns>Task that is executing and can be awaited.</returns>
		public Task ExecuteAsync(T parameter)
		{
			return execute(parameter);
		}

		/// <summary>
		/// Raise a CanExecute change event.
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			WeakEventManager.Current.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
		}

		#region Explicit implementations

		void System.Windows.Input.ICommand.Execute(object parameter)
		{
			Validator.MustBeValidParameter<T>(parameter, nameof(parameter));
			ExecuteAsync((T)parameter).SafeFireAndForget(continueOnCapturedContext, onException);

		}
		#endregion
	}
}
