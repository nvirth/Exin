﻿using System;
using System.Configuration;
using Common.Log;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.AppSettingsKeys;

namespace Common.Configuration
{
	public interface IRepoConfiguration : IEquatable<IRepoConfiguration>
	{
		DbType DbType { get; }
		DbAccessMode DbAccessMode { get; }
		ReadMode ReadMode { get; }
		SaveMode SaveMode { get; }
	}

	public class RepoConfiguration : IRepoConfiguration
	{
		public DbType DbType { get; set; } = 0;
		public DbAccessMode DbAccessMode { get; set; } = 0;
		public ReadMode ReadMode { get; set; } = 0;
		public SaveMode SaveMode { get; set; } = 0;

		#region Equality members

		public bool Equals(IRepoConfiguration other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return
				DbType == other.DbType &&
				DbAccessMode == other.DbAccessMode &&
				ReadMode == other.ReadMode &&
				SaveMode == other.SaveMode;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((RepoConfiguration) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) DbType;
				hashCode = (hashCode*397) ^ (int) DbAccessMode;
				hashCode = (hashCode*397) ^ (int) ReadMode;
				hashCode = (hashCode*397) ^ (int) SaveMode;
				return hashCode;
			}
		}

		public static bool operator ==(RepoConfiguration left, RepoConfiguration right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(RepoConfiguration left, RepoConfiguration right)
		{
			return !Equals(left, right);
		}

		#endregion
	}

	public static class Config
	{
		public const int CategoryValidFrom = 100;
		public const int UnitValidFrom = 100;
		public static readonly object DbStringNull = ""; // Other choice: DBNull.Value
		public const string FileExtension = "xml";

		public static IRepoConfiguration Repo { get; } = new RepoConfiguration()
		{
			DbAccessMode = EnumHelpers.Parse<DbAccessMode>(ConfigurationManager.AppSettings[C.DbAccessMode], ignoreCase: true),
			DbType = EnumHelpers.Parse<DbType>(ConfigurationManager.AppSettings[C.DbType], ignoreCase: true),
			ReadMode = EnumHelpers.Parse<ReadMode>(ConfigurationManager.AppSettings[C.ReadMode], ignoreCase: true),
			SaveMode = EnumHelpers.Parse<SaveMode>(ConfigurationManager.AppSettings[C.SaveMode], ignoreCase: true),
		};

		static Config()
		{
			// TODO it would be better if only SaveMode.FileAndDb could be used in ExinWPF

			if(Repo.ReadMode == ReadMode.FromDb && Repo.SaveMode == SaveMode.OnlyToFile)
			{
				string msg = Localized.Configuration_error__can_t_use_ReadMode_FromDb_and_SaveMode_OnlyToFile_together__;
				ExinLog.ger.LogError(msg);
				throw new Exception(msg);
			}
			if(Repo.ReadMode == ReadMode.FromFile && Repo.SaveMode == SaveMode.OnlyToDb)
			{
				string msg = Localized.Configuration_error__can_t_use_ReadMode_FromFile_and_SaveMode_OnlyToDb_together__;
				ExinLog.ger.LogError(msg);
				throw new Exception(msg);
			}
		}
	}
}
