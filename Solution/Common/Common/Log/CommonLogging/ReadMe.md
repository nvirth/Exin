If we would use the FormatMessageHandler from Common.Logging; we would have to
add reference to Common.Logging.Core in every project where we use logging -
so in every project.
Since our Log.cs provided logging API uses an own delegate, adding a reference
to Common.Logging.Core is not necessary anymore; a reference to our Common 
project is enough.