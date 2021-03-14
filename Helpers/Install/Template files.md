Template files
==============

You need to create a local copy of all the template files/folders, so the ones ending with ".template".
You can find instructions in them.
Never commit the local copies of these! There are present in the git ignore list, so they won't be commited by accident.

The complate list of these templates
------------------------------------

	Solution/WPF/Config/AppSettings.Debug.config.template
	Solution/WPF/Config/AppSettings.Release.config.template
	Solution/WPF/Config/AppSettings.config.template
	Solution/WPF/Config/ConnectionStrings.Debug.config.template
	Solution/WPF/Config/ConnectionStrings.Release.config.template
	Solution/WPF/Config/ConnectionStrings.config.template

Optional
--------

	Solution/Repository/FileRepoDeveloper.template/

-> This folder contains sandbox data for Debug mode. Creating a local copy of it is optional, because the app will init an empty repo if does not find any.
Only that case, the DeveloperRepo will be empty.