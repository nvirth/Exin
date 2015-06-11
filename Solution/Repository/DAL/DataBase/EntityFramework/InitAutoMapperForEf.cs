using System;
using System.Collections.Generic;
using Common.Db.Entities;
using Common.Utils;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;

namespace DAL.DataBase.EntityFramework
{
	public static class InitAutoMapperForEf
	{
		private static readonly Dictionary<Type, Action<object>> _switch = new Dictionary<Type, Action<object>>
		{
			// -- SQLite
			{typeof (Category_Sqlite), (sourceProxy) => Category_Sqlite.InitAutoMapper(sourceProxy as Category)},
			{typeof (SummaryItem_Sqlite), (sourceProxy) => SummaryItem_Sqlite.InitAutoMapper(sourceProxy as SummaryItem)},
			{typeof (TransactionItem_Sqlite), (sourceProxy) => TransactionItem_Sqlite.InitAutoMapper(sourceProxy as TransactionItem)},
			{typeof (Unit_Sqlite), (sourceProxy) => Unit_Sqlite.InitAutoMapper(sourceProxy as Unit)},

			// -- MS SQL
			{typeof (Category_MsSql), (sourceProxy) => Category_MsSql.InitAutoMapper(sourceProxy as Category)},
			{typeof (SummaryItem_MsSql), (sourceProxy) => SummaryItem_MsSql.InitAutoMapper(sourceProxy as SummaryItem)},
			{typeof (TransactionItem_MsSql), (sourceProxy) => TransactionItem_MsSql.InitAutoMapper(sourceProxy as TransactionItem)},
			{typeof (Unit_MsSql), (sourceProxy) => Unit_MsSql.InitAutoMapper(sourceProxy as Unit)},
		};

		// NOTE: In theory, all the used types are initialized in StaticInitializer.cs
		// This (and all calls to this) is here for safety sake
		public static void Init<TSource, TDestination>(TSource sourceProxy = default(TSource))
		{
			if(_switch.ContainsKey(typeof(TDestination)))
			{
				var doSwitch = _switch[typeof(TDestination)];

				if(Equals(sourceProxy, default(TSource)))
					doSwitch(null);
				else
					doSwitch(sourceProxy);
			}
			else // default case in switch
			{
				bool wasNew;
				AutoMapperInitializer<TSource, TDestination>
					.InitializeIfNeeded(out wasNew)
					.IgnoreAllNonExisting<TSource, TDestination>(wasNew);
			}
		}
	}
}
