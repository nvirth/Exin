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
	public enum CopyFormat
	{
		Xml, Json
	}

	public class MainSettings
	{
		public const string DefaultRepoName = "Default";
		public const string DefaultRootDir = @".\Repositories\" + RepoNamePlaceholder;
		public const string RepoNamePlaceholder = ":REPO_NAME:";

		public class RepoXml
		{
			public string Name { get; private set; }
			public string RootDir { get; private set; }

			public static RepoXml Parse(XElement xml)
			{
				try
				{
					var instance = new RepoXml()
					{
						Name = xml.ParseString(C.Name),
						RootDir = xml.ParseString(C.RootDir),
					};
					instance.InitAndValidate();
					return instance;
				}
				catch (Exception e)
				{
					ExinLog.ger.LogException("Could not parse MainSettings.RepoXml, or some values are invalid. ", e);
					throw;
				}
			}

			private void InitAndValidate()
			{
				var isRootEmpty = string.IsNullOrWhiteSpace(RootDir);
				var isNameEmpty = string.IsNullOrWhiteSpace(Name);
				if (isRootEmpty && isNameEmpty)
				{
#if DEBUG
					//var rootDir = ProjectInfos.GetSolutionsRootDircetory(Config.AppName);
					RootDir = Config.FileRepoDeveloperPath;
					isRootEmpty = false;
#endif
				}

				//--Name
				if(isNameEmpty)
					Name = DefaultRepoName;

				var isValid = Regex.IsMatch(Name, ValidationConstants.RegEx_START_EnChar_EnCharOrDigit_0Ti_END);
				if (!isValid)
					throw new ConfigurationErrorsException("RepoName: " + Localized.ResourceManager.GetString(ValidationConstants.RegExErrMsg_START_EnChar_EnCharOrDigit_0Ti_END));

				//--RootDir
				if(isRootEmpty) // Possibly the first start of the app
				{
					RootDir = DefaultRootDir.Replace(RepoNamePlaceholder, Name);
					Directory.CreateDirectory(RootDir);
				}
				else
				{
					RootDir = RootDir.Replace(RepoNamePlaceholder, Name);

					if(!Directory.Exists(RootDir))
						throw new DirectoryNotFoundException(RootDir);
				}
			}
		}
		public class UserSettingsXml
		{
			public const char CurrentRepoNamesSeparator = ',';

			public string[] CurrentRepoNames { get; private set; }
			public bool AllowsFutureDate { get; private set; }
			public CultureInfo Language { get; private set; }
			public CopyFormat CopyFormat { get; private set; }

			public static UserSettingsXml Parse(XElement xml)
			{
				try
				{
					var languageStr = xml.ParseString(C.Language);
					var copyFormatStr = xml.ParseString(C.CopyFormat);

					var instance = new UserSettingsXml()
					{
						CurrentRepoNames = xml.ParseString(C.CurrentRepoNames).ToLowerInvariant().Split(CurrentRepoNamesSeparator),
						AllowsFutureDate = xml.ParseBool(C.AllowsFutureDate),
						Language = Cultures.Parse(languageStr),
						CopyFormat = EnumHelpers.Parse<CopyFormat>(copyFormatStr, ignoreCase: true),
					};
					instance.InitAndValidate();
					return instance;
				}
				catch (Exception e)
				{
					ExinLog.ger.LogException("Could not parse MainSettings.UserSettingsXml. ", e);
					throw;
				}
			}

			private void InitAndValidate()
			{
				if(CurrentRepoNames.Length == 1 && string.IsNullOrWhiteSpace(CurrentRepoNames[0]))
					CurrentRepoNames[0] = DefaultRepoName;
			}
		}

		public string Version { get; private set; }
		public string LastInitVersion { get; private set; }

		public List<RepoXml> Repositories { get; private set; }
		public UserSettingsXml UserSettings { get; private set; }

		public static MainSettings Read(string xmlFilePath)
		{
			try
			{
				var xmlDoc = XElement.Load(xmlFilePath);
				var instance = xmlDoc.Elements(C.App).Select(xml => new MainSettings
				{
					Version = xml.ParseString(C.Version),
					LastInitVersion = xml.ParseString(C.LastInitVersion),
					UserSettings = UserSettingsXml.Parse(xml.Element(C.UserSettings)),
					Repositories = xml.Element(C.Repositories).Elements(C.Repo).Select(RepoXml.Parse).ToList(),
				})
				.First();

				instance.Validate();
				return instance;
			}
			catch (Exception e)
			{
				ExinLog.ger.LogException("Could not read the MainSettings.xml file. Location: {0}".Formatted(xmlFilePath), e);
				throw;
			}
		}

		private void Validate()
		{
			var repoNames = Repositories.Select(r => r.Name.ToLowerInvariant()).Distinct().ToList();

			var areRepoNamesUnique = repoNames.Count == Repositories.Count;
			if(!areRepoNamesUnique)
			{
				var duplicates = Repositories.GroupBy(r => r.Name).Where(g => g.Count() > 1).Select(g => g.Key).Join("; ");
				throw new ConfigurationErrorsException("The names of the repositories must be unique. These are duplicated: {0}".Formatted(duplicates));
			}

            //UserSettings.CurrentRepoNames.Any(repoName => !repoNames.Contains(repoName));
			var brokenRepoNames = UserSettings.CurrentRepoNames.Select(name => name.ToLowerInvariant()).Except(repoNames).ToList();
			if (brokenRepoNames.Any())
			{
				var msg = "These CurrentRepository values point to non-existing repo(s): {0}".Formatted(brokenRepoNames.Join("; "));
                throw new ConfigurationErrorsException(msg);
			}
		}
	}
	public static class MainSettingsHelpers // TODO move and use this
	{
		public static string ParseString(this XElement xml, string xName)
		{
			var element = xml.Element(xName);
			var result = ((string) element).Trim();
			return result;
		}
		public static bool ParseBool(this XElement xml, string xName)
		{
			var element = xml.Element(xName);
			var result = (bool)element;
			return result;
		}
	}
}