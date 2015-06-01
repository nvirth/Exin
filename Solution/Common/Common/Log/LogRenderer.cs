using System;
using System.Collections;
using System.IO;
using log4net.ObjectRenderer;

namespace Common.Log
{
	public class LogRenderer : IObjectRenderer
	{
		public void RenderObject(RendererMap rendererMap, object obj, TextWriter writer)
		{
			var exception = obj as Exception;
			if(exception == null)
				writer.WriteLine(obj.SerializeToLog());
			else
			{
				bool isInner = false;
				while(exception != null)
				{
					RenderException(exception, writer, isInner);
					exception = exception.InnerException;
					isInner = true;
				}
			}
		}

		private void RenderException(Exception ex, TextWriter writer, bool isInner = false)
		{
			if(isInner)
			{
				writer.WriteLine();
				writer.WriteLine("----- Inner Exception:");
				writer.WriteLine();
			}

			writer.WriteLine("Message: {0}", ex.Message);
			writer.WriteLine("Type: {0}", ex.GetType().FullName);
			writer.WriteLine("Source: {0}", ex.Source);
			writer.WriteLine("TargetSite: {0}", ex.TargetSite);
			writer.WriteLine();
			RenderExceptionData(ex, writer);
			writer.WriteLine();
			writer.WriteLine("StackTrace:\r\n {0}", ex.StackTrace);
		}

		private void RenderExceptionData(Exception ex, TextWriter writer)
		{
			foreach(DictionaryEntry entry in ex.Data)
			{
				writer.WriteLine("{0}: {1}", entry.Key, entry.Value.SerializeToLog());
			}
		}
	}
}