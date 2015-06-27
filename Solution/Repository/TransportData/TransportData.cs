using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Common;
using Common.Configuration;
using Common.Log;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;
using C = Common.Configuration.Constants.TransportData;

namespace TransportData
{
	class ParsedArgs
	{
		private static readonly string[] FromToPossibleValuesUpper = new[] { C.FILEREPO, C.DB_MSSQL, C.DB_SQLITE };
		private static readonly string[] LangPossibleValuesUpper = new[] { C.EN, C.HU };

		public List<string> Errors { get; private set; }

		public ParsedArgs(string[] args)
		{
			RawArgs = args;
			Errors = new List<string>(0);

			Lang = C.EN;
			Help = false;

			ParseCommandLineArguments();
			Errors = Errors.Distinct().ToList();
		}

		#region Parse Args

		private string _from;
		public string From
		{
			get { return _from; }
			private set
			{
				if (FromToPossibleValuesUpper.Contains(value))
					_from = value;
				else
					AddValueError();
			}
		}

		private string _to;
		public string To
		{
			get { return _to; }
			private set
			{
				if (FromToPossibleValuesUpper.Contains(value))
					_to = value;
				else
					AddValueError();
			}
		}

		private string _lang;
		public string Lang
		{
			get { return _lang; }
			private set
			{
				if (LangPossibleValuesUpper.Contains(value))
					_lang = value;
				else
					AddValueError();
			}
		}

		public bool Help { get; private set; }

		public string[] RawArgs { get; private set; }

		#endregion
		
		#region Parse Methods

		private void ParseCommandLineArguments()
		{
			for (int i = 0; i < RawArgs.Length; i++)
			{
				var key = ReadArg(i);
				//key = null; // todo test
				switch (key)
				{
					case "-F":
					case "--FROM":
						From = ReadArg(i + 1);
						i++;
						break;
					case "-T":
					case "--TO":
						To = ReadArg(i + 1);
						i++;
						break;
					case "-L":
					case "--LANG":
						Lang = ReadArg(i + 1);
						i++;
						break;
					case "-H":
					case "--HELP":
						Help = true;
						break;
					default:
						AddKeyError(key);
						break;
				}
			}
			ValidateParsedArguments();
		}

		private void ValidateParsedArguments()
		{
			if (From == null)
				AddKeyError(this.Property(x => x.From));
			if (To == null)
				AddKeyError(this.Property(x => x.To));

			if(From == To)
				Errors.Add("- The args From and To are the same");
			if((From == C.DB_MSSQL && To == C.DB_SQLITE) || (From == C.DB_SQLITE && To == C.DB_MSSQL))
				Errors.Add("- The args From and To are both DB types. This kind of operation is not implemented yet."); //TODO
		}

		private string ReadArg(int idx)
		{
			if (idx >= RawArgs.Length)
				return null;

			return RawArgs[idx].Trim().ToUpperInvariant();
		}

		private void AddKeyError(string argName = null)
		{
			Errors.Add("- Could not recognize this argument: " + argName);
		}

		private void AddValueError([CallerMemberName] string argName = null)
		{
			Errors.Add("- Could not recognize the given value for this argument: " + argName);
		}

		#endregion

		private RepoConfiguration _repoConfiguration;

		public RepoConfiguration ProcessArgs()
		{
			if (_repoConfiguration != null)
				return _repoConfiguration;

			if(Errors.Count != 0)
			{
				const string msg = "ParsedArgs.ProcessArgs: This method must not be called when Errors is not empty. ";
				throw ExinLog.ger.LogException(msg, new InvalidOperationException(msg));
			}

			_repoConfiguration = new RepoConfiguration();

			switch(From)
			{
				case C.FILEREPO:
					_repoConfiguration.ReadMode = ReadMode.FromFile;
					break;
				case C.DB_MSSQL:
					_repoConfiguration.ReadMode = ReadMode.FromDb;
					_repoConfiguration.DbType = DbType.MsSql;
					break;
				case C.DB_SQLITE:
					_repoConfiguration.ReadMode = ReadMode.FromDb;
					_repoConfiguration.DbType = DbType.SQLite;
					break;
			}
			switch(To)
			{
				case C.FILEREPO:
					_repoConfiguration.SaveMode = SaveMode.OnlyToFile;
					break;
				case C.DB_MSSQL:
					_repoConfiguration.SaveMode = SaveMode.OnlyToDb;
					_repoConfiguration.DbType = DbType.MsSql;
					break;
				case C.DB_SQLITE:
					_repoConfiguration.SaveMode = SaveMode.OnlyToDb;
					_repoConfiguration.DbType = DbType.SQLite;
					break;
			}
			switch(Lang)
			{
				case C.EN:
					Cultures.SetToEnglish();
                    break;
				case C.HU:
					Cultures.SetToHungarian();
					break;
			}

			// (Help should be already processed at this)

			return _repoConfiguration;
		}
	}

