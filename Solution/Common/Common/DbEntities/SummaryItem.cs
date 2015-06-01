using System;
using Common.Utils;
using Common.Utils.Helpers;
using UtilsShared;

namespace Common.DbEntities
{
	public partial class SummaryItem
	{
		public int ID { get; set; }
		public int Amount { get; set; }
		public int CategoryID { get; set; }
		public DateTime Date { get; set; }

		public Category Category { get; set; }
	}

	[Serializable]
	public partial class SummaryItem : ICloneable
	{
		public object Clone()
		{
			return this.DeepClone();
		}
	}
}
