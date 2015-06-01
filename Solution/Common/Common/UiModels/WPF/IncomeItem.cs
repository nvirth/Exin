using System;
using System.Xml.Linq;
using Common.DbEntities;
using Common.Utils;
using Common.Utils.Helpers;
using UtilsShared;

namespace Common.UiModels.WPF
{
	public partial class IncomeItem : TransactionItemBase
	{
		public IncomeItem()
		{
			Quantity = 1;

			// Here, in the Common.dll, we can not reach DAL methods...
			Unit = new Unit()
			{
				ID = 0,
				Name = "None",
				DisplayName = "Nincs",
			};
		}

		public override XElement ToXml()
		{
			return new XElement("IncomeItem", new[]
			{
				new XElement("Title", Title),
				new XElement("Amount", Amount),
				new XElement("Comment", Comment),
			});
		}
	}

	[Serializable]
	public partial class IncomeItem
	{
		public override object Clone()
		{
			return this.DeepClone();
		}
	}
}
