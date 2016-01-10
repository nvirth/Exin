using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Common.Configuration;
using Common.Db.Entities;
using Common.Utils.Helpers;
using Exin.Common.Logging.Core;
using WPF.ValueConverters;
using C = Common.Configuration.Constants.Resources.DefaultCategories;


namespace WPF.Web.Charting
{
	public class PlotlyChartManager
	{
		public WebBrowser WebBrowser { get; set; }

		public static readonly TaskCompletionSource<object> LoadCompletedDfd = new TaskCompletionSource<object>();

		public PlotlyChartManager(WebBrowser webBrowser)
		{
			WebBrowser = webBrowser;
			WebBrowser.ObjectForScripting = new PlotlyChartConnector();
			WebBrowser.LoadCompleted += (sender, args) => LoadCompletedDfd.SetResult(null);

			if(Config.WebChartingHtmlPath.CountOf(':') != 1)
				Log.Warn(this, m => m("Config.WebChartingHtmlPath.CountOf(':') != 1. WebBrowser.Source won't be set. "));
			else
				WebBrowser.Source = new Uri(@"file://127.0.0.1/" + Config.WebChartingHtmlPath.Replace(':', '$'));
		}

		public void DoCharting(IEnumerable<KeyValuePair<Category, int>> newData)
		{
			DoChartingSafe(newData);
		}

		private int doChartingCallerCount = 0;

		private async void DoChartingSafe(IEnumerable<KeyValuePair<Category, int>> newData)
		{
			var actualCallerCount = ++doChartingCallerCount;

			await LoadCompletedDfd.Task;

			if(actualCallerCount == doChartingCallerCount)
				DoChartingImpl(newData);
		}

		private void DoChartingImpl(IEnumerable<KeyValuePair<Category, int>> newData)
		{
			// https://plot.ly/javascript/reference/#bar

			var xValues = new List<string>();
			var yValues = new List<int>();
			var annotations = new List<object>();
			var colors = new List<string>();
			var barTexts = new List<string>();

			var i = 0;
			foreach(var kvp in newData)
			{
				var category = kvp.Key;

				var x = kvp.Key.DisplayName;
				var y = kvp.Value;
				var yK = (int)Math.Round(y/1000.0);
				var ykStr = "{0}k Ft".Formatted(yK.ToString("N0"));  // TODO currency

				var annotation = new {
					x = i,
					xref = "x",
					y = y,
					yref = "y",
					text = ykStr,
					showarrow = false,
					xanchor = "center",
					yanchor = "bottom",
				};

				xValues.Add(x);
				yValues.Add(y);
				annotations.Add(annotation);

				colors.Add(GetColorFor(category));
				barTexts.Add(ykStr);

				i++;
			}

			var trace1 = new {
				x = xValues,
				y = yValues,
				type = "bar",
				text = barTexts, // TODO show the bar's value directly above each bar
				//mode = "markers",
				marker = new {
					color = colors,
				},
				hoverinfo = "x+text", // Any combination of "x", "y", "z", "text", "name" joined with a "+" OR "all" or "none". 
			};

			var yAxisMax = Config.Repo.Settings.UserSettings.StatYAxisMax;
			var yAyisRange = yAxisMax.HasValue ? new[] { 0, ChartYAxisMaxConverter.Intance.Convert(yAxisMax.Value) } : null;

			var layout = new {
				autosize = true,
				yaxis = new {
					//title = "Ft",
					ticksuffix = " Ft",
					//tickprefix = "$ ", // TODO currency
					range = yAyisRange,
				},
				xaxis = new { type = "category", },
				annotations = annotations,
			};

			var options = new {
				displaylogo = false,
				scrollZoom = true,
				//autosizable = true, // plot will respect layout.autosize=true and infer its container size
				//fillFrame = true, // if we DO autosize, do we fill the container or the screen?
				//frameMargins = 0, // if we DO autosize, set the frame margins in percents of plot size
				modeBarButtons = new [] {
					new [] {
						PlotlyModeBarButtons.resetScale2d, 
						PlotlyModeBarButtons.autoScale2d, 
						PlotlyModeBarButtons.pan2d, 
						PlotlyModeBarButtons.zoom2d, 
						PlotlyModeBarButtons.zoomIn2d, 
						PlotlyModeBarButtons.zoomOut2d, 
						PlotlyModeBarButtons.hoverClosestCartesian, 
						PlotlyModeBarButtons.hoverCompareCartesian, 
					}.Select(e => e.ToString())
				}
			};

			var dataJson = trace1.Yield().ToJson();
			var layoutJson = layout.ToJson();
			var optionsJson = options.ToJson();

			WebBrowser.InvokeScript(PlotlyChartJsConstants.plainBarChart, dataJson, layoutJson, optionsJson);
		}

		private static string Rgb(int r, int g, int b)
		{
			return "rgb({0}, {1}, {2})".Formatted(r, g, b);
		}
		private static string GetColorFor(Category category)
		{
			switch(category.Name)
			{
				case C.FullExpenseSummary: return Rgb(255, 0, 0);
				case C.FullIncomeSummary: return Rgb(128, 255, 0);
				default: return Rgb(255, 128, 0);
			}
		}
	}
}
