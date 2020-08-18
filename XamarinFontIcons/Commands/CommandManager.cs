using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotup.Binding.Commands
{
	public class CommandManager
	{
		private static readonly Lazy<CommandManager> current = new Lazy<CommandManager>(() => new CommandManager());
		public static CommandManager Current { get { return current.Value; } }

		private readonly List<WeakReference<ICommand>> commands = new List<WeakReference<ICommand>>();

		private CommandManager()
		{ }

		public void Add(ICommand command)
		{
			this.CleanUp();
			commands.Add(new WeakReference<ICommand>(command));
		}

		public void RemoveAll()
		{
			commands.Clear();
		}

		public void RaiseCanExecuteChanged()
		{
			foreach (var item in commands)
			{
				if (item.TryGetTarget(out ICommand cmd))
					cmd.RaiseCanExecuteChanged();
			}
		}

		public ICommand NewAsyncCommand(Func<Task> action)
		{
			var newCommand = new AsyncCommand(action);
			this.Add(newCommand);
			return newCommand;
		}

		public ICommand NewAsyncCommand(Func<Task> action, Func<bool> canExecute, Action<Exception> onException = null, bool continueOnCapturedContext = false)
		{
			var newCommand = new AsyncCommand(action, canExecute, onException, continueOnCapturedContext);
			this.Add(newCommand);
			return newCommand;
		}

		public ICommand NewAsyncCommand<T>(Func<T, Task> action)
		{
			var newCommand = new AsyncCommand<T>(action);
			this.Add(newCommand);
			return newCommand;
		}

		public ICommand NewAsyncCommand<T>(Func<T, Task> action, Func<T, bool> canExecute)
		{
			var newCommand = new AsyncCommand<T>(action, canExecute);
			this.Add(newCommand);
			return newCommand;
		}

		/// <summary>
		/// Remove non existing targets from internal reference list
		/// </summary>
		public void CleanUp()
		{
			var itemsToRemove = commands.Where(item => !item.TryGetTarget(out var target)).ToList();

			foreach (var item in itemsToRemove)
			{
				commands.Remove(item);
			}
		}

	}
}
