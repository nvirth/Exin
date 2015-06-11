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
		private static readonly Dictionary<Tuple<Type, Type>, Action<object>> _switch = new Dictionary<Tuple<Type, Type>, Action<object>>
		{
			//---------------------------TSource------------TDestination----------------------Action---

			// -- SQLite
			{new Tuple<Type, Type>(typeof(Category), typeof(Category_Sqlite)), (sourceProxy) => Category_Sqlite.InitAutoMapper(sourceProxy as Category)},
			{new Tuple<Type, Type>(typeof(SummaryItem), typeof(SummaryItem_Sqlite)), (sourceProxy) => SummaryItem_Sqlite.InitAutoMapper(sourceProxy as SummaryItem)},
			{new Tuple<Type, Type>(typeof(TransactionItem), typeof(TransactionItem_Sqlite)), (sourceProxy) => TransactionItem_Sqlite.InitAutoMapper(sourceProxy as TransactionItem)},
			{new Tuple<Type, Type>(typeof(Unit), typeof(Unit_Sqlite)), (sourceProxy) => Unit_Sqlite.InitAutoMapper(sourceProxy as Unit)},

			{new Tuple<Type, Type>(typeof(Category_Sqlite), typeof(Category)), (sourceProxy) => Category_Sqlite.InitAutoMapper()},
			{new Tuple<Type, Type>(typeof(Unit_Sqlite), typeof(Unit)), (sourceProxy) => Unit_Sqlite.InitAutoMapper()},

			// -- MS SQL
			{new Tuple<Type, Type>(typeof(Category), typeof(Category_MsSql)), (sourceProxy) => Category_MsSql.InitAutoMapper(sourceProxy as Category)},
			{new Tuple<Type, Type>(typeof(SummaryItem), typeof(SummaryItem_MsSql)), (sourceProxy) => SummaryItem_MsSql.InitAutoMapper(sourceProxy as SummaryItem)},
			{new Tuple<Type, Type>(typeof(TransactionItem), typeof(TransactionItem_MsSql)), (sourceProxy) => TransactionItem_MsSql.InitAutoMapper(sourceProxy as TransactionItem)},
			{new Tuple<Type, Type>(typeof(Unit), typeof(Unit_MsSql)), (sourceProxy) => Unit_MsSql.InitAutoMapper(sourceProxy as Unit)},

			{new Tuple<Type, Type>(typeof(Category_MsSql), typeof(Category)), (sourceProxy) => Category_MsSql.InitAutoMapper()},
			{new Tuple<Type, Type>(typeof(Unit_MsSql), typeof(Unit)), (sourceProxy) => Unit_MsSql.InitAutoMapper()},
		};

		public static void Init<TSource, TDestination>(TSource sourceProxy = default(TSource))
		{
			var key = new Tuple<Type, Type>(typeof(TSource), typeof(TDestination));

			if(_switch.ContainsKey(key))
			{
				var doSwitch = _switch[key];

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
