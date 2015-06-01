using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL.WpfManagers;
using Common.UiModels.WPF;

namespace WinForms
{
	public static class Helpers
	{
		public static void InvokeThreadSafe(this Action action, Control control)
		{
			if (control.InvokeRequired)
				control.Invoke(action);
			else
				action();
		}
	}
}
