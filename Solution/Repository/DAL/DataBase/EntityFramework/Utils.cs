﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoMapper;
using Common;
using Common.Configuration;
using DAL.DataBase.EntityFramework.EntitiesMsSql;
using DAL.DataBase.EntityFramework.EntitiesSqlite;
using Localization;

namespace DAL.DataBase.EntityFramework
{
	public static class Utils
	{
		public static void CheckDbContext(DbContext dbContext, Type expectedType, [CallerFilePath] String callerFilePath = "")
		{
			if(dbContext.GetType() == expectedType)
				return;

			// Error: wrong type
			var callerQuasi = Path.GetFileNameWithoutExtension(callerFilePath);
			var errorMsg = string.Format(Localized._0__is_not_implemented_for_context_type___1___using_DbType_configuration___2_, callerQuasi, dbContext.GetType(), Config.DbType);
			throw new ArgumentException(errorMsg);
		}

		public static ExinEfMsSqlContext InitContextForMsSql(DbType dbType, DbAccessMode dbAccessMode)
		{
			return InitContextForMsSql(ExinEfContextFactory.Create(dbType, dbAccessMode));
		}
		public static ExinEfMsSqlContext InitContextForMsSql(DbContext dbContext)
		{
			CheckDbContext(dbContext, typeof(ExinEfMsSqlContext));
			return (ExinEfMsSqlContext)dbContext;
		}

		public static ExinEfSqliteContext InitContextForSqlite(DbType dbType, DbAccessMode dbAccessMode)
		{
			return InitContextForSqlite(ExinEfContextFactory.Create(dbType, dbAccessMode));
		}
		public static ExinEfSqliteContext InitContextForSqlite(DbContext dbContext)
		{
			CheckDbContext(dbContext, typeof(ExinEfSqliteContext));
			return (ExinEfSqliteContext)dbContext;
		}

		public static IEnumerable<TDestination> ExecRead<TSource, TDestination>(IQueryable<TSource> readQuery)
		{
			InitAutoMapperForEf.Init<TSource, TDestination>();
			var result = readQuery
				.ToList() // query exec
				.Select(source => Mapper.Map<TDestination>(source));
			//.ToList(); // transform exec

			return result;
		}

		/// <param name="dbSet">E.g. ctx.Category</param>
		/// <param name="dbContext">If set, calls .SaveChanges on it</param>
		/// <returns>The transformed object, which was added by the query</returns>
		public static TDestination ExecAdd<TSource, TDestination>(DbSet<TDestination> dbSet, TSource sourceObj, DbContext dbContext = null)
			where TDestination : class
		{
			InitAutoMapperForEf.Init<TSource, TDestination>(sourceObj);
			var entityObj = Mapper.Map<TDestination>(sourceObj);

			dbSet.Add(entityObj);
			if(dbContext != null)
				dbContext.SaveChanges();

			return entityObj;
		}

		/// <param name="dbSet">E.g. ctx.Category</param>
		/// <param name="dbContext">If set, calls .SaveChanges on it</param>
		/// <returns>The transformed object, which was added by the query</returns>
		public static IEnumerable<TDestination> ExecAddRange<TSource, TDestination>(DbSet<TDestination> dbSet, IEnumerable<TSource> sourceObjects, DbContext dbContext = null)
			where TDestination : class
		{
			InitAutoMapperForEf.Init<TSource, TDestination>(sourceObjects.FirstOrDefault());
			var entityObjects = sourceObjects.Select(sourceObj => Mapper.Map<TDestination>(sourceObj));

			dbSet.AddRange(entityObjects);
			if(dbContext != null)
				dbContext.SaveChanges();

			return entityObjects;
		}

		/// <param name="dbSet">E.g. ctx.Category</param>
		/// <param name="dbContext">THIS VERSION DO NOT CALL SAVECHANGES</param>
		/// <returns>The transformed object, which was added by the query</returns>
		public static TDestination ExecUpdate<TSource, TDestination>(DbSet<TDestination> dbSet, TSource sourceObj, DbContext dbContext)
			where TDestination : class
		{
			InitAutoMapperForEf.Init<TSource, TDestination>(sourceObj);
			var entityObj = Mapper.Map<TDestination>(sourceObj);

			dbSet.Attach(entityObj);
			dbContext.Entry(entityObj).State = EntityState.Modified;

			return entityObj;
		}

		/// <param name="dbSet">E.g. ctx.Category</param>
		/// <param name="dbContext">THIS VERSION DO NOT CALL SAVECHANGES</param>
		/// <returns>The transformed object, which was added by the query</returns>
		public static TDestination ExecUpdate<TSource, TDestination>(DbSet<TDestination> dbSet, TSource sourceObj, DbContext dbContext, out int numOfAffectedRecords)
			where TDestination : class
		{
			var entityObj = ExecUpdate(dbSet, sourceObj, dbContext);

			numOfAffectedRecords = dbContext.SaveChanges();

			return entityObj;
		}
	}
}
