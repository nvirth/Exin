using System.ComponentModel;

namespace Common
{
	internal class EnumResourceKeyConstants
	{
		public const string TabSummaryNumber_DailyExpenses = "Enum_TabSummaryNumber_DailyExpenses";
		public const string TabSummaryNumber_MonthlyExpenses = "Enum_TabSummaryNumber_MonthlyExpenses";
		public const string TabSummaryNumber_MonthlyIncomes = "Enum_TabSummaryNumber_MonthlyIncomes";
		public const string TabSummaryNumber_Statistics = "Enum_TabSummaryNumber_Statistics";
		public const string ReadMode_FromFile = "Enum_ReadMode_FromFile";
		public const string ReadMode_FromDb = "Enum_ReadMode_FromDb";
		public const string SaveMode_OnlyToFile = "Enum_SaveMode_OnlyToFile";
		public const string SaveMode_FileAndDb = "Enum_SaveMode_FileAndDb";
		public const string SaveMode_OnlyToDb = "Enum_SaveMode_OnlyToDb";
		public const string DbAccessMode_None = "Enum_DbAccessMode_None";
		public const string DbAccessMode_AdoNet = "Enum_DbAccessMode_AdoNet";
		public const string DbAccessMode_EntityFramework = "Enum_DbAccessMode_EntityFramework";
		public const string DbType_MsSql = "Enum_DbType_MsSql";
		public const string DbType_SQLite = "Enum_DbType_SQLite";
		public const string TransactionItemType_Expense = "Enum_TransactionItemType_Expense";
		public const string TransactionItemType_Income = "Enum_TransactionItemType_Income";
		public const string TransactionItemType_Both = "Enum_TransactionItemType_Both";
	}

	public enum TabSummaryNumber
	{
		DailyExpenses = 0,
		MonthlyExpenses = 1,
		MonthlyIncomes = 2,
		Statistics = 3,
	}
	public enum ReadMode : byte
	{
		[Description(EnumResourceKeyConstants.ReadMode_FromFile)]
		FromFile = 1,

		[Description(EnumResourceKeyConstants.ReadMode_FromDb)]
		FromDb = 2,
	}
	public enum SaveMode : byte
	{
		[Description(EnumResourceKeyConstants.SaveMode_OnlyToFile)]
		OnlyToFile = 1,

		[Description(EnumResourceKeyConstants.SaveMode_FileAndDb)]
		FileAndDb = 2,

		[Description(EnumResourceKeyConstants.SaveMode_OnlyToDb)]
		OnlyToDb = 3,
	}
	public enum DbAccessMode : byte
	{
		[Description(EnumResourceKeyConstants.DbAccessMode_None)]
		None = 0,

		[Description(EnumResourceKeyConstants.DbAccessMode_AdoNet)]
		AdoNet = 1,

		[Description(EnumResourceKeyConstants.DbAccessMode_EntityFramework)]
		EntityFramework = 2,
	}
	public enum DbType : byte
	{
		[Description(EnumResourceKeyConstants.DbType_MsSql)]
		MsSql = 1,

		[Description(EnumResourceKeyConstants.DbType_SQLite)]
		SQLite = 2,
	}
	public enum TransactionItemType : byte
	{
		Expense = 1,
		Income = 2,
		Both = 3,
	}
}