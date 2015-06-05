using System;
using System.Xml.Linq;
using Common.DbEntities;
using Common.UiModels.WPF.DefaultValues;
using Common.Utils.Helpers;

namespace Common.UiModels.WPF
{
	public partial class IncomeItem : TransactionItemBase
	{
		public IncomeItem()
		{
			Quantity = 1;

			Unit = DefaultValueProvider.Instance.DefaultUnit;
		}

		public override XElement ToXml()
		{
			return new XElement("IncomeItem", new object[]
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
