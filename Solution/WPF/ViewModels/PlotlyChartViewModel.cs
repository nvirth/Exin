using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Common.Db.Entities;
using WPF.Web.Charting;

namespace WPF.ViewModels
{
	public class PlotlyChartViewModel
	{
		public WebBrowser WebBrowser { get; set; }
		public PlotlyChartManager Manager { get; private set; }

		public PlotlyChartViewModel(WebBrowser webBrowser)
		{
			WebBrowser = webBrowser;
			Manager = new PlotlyChartManager(webBrowser);
		}

		public void Refresh(IEnumerable<KeyValuePair<Category, int>> newData)
		{
			Manager.DoCharting(newData);
		}
	}
}
