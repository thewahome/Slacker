using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Slacker
{
	public static class Commands
	{
		#region Properties

		public static Command Exit
		{
			get;
			private set;
		}

		public static Command EditSetting
		{
			get;
			private set;
		}

		public static Command SaveSetting
		{
			get;
			private set;
		}

		#endregion

		#region Constructors

		static Commands()
		{
			Exit = new Command(); 
			EditSetting = new Command();
			SaveSetting = new Command();
		}

		#endregion
	}

	public class Command : ICommand
	{
		#region Properties

		public Action<object> ExecuteAction
		{
			get;
			set;
		}

		public Func<object,bool> CanExecuteFunc
		{
			get;
			set;
		}

		#endregion

		#region ICommand Implement

		public bool CanExecute(object parameter)
		{
			return CanExecuteFunc == null || 
				   CanExecuteFunc(parameter);
		}

		public void Execute(object parameter)
		{
			if (ExecuteAction != null)
				ExecuteAction(parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		#endregion
	}
}
