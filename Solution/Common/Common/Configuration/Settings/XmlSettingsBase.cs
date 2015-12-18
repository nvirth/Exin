using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.UiModels.WPF.Base;
using Common.Utils.Helpers;

namespace Common.Configuration.Settings
{
	public abstract class XmlSettingsBase : NotifyPropertyChanged
	{
		protected readonly XElement XmlDoc;
		protected readonly string XmlFilePath;

		protected readonly XElement XElement;

		protected XmlSettingsBase(string xmlFilePath, XElement xmlDoc, XElement xElement)
		{
			XmlFilePath = xmlFilePath;
			XmlDoc = xmlDoc;
            XElement = xElement;
		}

		/// Call this after the properties have been initialized
		protected void Init()
		{
			this.PropertyChanged += (sender, args) => Save();
		}

		#region Save

		private readonly object _xmlDocLock = new object();

		private const int SaveDelay = 2000;
		private readonly Stopwatch _stopWatch = new Stopwatch().WithStart();
		private TimeSpan _latestSaveCall;

		public async void Save()
		{
			try
			{
				var current = _stopWatch.Elapsed;
				_latestSaveCall = current;

				await Task.Delay(SaveDelay);

				// Prevent too much IO operation by delaying them. Only the last one will be executed
				if(_latestSaveCall != current)
					return;

				lock(_xmlDocLock)
				{
					XmlDoc.Save(XmlFilePath);
				}

				ExinLog.ger.LogInfo("{0}. Successfully saved".Formatted(this.GetType().Name));
            }
			catch(Exception e)
			{
				ExinLog.ger.LogException("Could not save {0}. ".Formatted(this.GetType().Name), e);
			}
		}

		#endregion

	}
}