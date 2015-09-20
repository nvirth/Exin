using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Common.Db.Entities;
using Common.Log;
using Common.UiModels.WPF.Validation;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.Xml.Settings;

namespace Common.Configuration
{
	public class RepoSettings : RepoConfiguration
	{
		public class UserSettingsXml
		{
			public int? StatYAxisMax { get; private set; }

			public static UserSettingsXml Parse(XElement xml)
			{
				try
				{
					var instance = new UserSettingsXml()
					{
						StatYAxisMax = xml.ParseIntNullable(C.StatYAxisMax),
					};
					instance.InitAndValidate();
					return instance;
				}
				catch (Exception e)
				{
					ExinLog.ger.LogException("Could not parse RepoSettings.UserSettingsXml. ", e);
					throw;
				}
			}

			private void InitAndValidate()
			{
			}
		}

		public Version Version { get; private set; }
		public Version LastInitVersion { get; private set; }
		public string Currency { get; private set; }

		public UserSettingsXml UserSettings { get; private set; }

		public static RepoSettings Read(string xmlFilePath)
		{
			try
			{
				var xmlDoc = XElement.Load(xmlFilePath);
				var instance = xmlDoc.Elements(C.Repo).Select(xml => {
					var versionStr = xml.ParseString(C.Version);
					var lastInitVersionStr = xml.ParseString(C.LastInitVersion);
					var dbAccessMode = xml.ParseString(C.DbAccessMode);
					var dbType = xml.ParseString(C.DbType);
					var readMode = xml.ParseString(C.ReadMode);
					var saveMode = xml.ParseString(C.SaveMode);

					var result = new RepoSettings
					{
						Version = Version.Parse(versionStr),
						LastInitVersion = Version.Parse(lastInitVersionStr),
						Currency = xml.ParseString(C.Currency),
						UserSettings = UserSettingsXml.Parse(xml.Element(C.UserSettings)),
						DbAccessMode = EnumHelpers.Parse<DbAccessMode>(dbAccessMode, ignoreCase: true),
						DbType = EnumHelpers.Parse<DbType>(dbType, ignoreCase: true),
						ReadMode = EnumHelpers.Parse<ReadMode>(readMode, ignoreCase: true),
						SaveMode = EnumHelpers.Parse<SaveMode>(saveMode, ignoreCase: true),
					};
					return result;
				})
				.First();

				instance.Validate();
				return instance;
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException("Could not read the RepoSettings.xml file. Location: {0}".Formatted(xmlFilePath), e);
				throw;
			}
		}

		private void Validate()
		{
			if(!Currenies.IsValid(Currency))
				throw new ConfigurationErrorsException("Invalid currency: {0}. The available ones: {1}".Formatted(Currency, Currenies.Available.Join("; ")));

			if(ReadMode == ReadMode.FromDb && SaveMode == SaveMode.OnlyToFile)
				throw new ConfigurationErrorsException(Localized.Configuration_error__can_t_use_ReadMode_FromDb_and_SaveMode_OnlyToFile_together__);
			if(ReadMode == ReadMode.FromFile && SaveMode == SaveMode.OnlyToDb)
				throw new ConfigurationErrorsException(Localized.Configuration_error__can_t_use_ReadMode_FromFile_and_SaveMode_OnlyToDb_together__);

			if(LastInitVersion > Version)
				throw new ConfigurationErrorsException("LastInitVersion > Version");
		}
	}
}