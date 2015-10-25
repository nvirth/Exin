using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;

namespace Common.Utils
{
	public class ComparableComparer<T> : IComparer<T> where T : IComparable<T>
	{
		public int Compare(T x, T y)
		{
			if(x == null || y == null)
			{
				if(x == null && y == null)
					return 0;
				else if(x == null)
					return -1;
				else //if(y == null)
					return +1;
			}

			var result = x.CompareTo(y);
			return result;
		}
	}
}
