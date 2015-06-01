using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Common.Utils.Helpers
{
	public static class WpfHelper
	{
		public static Dispatcher Dispatcher;

		/// <summary>
		/// Insert sorted into already sorted list. If the <see cref="Dispatcher"/> is assigned, 
		/// it inserts besides calling Dispatcher.Invoke
		/// </summary>
		public static void InsertIntoSorted<T>(this IList<T> list, T newItem) where T : IComparable<T>
		{
			int i;
			for(i = 0; i < list.Count; i++)
			{
				if(!(list[i].CompareTo(newItem) > 0))
					break;
			}

			if(Dispatcher!=null)
				Dispatcher.Invoke(() => list.Insert(i, newItem));
			else
				list.Insert(i, newItem);
		}
	}
}
