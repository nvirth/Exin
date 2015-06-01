In those projects, in which we want to be able to run the ImportDataToDb in a new process, we need these:
- A reference to the ImportDataToDb project
- In the project file (xxx.csproj) of that project, this have to be exist, for the config file to be reachable after build:
  <Target Name="AfterBuild">
    <Copy SourceFiles="..\Repository\ImportDataToDb\bin\$(Configuration)\ImportDataToDb.exe.config" 
          DestinationFolder="$(TargetDir)" />
  </Target>
  
Projects, where this is in use:
+ WPF
- WEB
- WinForms
? UnitTests