using Common;
using Common.Configuration;
using Common.Db.Entities;
using DAL.RepoCommon;

namespace DAL.DataBase.AdoNet
{
	public static class Utils
	{
		#region TransactionItem helpers

		internal static void CopyStandardParams(this TransactionItem transactionItem, ExinAdoNetContextBase ctx, DbType dbType)
		{
			var comment = string.IsNullOrWhiteSpace(transactionItem.Comment) ? Config.DbStringNull : transactionItem.Comment;

			ctx.Command.Parameters.AddWithValue("@ID", transactionItem.ID, dbType);
			ctx.Command.Parameters.AddWithValue("@Amount", transactionItem.Amount, dbType);
			ctx.Command.Parameters.AddWithValue("@Quantity", transactionItem.Quantity, dbType);
			ctx.Command.Parameters.AddWithValue("@UnitID", transactionItem.UnitID, dbType);
			ctx.Command.Parameters.AddWithValue("@Title", transactionItem.Title, dbType);
			ctx.Command.Parameters.AddWithValue("@Comment", comment, dbType);
			ctx.Command.Parameters.AddWithValue("@Date", transactionItem.Date, dbType);
			ctx.Command.Parameters.AddWithValue("@CategoryID", transactionItem.CategoryID, dbType);
			ctx.Command.Parameters.AddWithValue("@IsExpenseItem", transactionItem.IsExpenseItem, dbType);
			ctx.Command.Parameters.AddWithValue("@IsIncomeItem", transactionItem.IsIncomeItem, dbType);
		}

		#endregion
	}
}
