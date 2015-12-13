﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Common.Log;
using Common.UiModels.WPF.Base;
using Common.UiModels.WPF.Validation;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.Xml.Settings;

namespace Common.Configuration.Settings
{
	public class MainSettings : XmlSettingsBase
	{
		public const string DefaultRepoName = "Default";
		public const string DefaultRootDir = @".\Repositories\" + RepoNamePlaceholder;
		public const string RepoNamePlaceholder = ":REPO_NAME:";

		public class RepoXml : NotifyPropertyChanged
		{
			#region Leaf xml properties

			private string _name;
			public string Name
			{
				get { return _name; }
				set
				{
					if(_name == value)
						return;

					_name = value;
					_xElement.Element(C.Name).SetValue(value);
					OnPropertyChanged();
				}
			}

			private string _rootDir;
			public string RootDir
			{
				get { return _rootDir; }
				set
				{
					if(_rootDir == value)
						return;

					_rootDir = value;
					_xElement.Element(C.RootDir).SetValue(value);
					OnPropertyChanged();
				}
			}

			#endregion

			private readonly XElement _xElement;

			public RepoXml(XElement xElement)
			{
				_xElement = xElement;
			}

			public static RepoXml Parse(XElement xml)
			{
				try
				{
					var instance = new RepoXml(xml)
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

					// TODO Starting problem: If a new, virgin app starts, it will crash on this
					//if(!Directory.Exists(RootDir))
					//	throw new DirectoryNotFoundException(RootDir);
				}
			}
		}
		public class UserSettingsXml : NotifyPropertyChanged
		{
			public const char CurrentRepoNamesSeparator = ',';

			#region Leaf xml properties

			private string[] _currentRepoNames;
			public string[] CurrentRepoNames
			{
				get { return _currentRepoNames; }
				set
				{
					if(_currentRepoNames == value)
						return;

					_currentRepoNames = value;
					_xElement.Element(C.CurrentRepoNames).SetValue(value);
					OnPropertyChanged();
				}
			}

			private bool _allowsFutureDate;
			public bool AllowsFutureDate
			{
				get { return _allowsFutureDate; }
				set
				{
					if(_allowsFutureDate == value)
						return;

					_allowsFutureDate = value;
					_xElement.Element(C.AllowsFutureDate).SetValue(value);
					OnPropertyChanged();
				}
			}

			private CultureInfo _language;
			public CultureInfo Language
			{
				get { return _language; }
				set
				{
					if(_language == value)
						return;

					_language = value;
					_xElement.Element(C.Language).SetValue(value);
					OnPropertyChanged();
				}
			}

			private CopyFormat _copyFormat;
			public CopyFormat CopyFormat
			{
				get { return _copyFormat; }
				set
				{
					if(_copyFormat == value)
						return;

					_copyFormat = value;
					_xElement.Element(C.CopyFormat).SetValue(value);
					OnPropertyChanged();
				}
			}

			#endregion

			private readonly XElement _xElement;

			public UserSettingsXml(XElement xElement)
			{
				_xElement = xElement;
			}

			public static UserSettingsXml Parse(XElement xml)
			{
				try
				{
					var languageStr = xml.ParseString(C.Language);
					var copyFormatStr = xml.ParseString(C.CopyFormat);

					var instance = new UserSettingsXml(xml)
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

		#region Leaf xml properties

		private Version _version;
		public Version Version
		{
			get { return _version; }
			set
			{
				if(_version == value)
					return;

				_version = value;
				XElement.Element(C.Version).SetValue(value);
				OnPropertyChanged();
			}
		}

		private Version _lastInitVersion;
		public Version LastInitVersion
		{
			get { return _lastInitVersion; }
			set
			{
				if(_lastInitVersion == value)
					return;

				_lastInitVersion = value;
				XElement.Element(C.LastInitVersion).SetValue(value);
				OnPropertyChanged();
			}
		}

		#endregion

		public List<RepoXml> Repositories { get; private set; }
		public UserSettingsXml UserSettings { get; private set; }

		private MainSettings(string xmlFilePath, XElement xmlDoc, XElement xElement) : base(xmlFilePath, xmlDoc, xElement)
		{
		}

		public static MainSettings Read(string xmlFilePath)
		{
			try
			{
				var xmlDoc = XElement.Load(xmlFilePath);
				var instance = xmlDoc.Elements(C.App).Select(xml => {
					var versionStr = xml.ParseString(C.Version);
					var lastInitVersionStr = xml.ParseString(C.LastInitVersion);

					var result = new MainSettings(xmlFilePath, xmlDoc, xml)
					{
						Version = Version.Parse(versionStr),
						LastInitVersion = Version.Parse(lastInitVersionStr),
						UserSettings = UserSettingsXml.Parse(xml.Element(C.UserSettings)),
						Repositories = xml.Element(C.Repositories).Elements(C.Repo).Select(RepoXml.Parse).ToList(),
					};
					return result;
				})
				.First();

				instance.InitAndValidate();
				return instance;
			}
			catch (Exception e)
			{
				ExinLog.ger.LogException("Could not read the MainSettings.xml file. Location: {0}".Formatted(xmlFilePath), e);
				throw;
			}
		}

		private void InitAndValidate()
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

			if(LastInitVersion > Version)
				throw new ConfigurationErrorsException("LastInitVersion > Version");

			// --

			UserSettings.PropertyChanged += (sender, args) => OnPropertyChanged(C.UserSettings);
			Repositories.ForEach(repoXml => repoXml.PropertyChanged += (sender, args) => OnPropertyChanged(C.Repositories));
            Init();
		}
	}
}