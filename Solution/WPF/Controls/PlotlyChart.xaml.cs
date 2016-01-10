﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Db.Entities;
using WPF.ViewModels;
using WPF.Web.Charting;

namespace WPF.Controls
{
	public partial class PlotlyChart : UserControl
	{
		public PlotlyChartViewModel ViewModel { get; private set; }

		public PlotlyChart()
		{
			InitializeComponent();
			//LayoutRoot.DataContext = this;

			ViewModel = new PlotlyChartViewModel(WebBrowser);

			this.DataContextChanged += (sender, args) => RefreshViewModel();
			RefreshViewModel();
		}

		private void RefreshViewModel()
		{
			var data = this.DataContext as IEnumerable<KeyValuePair<Category, int>>;
			if(data != null)
				ViewModel.Refresh(data);
		}
	}
}
