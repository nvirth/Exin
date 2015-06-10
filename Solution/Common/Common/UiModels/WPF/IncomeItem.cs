using System;
using System.Xml.Linq;
using Common.Db;
using C = Common.Configuration.Constants.XmlTags;

namespace Common.UiModels.WPF
{
	public partial class IncomeItem : TransactionItemBase
	{
		public IncomeItem()
		{
			Quantity = 1;

			Unit = ManagersRelief.UnitManager.GetDefaultUnit;
		}
	}

	[Serializable]
	public partial class IncomeItem
	{
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

		public static IncomeItem FromXml(DateTime date, XElement xmlEi)
		{
			date = new DateTime(date.Year, date.Month, 1);
			var incomeItem = new IncomeItem
			{
				Title = ((string)xmlEi.Element(C.Title)).Trim(),
				Amount = ((int)xmlEi.Element(C.Amount)),
				Comment = ((string)xmlEi.Element(C.Comment) ?? "").Trim(),
				Date = date,
			};
			return incomeItem;
		}
	}
}
