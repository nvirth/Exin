﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common.Configuration;
using Exin.Common.Logging;
using Exin.Common.Logging.Core;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;
using Config = Common.Configuration.Config;

namespace Common
{
	public class DatePaths
	{
		#region Properties, Fields

		#region Month

		public int DaysInMonth { get; private set; }

		public string MonthDirPath { get; private set; }
		public DirectoryInfo MonthDir { get; private set; }

		public string MonthlyExpensesFilePath { get; private set; }
		public FileInfo MonthlyExpensesFile { get; private set; }

		public string MonthlyIncomesFilePath { get; private set; }
		public FileInfo MonthlyIncomesFile { get; private set; }

		private string _monthDirName;
		public string MonthDirName
		{
			get { return _monthDirName; }
			private set
			{
				_monthDirName = value;
				DaysInMonth = DateTime.DaysInMonth(Date.Year, Date.Month);

				MonthDirPath = Path.Combine(Config.Repo.Paths.ExpensesAndIncomesDir, MonthDirName);
				MonthDir = new DirectoryInfo(MonthDirPath);

				if(!MonthDir.Exists)
					CreateAndFillMonthDir();

				MonthlyExpensesFilePath = Path.Combine(MonthDirPath, RepoPaths.Names.MonthlyExpensesSum);
				MonthlyExpensesFile = new FileInfo(MonthlyExpensesFilePath);

				if(!MonthlyExpensesFile.Exists)
				{
					var foundFile = SearchSummaryFile(MonthlyExpensesFile);
					if(foundFile != null)
					{
						MonthlyExpensesFile = foundFile;
						MonthlyExpensesFilePath = foundFile.FullName;
					}
				}

				MonthlyIncomesFilePath = Path.Combine(MonthDirPath, RepoPaths.Names.MonthlyIncomesSum);
				MonthlyIncomesFile = new FileInfo(MonthlyIncomesFilePath);

				if(!MonthlyIncomesFile.Exists)
				{
					var foundFile = SearchSummaryFile(MonthlyIncomesFile);
					if(foundFile != null)
					{
						MonthlyIncomesFile = foundFile;
						MonthlyIncomesFilePath = foundFile.FullName;
					}
				}
			}
		}

		#endregion

		#region Day

		public string DayFilePath { get; private set; }
		public FileInfo DayFile { get; private set; }

		private string _dayFileName;
		public string DayFileName
		{
			get { return _dayFileName; }
			private set
			{
				SetDayFileName(value);
			}
		}

		private void SetDayFileName(string value, bool searchIfNotExist = true)
		{
			_dayFileName = value;
			DayFilePath = Path.Combine(MonthDirPath, DayFileName);
			DayFile = new FileInfo(DayFilePath);

			if(!DayFile.Exists && searchIfNotExist)
			{
				var foundFile = SearchSummaryFile(DayFile);
				if(foundFile != null)
				{
					_dayFileName = foundFile.Name;
					DayFilePath = foundFile.FullName;
					DayFile = foundFile;
				}
				else
				{
					DayFile.Create();
				}
			}
		}

		#endregion

		#region Date

		private DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				bool isNewMonth = _date.Month != value.Month;
				bool isNewDay = _date.Day != value.Day;

				_date = value;

				if(isNewMonth)
					MonthDirName = value.CalculateMonthDirName();

