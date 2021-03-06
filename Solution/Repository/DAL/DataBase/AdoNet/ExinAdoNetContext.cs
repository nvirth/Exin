﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using Common.Configuration;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.Utils;
using DAL.RepoCommon;
using Localization;
using DbType = Common.DbType;
using C = Common.Configuration.Constants.Db;

namespace DAL.DataBase.AdoNet
{
	public static class ExinAdoNetContextFactory
	{
		public static ExinAdoNetContextBase Create(IRepoConfiguration repoConfiguration)
		{
			switch(repoConfiguration.DbType)
			{
				case DbType.MsSql:
					return new ExinAdoNetContextMsSql(repoConfiguration);

				case DbType.SQLite:
					return new ExinAdoNetContextSQLite(repoConfiguration);

				default:
					throw new NotImplementedException(string.Format(Localized.ExinAdoNetContextFactory_is_not_implemented_for_this_DbType__FORMAT__, repoConfiguration.DbType));
			}
		}
	}

	public abstract class ExinAdoNetContextBase : RepoConfigurableBase, IDisposable
	{
		public IRepoConfiguration LocalConfig { get; }

		public bool IsIdentityInsertOn { get; private set; }

		public DbConnection Connection { get; set; }
		public DbCommand Command { get; set; }
		public DbDataAdapter Adapter { get; set; }
		public DataSet DataSet { get; set; }

		protected abstract void InitProperties();
		protected abstract void AfterConnectionOpened();

		protected ExinAdoNetContextBase(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
			LocalConfig = repoConfiguration;

			InitProperties();

			try
			{
				Connection.Open();
				AfterConnectionOpened();
			}
			catch(Exception e)
			{
				Log.Fatal(this,
					m => m(Localized.ResourceManager, LocalizedKeys.Could_not_open_the_database_connection),
					LogTarget.All,
					e
				);
				throw;
			}
		}

		#region Transaction

		/// <summary>
		/// It can be used in a using{...} block. No need to hold the returned Transaction instance in a 
		/// local variable; it's accessible here: ctx|this|.Command.Transaction
		/// </summary>
		public DbTransaction WithTransaction()
		{
			var transaction = this.Connection.BeginTransaction();
			this.Command.Transaction = transaction;

			return transaction;
		}

		/// <summary>
		/// Creates a DbTransaction (in using), calls ExecuteNonQuery, and then Commit
		/// </summary>
		public int ExecInTransactionWithCommit()
		{
			using(this.WithTransaction())
			{
				var res = this.Command.ExecuteNonQuery();
				this.Command.Transaction.Commit();

				return res;
			}
		}

		#endregion

		#region IdentityInsert (abstract)

		/// It creates a new IdentityInsert instance, which implements IDisposable, 
		/// and does nothing but in ctor sets identity insert on (for the specified 
		/// table), and in dtor (dispose) sets it off. 
		/// So you can use this method with 'using(...){...}' context
		public virtual IdentityInsert WithIdentityInsert(string tableName, bool? activate)
		{
			activate = activate ?? LocalConfig.DbInsertId ?? false;

			return new IdentityInsert(this, tableName, activate.Value);
		}

		/// <summary>
		/// Can insert ID values
		/// </summary>
		public virtual void SetIdentityInsertOn(string tableName)
		{
			IsIdentityInsertOn = true;
		}

		/// <summary>
		/// Can not insert ID values (default)
		/// </summary>
		public virtual void SetIdentityInsertOff(string tableName)
		{
			IsIdentityInsertOn = false;
		}

		#endregion

		#region IDisposable

		private bool _isDisposed = false;

		public void Dispose()
		{
			if(!_isDisposed)
			{
			    Connection?.Close();
			    DataSet?.Dispose();

			    GC.SuppressFinalize(this);
				_isDisposed = true;
			}
		}

		#endregion

	}

	public class ExinAdoNetContextMsSql : ExinAdoNetContextBase
	{
		public ExinAdoNetContextMsSql(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		protected override void InitProperties()
		{
			Connection = new SqlConnection(new ExinConnectionString(LocalConfig).Get);
			Command = Connection.CreateCommand();
			Adapter = new SqlDataAdapter();
			DataSet = new DataSet(C.ExinDataSet);
		}

		protected override void AfterConnectionOpened()
		{
		}

		#region IdentityInsert

		public override void SetIdentityInsertOn(string tableName)
		{
			base.SetIdentityInsertOn(tableName);

			Command.CommandText = "set identity_insert " + tableName + " on";
			Command.ExecuteNonQuery();
			Command.CommandText = "";
		}

		public override void SetIdentityInsertOff(string tableName)
		{
			base.SetIdentityInsertOff(tableName);

			Command.CommandText = "set identity_insert " + tableName + " off";
			Command.ExecuteNonQuery();
			Command.CommandText = "";
		}

		#endregion
	}

	public class ExinAdoNetContextSQLite : ExinAdoNetContextBase
	{
		// SQLite always allows identity insert...

		public ExinAdoNetContextSQLite(IRepoConfiguration repoConfiguration) : base(repoConfiguration)
		{
		}

		protected override void InitProperties()
		{
			var connectionString = new ExinConnectionString(LocalConfig).Get;

			Connection = new SQLiteConnection(connectionString);
			Command = Connection.CreateCommand();
			Adapter = new SQLiteDataAdapter();
			DataSet = new DataSet(C.ExinDataSet);
		}

		protected override void AfterConnectionOpened()
		{
			//// We have to do a commit first... otherwise, we cannot start a new transaction with "begin;"
			//// -> the error message says something that a transacion is already opened
			//// Okey, then we could write the commit explicitly to the end of all command (if we don't want to run a 
			//// real transaction) - but, if we once put a commit; we have to manually start a new "transaction",
			//// otherwise the second commit will raise an error: no transaction is opened - can not commit.
			////
			//// The best is, I could not find a solution to this on the internet - it's like only I've got this
			//// situation...
			////
			//// Best2: this did not work:  using(var tran = Connection.BeginTransaction()){...; tran.Commit();}
			//// -> Nothing happened in the database; like no commit was taken
			////
			//try
			//{
			//	Command.CommandText = " COMMIT; ";
			//	Command.ExecuteNonQuery();
			//}
			//catch(Exception e)
			//{
			//	// If we've got multiple connections at the same time, I think there belong to the same transaction.
			//	// Only the first one can be committed
			//	ExinLog.ger.LogException("SQLite - frist commit (after connection opened) failed", e);
			//}
			//finally
			//{
			//	Command.CommandText = "";
			//}
		}
	}
}
