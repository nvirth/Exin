using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Exin.Common.Logging.Core;
using Common.UiModels.WPF.Base;
using Common.UiModels.WPF.Validation;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.Xml.Settings;
using LogLevel = Common.Logging.LogLevel;

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
					RootDirAbs = value;
					_xElement.Element(C.RootDir).SetValue(value);
					OnPropertyChanged();
				}
			}

			private string _rootDirAbs;
			public string RootDirAbs
			{
				get { return _rootDirAbs; }
				private set
				{
					if(_rootDirAbs == value)
						return;

					var fullPath = value;
					if(!Path.IsPathRooted(value))
						fullPath = Path.GetFullPath(Path.Combine(Config.AppExecDir, value));

					//_rootDirAbs = Path.GetFullPath(fullPath);
					_rootDirAbs = fullPath;
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
					Log.Fatal(typeof(RepoXml),
						m => m(Localized.ResourceManager, LocalizedKeys.Could_not_parse_MainSettings_RepoXml__or_some_values_are_invalid__),
						LogTarget.All,
						e
					);
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
					Directory.CreateDirectory(RootDirAbs);
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
					Log.Fatal(typeof(UserSettingsXml),
						m => m(Localized.ResourceManager, LocalizedKeys.Could_not_parse_MainSettings_UserSettingsXml__),
						LogTarget.All,
						e
					);
					throw;
				}
			}

			private void InitAndValidate()
			{
				if(CurrentRepoNames.Length == 1 && string.IsNullOrWhiteSpace(CurrentRepoNames[0]))
					CurrentRepoNames[0] = DefaultRepoName;
			}
		}
		public class LoggingXml : NotifyPropertyChanged
		{
			#region Leaf xml properties

			private LogLevel _uiLoggerLevel;
			public LogLevel UiLoggerLevel
			{
				get { return _uiLoggerLevel; }
				set
				{
					if(_uiLoggerLevel == value)
						return;

					_uiLoggerLevel = value;
					_xElement.Element(C.UiLoggerLevel).SetValue(value);
					OnPropertyChanged();
				}
			}

			private LogLevel _logLoggerLevel;
			public LogLevel LogLoggerLevel
			{
				get { return _logLoggerLevel; }
				set
				{
					if(_logLoggerLevel == value)
						return;

					_logLoggerLevel = value;
					_xElement.Element(C.LogLoggerLevel).SetValue(value);
					OnPropertyChanged();
				}
			}

			#endregion

			private readonly XElement _xElement;

			public LoggingXml(XElement xElement)
			{
				_xElement = xElement;
			}

			public static LoggingXml Parse(XElement xml)
			{
				try
				{
					var uiLoggerLevelStr = xml.ParseString(C.UiLoggerLevel);
					var logLoggerLevelStr = xml.ParseString(C.LogLoggerLevel);

					var instance = new LoggingXml(xml) {
						UiLoggerLevel = EnumHelpers.Parse<LogLevel>(uiLoggerLevelStr, ignoreCase: true),
						LogLoggerLevel = EnumHelpers.Parse<LogLevel>(logLoggerLevelStr, ignoreCase: true),
					};
					instance.InitAndValidate();
					return instance;
				}
				catch(Exception e)
				{
					Log.Fatal(typeof(LoggingXml),
						m => m(Localized.ResourceManager, LocalizedKeys.Parsing_error__),
						LogTarget.All,
						e
					);
					throw;
				}
			}

			private void InitAndValidate()
			{
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
		public LoggingXml Logging { get; private set; }

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
						Logging = LoggingXml.Parse(xml.Element(C.Logging)),
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
				Log.Fatal(typeof(MainSettings),
					m => m(Localized.ResourceManager, LocalizedKeys.Could_not_read_the_MainSettings_xml_file__Location___0_, xmlFilePath),
					LogTarget.All,
					e
				);
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