				if(isNewDay)
					DayFileName = value.CalculateDayFileName(Config.FileExtension);
			}
		}

		#endregion

		#endregion

		#region Constructors

		public DatePaths(DateTime dateTime)
		{
			_date = dateTime.AddDays(-1).AddMonths(-1); // Important, init, Null-Exception without it
			Date = dateTime;
		}

		#endregion

		#region Methods

		private static FileInfo SearchSummaryFile(FileInfo file)
		{
			if(file.Directory != null && file.Directory.Exists)
			{
				var startsWith = Path.GetFileNameWithoutExtension(file.Name);
				var fileFilter = new Regex("^" + startsWith + @" - ((\d)| )+" + file.Extension + "$");
				var possibleFiles = file.Directory.EnumerateFiles().Where(fileInfo => fileFilter.IsMatch(fileInfo.Name));

				try
				{
					return possibleFiles.SingleOrDefault();
				}
				catch(Exception e)
				{
					var stringBuilder = new StringBuilder();
					stringBuilder.AppendLine(string.Format(Localized.Error__More_than_one_exists_from_files_starting_with__0__FORMAT__, startsWith));

					var fileInfos = possibleFiles.ToArray();
					var result = fileInfos[0];
					foreach(var possibleFile in fileInfos)
					{
						stringBuilder.Append(possibleFile.Name)
							.Append(" (")
							.Append(Localized.last_written_at__)
							.Append(possibleFile.LastWriteTime.ToString(Localized.DateFormat_month_day))
							.AppendLine(")");

						if(result.LastWriteTime < possibleFile.LastWriteTime)
							result = possibleFile;
					}

					stringBuilder.Append(Localized.The_file_choosen_for_use_)
						.AppendLine(result.Name);

					MessagePresenter.Instance.WriteLineSeparator();
					MessagePresenter.Instance.WriteError(stringBuilder.ToString());
					MessagePresenter.Instance.WriteException(e);

					return result;
				}
			}
			else
			{
				throw Log.Error(typeof(DatePaths),
					m => m(Localized.ResourceManager, LocalizedKeys.Directory_not_found),
					LogTarget.All,
					new DirectoryNotFoundException(file.DirectoryName)
				);
			}
		}

		public void CreateAndFillMonthDir()
		{
			MessagePresenter.Instance.WriteLine(Localized.Could_not_find_the_monthly_directory_ + MonthDirName);
			MonthDir.Create();
			MessagePresenter.Instance.WriteLine(Localized.The_monthly_directory_created_);

			//MessagePresenter.Instance.WriteLine(Localized.Creating_the_monthly_files_);
			for(int i = 1; i <= DaysInMonth; i++)
			{
				var fileName = (i < 10 ? "0" : "") + i;
				fileName = Path.ChangeExtension(fileName, Config.FileExtension);

				var filePath = Path.Combine(MonthDirPath, fileName);
				using(var fileStream = File.OpenWrite(filePath))
					fileStream.SetLength(0);

				//MessagePresenter.Instance.WriteLine(fileName);
			}
			//MessagePresenter.Instance.WriteLine(Localized.All_created_successfully);
		}

		#region Calculate...FileNames

		public void CalculateDailyFileNames(int dailySum, out FileInfo oldFile, out FileInfo newFile)
		{
			oldFile = DayFile;

			var start = Date.DayIn2Digits();

			string newFileName;
			if(dailySum == 0)
				newFileName = start;
			else
				newFileName = start + " - " + dailySum.ToExinStringDailyFileName();

			newFileName = Path.ChangeExtension(newFileName, Config.FileExtension);
			SetDayFileName(newFileName, /*searchIfNotExist*/ false);

			newFile = DayFile;
		}

		public void CalculateMonthlyIncomesFileNames(int sum, out FileInfo oldFile, out FileInfo newFile)
		{
			oldFile = MonthlyIncomesFile;

			var start = Path.GetFileNameWithoutExtension(RepoPaths.Names.MonthlyIncomesSum);

			string newFileName;
			if(sum == 0)
				newFileName = start;
			else
				newFileName = start + " - " + sum.ToExinStringDailyFileName();

			newFileName = Path.ChangeExtension(newFileName, Config.FileExtension);

			MonthlyIncomesFilePath = Path.Combine(MonthDirPath, newFileName);
			MonthlyIncomesFile = new FileInfo(MonthlyIncomesFilePath);

			newFile = MonthlyIncomesFile;
		}

		public void CalculateMonthlyFileNames(int monthlySum, out FileInfo oldFile, out FileInfo newFile)
		{
			oldFile = MonthlyExpensesFile;

			var start = Path.GetFileNameWithoutExtension(RepoPaths.Names.MonthlyExpensesSum);

			var newFileName = start + " - " + monthlySum.ToExinStringMonthlyFileName();

			newFileName = Path.ChangeExtension(newFileName, Config.FileExtension);

			MonthlyExpensesFilePath = Path.Combine(MonthDirPath, newFileName);
			MonthlyExpensesFile = new FileInfo(MonthlyExpensesFilePath);

			newFile = MonthlyExpensesFile;
		}

		#endregion

		#endregion
	}
}
