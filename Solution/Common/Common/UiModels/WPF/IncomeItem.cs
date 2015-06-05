using System;
using System.Xml.Linq;
using Common.DbEntities;
using Common.UiModels.WPF.DefaultValues;
using Common.Utils.Helpers;
using C = Common.Configuration.Constants.XmlTags;

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
			// C <--> Constants.XmlTags
			return new XElement(C.IncomeItem, new object[]
			{
				new XElement(C.Title, Title),
				new XElement(C.Amount, Amount),
				new XElement(C.Comment, Comment),
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
