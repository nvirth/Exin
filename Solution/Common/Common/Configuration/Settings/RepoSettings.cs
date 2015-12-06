using System;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using Common.Log;
using Common.UiModels.WPF.Base;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.Xml.Settings;

namespace Common.Configuration.Settings
{
	public class RepoSettings : XmlSettingsBase
	{
		public class UserSettingsXml : NotifyPropertyChanged
		{
			#region Leaf xml properties

			private int? _statYAxisMax;
			public int? StatYAxisMax
			{
				get { return _statYAxisMax; }
				set
				{
					if(_statYAxisMax == value)
						return;

					_statYAxisMax = value;
					_xElement.Element(C.StatYAxisMax).SetValue(value);
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
					var instance = new UserSettingsXml(xml)
					{
						_statYAxisMax = xml.ParseIntNullable(C.StatYAxisMax),
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
		public class MsSqlSettingsXml : NotifyPropertyChanged
		{
			public class ConnectionStringsXml : NotifyPropertyChanged
			{
				#region Leaf xml properties

				private string _adoNet;
				public string AdoNet
				{
					get { return _adoNet; }
					set
					{
						if(_adoNet == value)
							return;

						_adoNet = value;
						_xElement.Element(C.AdoNet).SetValue(value);
						OnPropertyChanged();
					}
				}

				private string _entityFramework;
				public string EntityFramework
				{
					get { return _entityFramework; }
					set
					{
						if(_entityFramework == value)
							return;

						_entityFramework = value;
						_xElement.Element(C.EntityFramework).SetValue(value);
						OnPropertyChanged();
					}
				}

				#endregion

				private readonly XElement _xElement;

				public ConnectionStringsXml(XElement xElement)
				{
					_xElement = xElement;
				}

				public static ConnectionStringsXml Parse(XElement xml)
				{
					try
					{
						var instance = new ConnectionStringsXml(xml) {
							AdoNet = xml.ParseString(C.AdoNet),
							EntityFramework = xml.ParseString(C.EntityFramework),
						};
						instance.InitAndValidate();
						return instance;
					}
					catch(Exception e)
					{
						ExinLog.ger.LogException("Could not parse RepoSettings.ConnectionStringsXml. ", e);
						throw;
					}
				}

				private void InitAndValidate()
				{
					// Validation (yet for the existence) of these xml members are done on the fly,
					// but only if these members are necessary
				}
				public bool ValidateAdoNet()
				{
					return !string.IsNullOrWhiteSpace(AdoNet);
				}
				public bool ValidateEntityFramework()
				{
					return !string.IsNullOrWhiteSpace(EntityFramework);
				}
			}

			public ConnectionStringsXml ConnectionStrings { get; private set; }

			private readonly XElement _xElement;

			public MsSqlSettingsXml(XElement xElement)
			{
				_xElement = xElement;
			}

			public static MsSqlSettingsXml Parse(XElement xml)
			{
				try
				{
					var instance = new MsSqlSettingsXml(xml) {
						ConnectionStrings = ConnectionStringsXml.Parse(xml.Element(C.ConnectionStrings)),
					};
					instance.InitAndValidate();
					return instance;
				}
				catch(Exception e)
				{
					ExinLog.ger.LogException("Could not parse RepoSettings.MsSqlSettingsXml. ", e);
					throw;
				}
			}

			private void InitAndValidate()
			{
				ConnectionStrings.PropertyChanged += (sender, args) => OnPropertyChanged(C.ConnectionStrings);
			}
		}

		public UserSettingsXml UserSettings { get; private set; }
		public MsSqlSettingsXml MsSqlSettings { get; private set; }

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

		private string _currency;
		public string Currency
		{
			get { return _currency; }
			set
			{
				if(_currency == value)
					return;

				_currency = value;
				XElement.Element(C.Currency).SetValue(value);
				OnPropertyChanged();
			}
		}

		private DbType _dbType;
		public DbType DbType
		{
			get { return _dbType; }
			set
			{
				if(_dbType == value)
					return;

				_dbType = value;
				XElement.Element(C.DbType).SetValue(value);
				OnPropertyChanged();
			}
		}

		private DbAccessMode _dbAccessMode;
		public DbAccessMode DbAccessMode
		{
			get { return _dbAccessMode; }
			set
			{
				if(_dbAccessMode == value)
					return;

				_dbAccessMode = value;
				XElement.Element(C.DbAccessMode).SetValue(value);
				OnPropertyChanged();
			}
		}

		private ReadMode _readMode;
		public ReadMode ReadMode
		{
			get { return _readMode; }
			set
			{
				if(_readMode == value)
					return;

				_readMode = value;
				XElement.Element(C.ReadMode).SetValue(value);
				OnPropertyChanged();
			}
		}

		private SaveMode _saveMode;
		public SaveMode SaveMode
		{
			get { return _saveMode; }
			set
			{
				if(_saveMode == value)
					return;

				_saveMode = value;
				XElement.Element(C.SaveMode).SetValue(value);
				OnPropertyChanged();
			}
		}

		#endregion

		private RepoSettings(string xmlFilePath, XElement xmlDoc, XElement xElement) : base(xmlFilePath, xmlDoc, xElement)
		{
		}

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

					var result = new RepoSettings(xmlFilePath, xmlDoc, xml)
					{
						Version = Version.Parse(versionStr),
						LastInitVersion = Version.Parse(lastInitVersionStr),
						Currency = xml.ParseString(C.Currency),
						UserSettings = UserSettingsXml.Parse(xml.Element(C.UserSettings)),
						MsSqlSettings = MsSqlSettingsXml.Parse(xml.Element(C.MsSqlSettings)),
						DbAccessMode = EnumHelpers.Parse<DbAccessMode>(dbAccessMode, ignoreCase: true),
						DbType = EnumHelpers.Parse<DbType>(dbType, ignoreCase: true),
						ReadMode = EnumHelpers.Parse<ReadMode>(readMode, ignoreCase: true),
						SaveMode = EnumHelpers.Parse<SaveMode>(saveMode, ignoreCase: true),
					};
					return result;
				})
				.First();

				instance.InitAndValidate();
				return instance;
			}
			catch(Exception e)
			{
				ExinLog.ger.LogException("Could not read the RepoSettings.xml file. Location: {0}".Formatted(xmlFilePath), e);
				throw;
			}
		}

		private void InitAndValidate()
		{
			if(!Currenies.IsValid(Currency))
				throw new ConfigurationErrorsException("Invalid currency: {0}. The available ones: {1}".Formatted(Currency, Currenies.Available.Join("; ")));

			if(ReadMode == ReadMode.FromDb && SaveMode == SaveMode.OnlyToFile)
				throw new ConfigurationErrorsException(Localized.Configuration_error__can_t_use_ReadMode_FromDb_and_SaveMode_OnlyToFile_together__);
			if(ReadMode == ReadMode.FromFile && SaveMode == SaveMode.OnlyToDb)
				throw new ConfigurationErrorsException(Localized.Configuration_error__can_t_use_ReadMode_FromFile_and_SaveMode_OnlyToDb_together__);

			if(LastInitVersion > Version)
				throw new ConfigurationErrorsException("LastInitVersion > Version");

			//--

			UserSettings.PropertyChanged += (sender, args) => OnPropertyChanged(C.UserSettings);
			MsSqlSettings.PropertyChanged += (sender, args) => OnPropertyChanged(C.MsSqlSettings);
			Init();
		}
	}
}