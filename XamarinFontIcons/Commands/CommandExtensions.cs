//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace dotup.Binding.Commands
//{
//	public static class CommandExtensions
//	{
//		public static AsyncCommand Create(this CommandManager self, Func<Task> action)
//		{
//			var newCommand = new AsyncCommand(action);
//			self..Add(new WeakReference<ICommand>(newCommand));
//			return newCommand;
//		}

//		public ICommand NewAsyncCommand(Func<Task> action, Func<bool> canExecute, Action<Exception> onException = null, bool continueOnCapturedContext = false)
//		{
//			var newCommand = new AsyncCommand(action, canExecute, onException, continueOnCapturedContext);
//			commands.Add(new WeakReference<ICommand>(newCommand));
//			return newCommand;
//		}

//		public ICommand NewAsyncCommand<T>(Func<T, Task> action)
//		{
//			var newCommand = new AsyncCommand<T>(action);
//			commands.Add(new WeakReference<ICommand>(newCommand));
//			return newCommand;
//		}

//		public ICommand NewAsyncCommand<T>(Func<T, Task> action, Func<T, bool> canExecute)
//		{
//			var newCommand = new AsyncCommand<T>(action, canExecute);
//			commands.Add(new WeakReference<ICommand>(newCommand));
//			return newCommand;
//		}

//		/// <summary>
//		/// Remove non existing targets from internal reference list
//		/// </summary>
//		public void CleanUp()
//		{
//			var itemsToRemove = commands.Where(item => !item.TryGetTarget(out var target)).ToList();

//			foreach (var item in itemsToRemove)
//			{
//				commands.Remove(item);
//			}
//		}

//	}
//}
