using System;
using System.IO;

namespace Common.Utils.FileManagerSafe
{
	public class FileInfoSafe : IDisposable
	{
		public readonly object FileLock;
		public readonly FileInfo FileInfo;

		public FileInfoSafe(string file)
		{
			FileManagerSafe.Get(file, out FileInfo, out FileLock);
		}

		public void Dispose()
		{
			FileManagerSafe.Remove(FileInfo);
		}
	}
}