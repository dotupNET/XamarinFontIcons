using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Input;

namespace dotup.Binding.Commands
{
	public class ObservableCommand<T> : Command<T>
	{
		private readonly ISubject<T> executeSubject = new Subject<T>();

		public ObservableCommand() :
			base(_ => { })
		{ }

		public ObservableCommand(Func<T, bool> canExecute) :
			base(_ => { }, canExecute)
		{ }

		public new void Execute(object parameter)
		{
			executeSubject.OnNext((T)parameter);
		}

		public IObservable<T> ExecuteObservable
		{
			get
			{
				return executeSubject.AsObservable();
			}
		}

	}
}
