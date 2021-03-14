SlowCheetah
===========

SlowCheetah is a Visual Studio extension (and msbuild) extension for Config Transform.

Troubleshooting
---------------

If you get this error:

	The "SlowCheetah.Xdt.TransformXml" task could not be loaded from the assembly C:\Users\<username>\AppData\Local\Microsoft\MSBuild\SlowCheetah\v2.5.10.2\SlowCheetah.Xdt.dll. Could not load file or assembly 'C:\Users\<username>\AppData\Local\Microsoft\MSBuild\SlowCheetah\v2.5.10.2\SlowCheetah.Xdt.dll' or one of its dependencies. The system cannot find the file specified. Confirm that the <UsingTask> declaration is correct, that the assembly and all its dependencies are available, and that the task contains a public class that implements Microsoft.Build.Framework.ITask.

Then follow the steps from here:
http://stackoverflow.com/questions/19936476/the-slowcheetah-xdt-transformxml-task-could-not-be-loaded-from-the-assembly
(Saved version: SlowCheetah.mht)
+ Pay attention for this comment: I also needed to restart VS 2013