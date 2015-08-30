using System.Collections.Generic;
using System.IO;

namespace Common.Utils.FileManagerSafe
{
	public static class FileManagerSafe
	{
		private static readonly Dictionary<string, object> _lokcDictionary = new Dictionary<string, object>();
		private static readonly object _lockDictionaryGuard = new object();

		public static void Get(string file, out FileInfo fileInfo, out object fileLock)
		{
			fileInfo = new FileInfo(file);

			lock(_lockDictionaryGuard)
			{
				if(_lokcDictionary.ContainsKey(fileInfo.FullName))
				{
					fileLock = _lokcDictionary[fileInfo.FullName];
				}
				else
				{
					fileLock = new object();
					_lokcDictionary.Add(fileInfo.FullName, fileLock);
				}
			}
		}

		public static void Remove(FileInfo fileInfo)
		{
			lock(_lockDictionaryGuard)
			{
				if(_lokcDictionary.ContainsKey(fileInfo.FullName))
					_lokcDictionary.Remove(fileInfo.FullName);
			}
		}
	}
}