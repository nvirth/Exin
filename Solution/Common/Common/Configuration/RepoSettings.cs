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
		public RepoPaths RepoPaths { get; private set; }

		public RepoSettings(RepoPaths repoPaths)
		{
			RepoPaths = repoPaths;
		}
	}
}