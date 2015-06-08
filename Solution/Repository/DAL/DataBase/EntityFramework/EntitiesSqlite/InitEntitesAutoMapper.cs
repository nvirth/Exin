using Common.Db.Entities;
using Common.Utils;
using Common.Utils.Helpers;

namespace DAL.DataBase.EntityFramework.EntitiesSqlite
{
	// Unit (in both ms sql and slite) has no navigation properties

	public partial class Category_Sqlite
	{
		public static void InitAutoMapper(Category sourceProxy = null)
		{
			var x = new Category_Sqlite();
			bool wasNew;
			AutoMapperInitializer<Category, Category_Sqlite>
				.InitializeIfNeeded(out wasNew, sourceProxy)
				.ForMemberIfNeeded(wasNew, x.Property(y => y.SummaryItems), imce => imce.Ignore())
				.ForMemberIfNeeded(wasNew, x.Property(y => y.TransactionItems), imce => imce.Ignore())
				;
		}
	}
	public partial class SummaryItem_Sqlite
	{
		public static void InitAutoMapper(SummaryItem sourceProxy = null)
		{
			var x = new SummaryItem_Sqlite();
			bool wasNew;
			AutoMapperInitializer<SummaryItem, SummaryItem_Sqlite>
				.InitializeIfNeeded(out wasNew, sourceProxy)
				.ForMemberIfNeeded(wasNew, x.Property(y => y.Category), imce => imce.Ignore())
				;
		}
	}
	public partial class TransactionItem_Sqlite
	{
		public static void InitAutoMapper(TransactionItem sourceProxy = null)
		{
			var x = new TransactionItem_Sqlite();
			bool wasNew;
			AutoMapperInitializer<TransactionItem, TransactionItem_Sqlite>
				.InitializeIfNeeded(out wasNew, sourceProxy)
				.ForMemberIfNeeded(wasNew, x.Property(y => y.Category), imce => imce.Ignore())
				.ForMemberIfNeeded(wasNew, x.Property(y => y.Unit), imce => imce.Ignore())
				;
		}
	}
	public partial class Unit_Sqlite
	{
		public static void InitAutoMapper(Unit sourceProxy = null)
		{
			var x = new Unit_Sqlite();
			bool wasNew;
			AutoMapperInitializer<Unit, Unit_Sqlite>
				.InitializeIfNeeded(out wasNew, sourceProxy)
				;
		}
	}
}