	/// <summary>
	/// Imports data from the file repository to the database. <para />
	/// See <see cref="PrintCliUsage"/> fn for command line usage details.
	/// </summary>
	public static class TransportData
	{
		public static void Main(string[] args)
		{
			//TODO register unhandled exception handler?
			//TODO console.readyKey -> only if Debugger.IsAttached

			//StartDebugger();
			MessagePresenterManager.WireToConsole();

			#region Parsing args

			var parsedArgs = new ParsedArgs(args);
			if (parsedArgs.Help)
				PrintCliUsageAndExit();
			if (parsedArgs.Errors.Count != 0)
				PrintArgumentErrorsAndExit(parsedArgs);

			var repoConfiguration = parsedArgs.ProcessArgs();
			if (parsedArgs.Errors.Count != 0)
				PrintArgumentErrorsAndExit(parsedArgs);

			#endregion

			repoConfiguration.DbAccessMode = DbAccessMode.AdoNet;

			// TODO test new solution for "ImportDataToDb"
			if(repoConfiguration.ReadMode == ReadMode.FromFile)
				new TransportData_FromFile_ToDb(repoConfiguration.DbType).DoWork();
			else if(repoConfiguration.ReadMode == ReadMode.FromDb) // TODO implement
				new TransportData_Worker(repoConfiguration).DoWork();

			MessagePresenter.Instance.WriteLine(Localized.Press_any_key_to_continue_);
			Console.ReadKey();
		}

		private static void PrintArgumentErrorsAndExit(ParsedArgs parsedArgs)
		{
			PrintArgumentErrors(parsedArgs);

			Console.ReadKey();
			Environment.Exit(1);
		}

		private static void PrintCliUsageAndExit()
		{
			PrintCliUsage();

			Console.ReadKey();
			Environment.Exit(0);
		}

		private static void PrintArgumentErrors(ParsedArgs parsedArgs)
		{
			MessagePresenter.Instance.WriteError(parsedArgs.Errors.Join(Environment.NewLine));
			MessagePresenter.Instance.WriteLine();
			MessagePresenter.Instance.WriteLine("Type in 'TransportData --help' for help. ");
			MessagePresenter.Instance.WriteLine();
		}

		private static void PrintCliUsage()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Copies data from one repository type to another within the same repository. ");
			sb.AppendLine("Usage: TransportData.exe <options>");
			sb.AppendLine();
			sb.AppendLine("Mandatory options:");
			sb.AppendLine();
			sb.AppendLine("  -F, --From      Source of the data. Possible values: FileRepo, Db_MsSql, Db_SQLite");
			sb.AppendLine("  -T, --To        Destination, so here will be the data copied. Possible values: FileRepo, Db_MsSql, Db_SQLite");
			sb.AppendLine();
			sb.AppendLine("Optional options:");
			sb.AppendLine();
			sb.AppendLine("  -L, --Lang      Set UI language. Possible values: hu, en. Default value: en");
			sb.AppendLine("  -H, --Help      Prints this screen");
			sb.AppendLine();
			sb.AppendLine();

			MessagePresenter.Instance.WriteLine(sb.ToString());
		}

		private static void StartDebugger()
		{
			if(!Debugger.IsAttached)
				Debugger.Launch();
			Debugger.Break();
		}
	}
}
