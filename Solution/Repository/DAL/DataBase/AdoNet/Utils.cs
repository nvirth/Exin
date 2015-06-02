using Common.Config;
using Common.DbEntities;

namespace DAL.DataBase.AdoNet
{
	public static class Utils
	{
		#region TransactionItem helpers

		internal static void CopyStandardParams(this TransactionItem transactionItem, ExinAdoNetContextBase ctx)
		{
			var comment = string.IsNullOrWhiteSpace(transactionItem.Comment) ? Config.StringNull : transactionItem.Comment;

			ctx.Command.Parameters.AddWithValue("@ID", transactionItem.ID);
			ctx.Command.Parameters.AddWithValue("@Amount", transactionItem.Amount);
			ctx.Command.Parameters.AddWithValue("@Quantity", transactionItem.Quantity);
			ctx.Command.Parameters.AddWithValue("@UnitID", transactionItem.UnitID);
			ctx.Command.Parameters.AddWithValue("@Title", transactionItem.Title);
			ctx.Command.Parameters.AddWithValue("@Comment", comment);
			ctx.Command.Parameters.AddWithValue("@Date", transactionItem.Date);
			ctx.Command.Parameters.AddWithValue("@CategoryID", transactionItem.CategoryID);
			ctx.Command.Parameters.AddWithValue("@IsExpenseItem", transactionItem.IsExpenseItem);
			ctx.Command.Parameters.AddWithValue("@IsIncomeItem", transactionItem.IsIncomeItem);
		}

		#endregion
	}
}
