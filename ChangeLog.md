# Change Log

## Unreleased
###Added
###Changed
###Deprecated
###Removed
###Fixed
###Security

## v0.0.5
End user involving changes
* Copy Xml or Json data with Ctlr+C, or from Main menu
* New, JavaScript based chart is available
* BugFix: Pasting works again
* Title and Comment TextBoxes: Do not allow the user to write in more chars than maximum allowed. (Previously there was only an error msg in log window)
* Comment TextBox: Can be multiline

Other changes
* Connection strings moved into RepoSettings.xml, Config/ConnectionString.config eliminated
* Logging with Common.Logging from now on.
** LogLevels for UI and normal file logs are adjustable from MainSettings.xml

## v0.0.4
End user involving changes
* Numeric TextBoxes don't allow chars
* Selection summary on UI
* Alternate row colors in ListViews
* The default button is bold
* The Add-new-daily-expense and Add-new-montly-income forms' labels width is fixed (to Grid auto)
* Statistics: Chart's Y-Axis' MaxValue: Delay in Binding
** This way, if the user types in eg 200, which would mean 3 binding update
   (2,20,200); that will result only 1 update. (If the user types with a normal speed)
* Selected-Edited item
** That row, which is currently edited in eg "Add new daily expense", will be highlighted (bold)
** The Add button's label will be "Copy" if a Selected-Edited item exists.
* Added GridSplitter, the log view is resizable
* "Refresh" instead of "Again" (button)
* Copy TransactionItems to Clipboard (first sketches only)
* BUG-FIXES
** BackgroundTasks don't get awaited on exit
** GridView's selection is lost on AmountChanged
** Adding expense item with empty Title is no more possible.
*** Plus it won't alert validation error when first char is like éÉáÁûÛ...
* MonthlyExpenses ListView: Two fast click on column header (to sort column data)
  - if there was a SelectedItem in the ListView - resulted in jumping to its DailyExpense item

Other changes
* TransportData: App.config: Removed AppSettings and ConnectionStrings
* WPF: Config/AppSettings.config: Removed totally
* Settings.xml-s
** The goal is to eliminate App.config dependecy from the app, and use custom xml files instead
*** MainSettings.xml (1/app)
*** RepoSettings.xml (1/repo [the ability for multiple repos is future plan])
* TransportData: Applied the new solution for "ImportDataToDB"
** So using the TransportData_Worker for tasks with FromFile ReadMode, instead of the legacy TransportData_FromFile_ToDb class
** Implemented ID-Insert for Unit- and CategoryManager
* Started to extract ViewModels
* Started to extract Xaml elements (MainMenu, NewTransactionForm)
* BUG-FIXES
** Updated SQL script: Unit.DisplayNames is unique

## v0.0.3
End user involving changes
* Only 1 instance can run from the app
* Implemented unhandled exception handler (for WPF)
** Prompts the user before exiting
* MonthlySummaries are calculated on every DailyExpense saving
* Renamed Category/eats->food, Unit/db->pc
* BUG-FIXES
** Inconsistent currency on UI
** Transaction's month-dir-names are still localized
** Localization
*** BugFixes: ClearButton, WindowTitle, MainWindow gapes,
*** ImportDataToDb/TransportData project localized

Other changes
* DbManagers more configurable
* FileRepo- and DB- Managers are brought to the same interface
* DAL Managers are no more static, nor singleton
* InitAllStatic
* Renamed project ImportDataToDb to TransportData
** Created universal worker
** Created command line interface
*** See CLI usage in commit 6a27a2d9aed860872ed877b14f60d9b9b45914ba
* Eliminating overused singletons
* Unit, Category: property DisplayName --> DisplayNames (due to localization)
* AppSettings
** Removed old, unused AppSettings items
** Renamed key for the data repository's root to "RepoRootDir" from "RootDir"
* Renamed ConnectionString names to apropriate ones
** See ee626c85c4eb67016d394adf10dfe8834e825333

## v0.0.2
This initial tag contains the project's "initial" state, ready for source control; in the middle of the localization process.
Source control came to this project in the middle of a bigger process: localization.
So this version is not recommended for direct use.
Only contains the WPF based GUI now; these got cleared out: WinForms, Web, UnitTests

## v0.0.1
This initial tag contains all the depricated subprojects as well:
* WinForms
* Web
* UnitTests

These were added to source control only for preserving them; will be removed later.
NOTE: The project came under source control in the middle of a bigger restruction process, so these projects don't fit for reference