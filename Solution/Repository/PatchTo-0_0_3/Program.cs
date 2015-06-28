using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Configuration;
using Common.Utils;
using Common.Utils.Helpers;
using Localization;

namespace PatchTo_0_0_3
{
	class Program
	{
		static void Main(string[] args)
		{
			Cultures.SetToEnglish();

			PromptBackupWarning(OldRepoPaths.RootDir);

			
		}

		private static void PromptBackupWarning(string backupMsg)
		{
			MessagePresenter.Instance.WriteLine(Localized.Before_going_further__please_create_a_manual_backup_of_these__);
			MessagePresenter.Instance.WriteLine(backupMsg);
			MessagePresenter.Instance.WriteLine();
			Helpers.ExecuteWithConsoleColor(
				ConsoleColor.Yellow,
				() => MessagePresenter.Instance.WriteLine(Localized.Press_any_key_when_you_are_ready_to_continue_______)
			);
			Console.ReadKey();
			MessagePresenter.Instance.WriteLine();
		}
	}
}
