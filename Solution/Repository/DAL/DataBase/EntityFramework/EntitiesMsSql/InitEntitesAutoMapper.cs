using Common.Db.Entities;
using Common.Utils;
using Common.Utils.Helpers;

namespace DAL.DataBase.EntityFramework.EntitiesMsSql
{
	// Unit (in both ms sql and slite) has no navigation properties

	public partial class Category_MsSql
	{
		public static void InitAutoMapper(Category sourceProxy = null)
		{
			var x = new Category_MsSql();
			bool wasNew;
			AutoMapperInitializer<Category, Category_MsSql>
				.InitializeIfNeeded(out wasNew, sourceProxy)
				.ForMemberIfNeeded(wasNew, x.Property(y => y.SummaryItems), imce => imce.Ignore())
				.ForMemberIfNeeded(wasNew, x.Property(y => y.TransactionItems), imce => imce.Ignore())
				;
		}
	}
	public partial class SummaryItem_MsSql
	{
		public static void InitAutoMapper(SummaryItem sourceProxy = null)
		{
			var x = new SummaryItem_MsSql();
			bool wasNew;
			AutoMapperInitializer<SummaryItem, SummaryItem_MsSql>
				.InitializeIfNeeded(out wasNew, sourceProxy)
				.ForMemberIfNeeded(wasNew, x.Property(y => y.Category), imce => imce.Ignore())
				;
		}
	}
	public partial class TransactionItem_MsSql
	{
		public static void InitAutoMapper(TransactionItem sourceProxy = null)
		{
			var x = new TransactionItem_MsSql();
			bool wasNew;
			AutoMapperInitializer<TransactionItem, TransactionItem_MsSql>
				.InitializeIfNeeded(out wasNew, sourceProxy)
				.ForMemberIfNeeded(wasNew, x.Property(y => y.Category), imce => imce.Ignore())
				.ForMemberIfNeeded(wasNew, x.Property(y => y.Unit), imce => imce.Ignore())
				;
		}
	}
	public partial class Unit_MsSql
	{
		public static void InitAutoMapper(Unit sourceProxy = null)
		{
			var x = new Unit_MsSql();
			bool wasNew;
			AutoMapperInitializer<Unit, Unit_MsSql>
				.InitializeIfNeeded(out wasNew, sourceProxy)
				;
		}
	}
}
