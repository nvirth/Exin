﻿using System;
using Common.Configuration;
using Common.Log;

namespace DAL.RepoCommon
{
	public interface IRepoConfigurableBase
	{
		IRepoConfiguration LocalConfig { get; }
	}

	public class RepoConfigurableBase : IRepoConfigurableBase
	{
		public IRepoConfiguration LocalConfig { get; }

		public RepoConfigurableBase(IRepoConfiguration repoConfiguration)
		{
			var mutableConfig = repoConfiguration as RepoConfiguration ?? new RepoConfiguration();
			repoConfiguration = repoConfiguration ?? mutableConfig;

			mutableConfig.DbType = repoConfiguration.DbType == 0 ? Config.Repo.DbType : repoConfiguration.DbType;
			mutableConfig.DbAccessMode = repoConfiguration.DbAccessMode == 0 ? Config.Repo.DbAccessMode : repoConfiguration.DbAccessMode;
			mutableConfig.ReadMode = repoConfiguration.ReadMode == 0 ? Config.Repo.ReadMode : repoConfiguration.ReadMode;
			mutableConfig.SaveMode = repoConfiguration.SaveMode == 0 ? Config.Repo.SaveMode : repoConfiguration.SaveMode;

			LocalConfig = mutableConfig;
		}
	}
}
