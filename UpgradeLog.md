# How to upgrade

This document contains incremental upgrade instructions from version to version

## v0.0.7

0. Create a **backup** of your data.
1. Update this file: `<ExinRoot>`\\Repositories\\`<RepoName>`\\Data\\Categories.xml 
	1. Merge with: `<ExinRoot>`\\ResourcesDefault\\Categories.xml
	1. Note, that in case you did not add categories manually, you can just overwrite your active version
1. Run `TransportData.exe --From FileRepo --To [Db_SQLite|Db_MsSql]`
	1. This will import the new categories from `Categories.xml` into DB.	 

## v0.0.5

* RepoSettings.xml new members:
	* Repo
		* MsSqlSettings
			* ConnectionStrings
				* AdoNet
				* EntityFramework


## v0.0.4

The v0.0.4 is the release of the MainSettings.xml and RepoSettings.xml.  
No automatic patch has been made for it.  
If you want to upgrade an existing v0.0.3 app to v0.0.4, you have to manually patch it.

Used placeholders:  
`<OldVersionInstallDir>` : The installation directory of the old version  
`<NewVersionInstallDir>` : The installation directory of the new version`*`  
`<RepoRoot>`             : The existing repository's root directory  
`*`Put it first into a separate directory

0. Create a **backup** of your data and old version app.
1. Manually copy these files:
	`<NewVersionInstallDir>`/ResourcesDefault/RepoSettings.xml  --> `<RepoRoot>`/Data/RepoSettings.xml
	1. If you have your `<RepoRoot>` under `<OldVersionInstallDir>`, copy into into `<NewVersionInstallDir>`
2. You have to extract the values for these xml files:
	`<OldVersionInstallDir>`/Config/AppSettings.config         <--> `<NewVersionInstallDir>`/Config/MainSettings.xml
	1. AppSettings

* Important:
	* RepoRootDir <--> MainSettings: App/Repositories/Repo/RootDir
* Others:
	* ReadMode <--> RepoSettings: Repo/ReadMode
	* SaveMode <--> RepoSettings: Repo/SaveMode
	* DbAccessMode <--> RepoSettings: Repo/DbAccessMode
	* DbType <--> RepoSettings: Repo/DbType


## v0.0.3

(No info here yet)
