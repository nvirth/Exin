using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using log4net.Layout.Pattern;

namespace Exin.Common.Logging.Log4Net
{
	public sealed class ExceptionDataPlConverter : PatternLayoutConverter
	{
		protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
		{
			var data = loggingEvent.ExceptionObject?.Data;
			if(data == null)
				return;

			if(data.Keys.Count > 0)
				writer.Write("Data:{0}", Environment.NewLine);

			foreach(var key in data.Keys)
				writer.Write("\t{0}: {1}{2}", key, data[key], Environment.NewLine);
		}
	}
}
