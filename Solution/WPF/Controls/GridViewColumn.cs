using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF.Controls
{
	public class GridViewColumn : System.Windows.Controls.GridViewColumn
	{
		public static readonly DependencyProperty DataPropertyProperty = DependencyProperty.Register(
			"DataProperty", typeof(object), typeof(GridViewColumn), new PropertyMetadata(default(object)));

		public object DataProperty
		{
			get { return (object)GetValue(DataPropertyProperty); }
			set { SetValue(DataPropertyProperty, value); }
		}
	}
}
