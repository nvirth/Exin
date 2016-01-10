using System.Runtime.InteropServices;
using Exin.Common.Logging.Core;

namespace WPF.Web.Charting
{
	[ComVisible(visibility: true)]
	public class PlotlyChartConnector
	{
		/// Could be set from JS via "window.external.Prop1 = ..."
		/// A better solution would be to create a strongly typed Prop2 propety, plus a SetProp2 String setter for it,
		/// and call JsonConvert.DeserializeObject in that setter
		public object Prop1 { get; set; }

		public void LogInfo(string message)
		{
			Log.Info(this, m => m("WebBrowser says: " + message));
		}
	}
}
