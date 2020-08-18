namespace dotup.Binding.Commands
{
	public interface ICommand: System.Windows.Input.ICommand
	{
		void RaiseCanExecuteChanged();
	}
}