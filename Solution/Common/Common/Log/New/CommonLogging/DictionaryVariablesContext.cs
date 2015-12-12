using System.Collections.Generic;
using Common.Logging;

namespace Common.Log.New.CommonLogging
{
	public class DictionaryVariablesContext : Dictionary<string, object>, IVariablesContext
	{
		public void Set(string key, object value)
		{
			this[key] = value;
		}

		public object Get(string key)
		{
			return ContainsKey(key) ? this[key] : null;
		}

		public bool Contains(string key)
		{
			return this.ContainsKey(key);
		}

		public new void Remove(string key)
		{
			base.Remove(key);
		}

		public void Clear()
		{
			base.Clear();
		}
	}
}