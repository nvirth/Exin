using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Common.Configuration;
using Common.Log;
using Localization;
using Newtonsoft.Json;
using C = Common.Configuration.Constants.Xml.TransactionItem;

namespace Common.Utils.Helpers
{
	public static class Helpers
	{
		// -- Properties, Fields

		public static readonly Random Random = new Random(DateTime.Now.Millisecond);

		// -- Methods

		#region Normal methods

		#region Console

		public static void ExecuteWithConsoleColor(ConsoleColor color, Action action)
		{
			var beforeColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			action();
			Console.ForegroundColor = beforeColor;
		}

		public static void WriteLineToConsoleWithColor(string msg, ConsoleColor color)
		{
			ExecuteWithConsoleColor(color, () => Console.WriteLine(msg));
		}

		public static void WriteToConsoleWithColor(string msg, ConsoleColor color)
		{
			ExecuteWithConsoleColor(color, () => Console.Write(msg));
		}

		public static void WriteLineToConsoleYellow(string msg)
		{
			ExecuteWithConsoleColor(ConsoleColor.Yellow, () => Console.WriteLine(msg));
		}

		public static void WriteToConsoleYellow(string msg)
		{
			ExecuteWithConsoleColor(ConsoleColor.Yellow, () => Console.Write(msg));
		}

		public static void WriteLineToConsoleRed(string msg)
		{
			ExecuteWithConsoleColor(ConsoleColor.Red, () => Console.WriteLine(msg));
		}

		public static void WriteToConsoleRed(string msg)
		{
			ExecuteWithConsoleColor(ConsoleColor.Red, () => Console.Write(msg));
		}

		#endregion

		#region DateTime

		public static DateTime GetRandomDateTimeBetween(DateTime dateTime1, DateTime dateTime2)
		{
			TimeSpan deltaTime = (dateTime2 - dateTime1).Duration();
			var plusRandomTime = new DateTime()
				.AddDays(Random.Next(0, deltaTime.Days))
				.AddHours(Random.Next(0, deltaTime.Hours))
				.AddMinutes(Random.Next(0, deltaTime.Minutes))
				.AddSeconds(Random.Next(0, deltaTime.Seconds))
				- new DateTime();

			if(dateTime1 < dateTime2)
				return dateTime1.Add(plusRandomTime);
			else
				return dateTime2.Add(plusRandomTime);
		}

		public static DateTime GetRandomDateTimeBetween2mins1max(DateTime earlier1, DateTime earlier2, DateTime later)
		{
			DateTime earlier = earlier1 > earlier2 ? earlier1 : earlier2;
			if(later < earlier)
				throw new ArgumentException(Localized.The_theoretically_later_date_is_earlier_than_the_theoretically_earlier_date_);

			return GetRandomDateTimeBetween(earlier, later);
		}

		public static bool CheckDateTimes_2earlier1later(DateTime earlier1, DateTime earlier2, DateTime later)
		{
			DateTime earlier = earlier1 > earlier2 ? earlier1 : earlier2;
			if(later < earlier)
				return false;

			return true;
		}

		#endregion

		#region DeleteDirectoryIfExist

		public static void RecreateDirectory(string dirPath)
		{
			DeleteDirectoryIfExistsThanRecreateIf(dirPath, alsoRecreate: true);
		}
		public static void DeleteDirectoryIfExists(string dirPath)
		{
			DeleteDirectoryIfExistsThanRecreateIf(dirPath, alsoRecreate: false);
		}

		private static void DeleteDirectoryIfExistsThanRecreateIf(string dirPath, bool alsoRecreate = true)
		{
			int numOfErrors = 0;
			bool wasError;

			do
			{
				try
				{
					if(Directory.Exists(dirPath))
						Directory.Delete(dirPath, recursive: true);

					if(alsoRecreate)
					{
						Thread.Sleep(10);
						Directory.CreateDirectory(dirPath);

						// Sometimes, seemingly randomly, it happens it can't recreate the folder
						// immeadiately after deleting it; but there won't be any exception thrown!
						for(int i = 0; i < 10; i++)
						{
							Thread.Sleep(10);
							if(!Directory.Exists(dirPath))
								throw new DirectoryNotFoundException("DeleteDirectoryIfExist: The removed, than recreated directory does not exist. Dir: \r\n{0}\r\n".Formatted(dirPath));
						}
					}

					wasError = false;
				}
				catch(Exception e)
				{
					ExinLog.ger.LogException(e.Message, e); // TODO should not log to UI

					wasError = true;
					numOfErrors++;

					if(numOfErrors == 10)
					{
						ExinLog.ger.LogError("DeleteDirectoryIfExist: Could not purge directory: " + dirPath);
						throw;
					}

					Thread.Sleep(50);
				}
			} while((numOfErrors < 10) && (wasError));
		}

		#endregion

		#region File

		public static void CreateNewFileDeleteOld(FileInfo newFile, FileInfo oldFile, StringBuilder stringBuilder)
		{
			using(var streamWriter = new StreamWriter(newFile.FullName))
			{
				streamWriter.Write(stringBuilder.ToString());
			}
			newFile.Refresh(); // Otherwise the FileInfo object wouldn't refresh, and eg Exist == false would remain

			if(oldFile != null && oldFile.FullName != newFile.FullName) // && oldFile.Exists)
				oldFile.Delete();
		}

		public static void CreateNewFileDeleteOld(string newFilePath, FileInfo oldFile, FileInfo fileToCopy)
		{
			fileToCopy.CopyTo(newFilePath, /*overwrite*/ true);

			if(oldFile != null && oldFile.FullName != newFilePath)
				oldFile.Delete();
		}

		#endregion

		#region File in

		public static string[] ReadInFile(string path, Encoding encoding = null)
		{
			return ReadInFileList(path, encoding).ToArray();
		}

		public static List<string> ReadInFileList(string path, Encoding encoding = null)
		{
			encoding = encoding ?? Encoding.UTF8;

			using(var reader = new StreamReader(new FileStream(path, FileMode.Open), encoding))
			{
				var stringList = new List<string>();
				while(!reader.EndOfStream)
				{
					stringList.Add(reader.ReadLine());
				}
				return stringList;
			}
		}

		public static string ReadInFileString(string path, Encoding encoding = null)
		{
			encoding = encoding ?? Encoding.UTF8;

			using(var reader = new StreamReader(new FileStream(path, FileMode.Open), encoding))
			{
				return reader.ReadToEnd();
			}
		}

		#endregion

		#region GetNameOfThisMethod

		/// <summary>
		/// Returns the name of the caller method, if the input string is null. 
		/// (Othervise returns the input string)
		/// </summary>
		public static string GetNameOfThisMethod([CallerMemberName] string leaveThisNull = null)
		{
			return leaveThisNull;
		}

		#endregion

		#region PrintCurrentProcessMemoryUsage

		/// <summary>
		/// Prints all kind of memory usage of the current process
		/// </summary>
		public static void PrintCurrentProcessMemoryUsage()
		{
			var currentProcess = Process.GetCurrentProcess();
			MessagePresenter.Instance.WriteLine("currentProcess.NonpagedSystemMemorySize64: " + currentProcess.NonpagedSystemMemorySize64);
			MessagePresenter.Instance.WriteLine("currentProcess.PagedMemorySize64: " + currentProcess.PagedMemorySize64);
			MessagePresenter.Instance.WriteLine("currentProcess.PagedSystemMemorySize64: " + currentProcess.PagedSystemMemorySize64);
			MessagePresenter.Instance.WriteLine("currentProcess.PeakPagedMemorySize64: " + currentProcess.PeakPagedMemorySize64);
			MessagePresenter.Instance.WriteLine("currentProcess.PeakVirtualMemorySize64: " + currentProcess.PeakVirtualMemorySize64);
			MessagePresenter.Instance.WriteLine("currentProcess.PrivateMemorySize64: " + currentProcess.PrivateMemorySize64);
			MessagePresenter.Instance.WriteLine("currentProcess.VirtualMemorySize64: " + currentProcess.VirtualMemorySize64);
		}

		#endregion

		#endregion

		#region Extension methods

		#region Action

		public static void ExecuteWithTimeMeasuring(this Action action, string text)
		{
			var stopwatch = ExecuteWithTimeMeasuring(action);
			var results = stopwatch.ToFormattedString();

			MessagePresenter.Instance.WriteLine(string.Format("{0} {1}", (text + ":").PadRight(35), results));
		}

		public static Stopwatch ExecuteWithTimeMeasuring(this Action action)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			action();

			stopwatch.Stop();
			return stopwatch;
		}

		#endregion

		#region Array

		public static void AllToFalse(this bool[] array)
		{
			for(int i = 0; i < array.Length; i++)
			{
				array[i] = false;
			}
		}

		#endregion

		#region ComboBox

		/// <summary>
		/// Handle ComboBox's PreviewTextInput event to implement quick search among it's items. 
		/// </summary>
		/// <param name="stringSelector">
		/// It's input params are the items of the ComboBox (one by one, for each). 
		/// It's output must be the item's property, in which the (StartsWith) search will be called. 
		/// </param>
		public static void ComboBoxSearchKey(this ComboBox comboBox, object sender, TextCompositionEventArgs e, Func<object, string> stringSelector)
		{
			int i = 0;
			var indexes = new List<int?>();
			foreach(var comboBoxItem in comboBox.ItemsSource)
			{
				if(stringSelector(comboBoxItem).StartsWith(e.Text, StringComparison.InvariantCultureIgnoreCase))
					indexes.Add(i);

				i++;
			}
			if(indexes.Count != 0)
			{
				int? firstAfterCurrent = indexes.FirstOrDefault(index => comboBox.SelectedIndex < index);
				comboBox.SelectedIndex = firstAfterCurrent ?? indexes[0].Value;
			}

			e.Handled = true;
		}

		#endregion

		#region DateTime

		public static string CalculateMonthDirName(this DateTime dateTime)
		{
			var monthNameEn = Cultures.en_US.DateTimeFormat.GetMonthName(dateTime.Month);
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(dateTime.Year);
			stringBuilder.Append('_');
			stringBuilder.Append(dateTime.MonthIn2Digits());
			stringBuilder.Append(' ');
			stringBuilder.Append(Char.ToUpper(monthNameEn[0])); // Month 1. char
			stringBuilder.Append(monthNameEn.Substring(1)); // Month other chars

			return stringBuilder.ToString();
		}

		public static string CalculateDayFileName(this DateTime dateTime, string extension)
		{
			var result = Path.ChangeExtension(dateTime.DayIn2Digits(), extension);
			return result;
		}

		public static string DayIn2Digits(this DateTime dateTime)
		{
			return dateTime.Day.ToString().PadLeft(2, '0');
			//return dateTime.Day < 10 ? "0" + dateTime.Day : dateTime.Day.ToString();
		}

		public static string MonthIn2Digits(this DateTime dateTime)
		{
			return dateTime.Month < 10 ? "0" + dateTime.Month : dateTime.Month.ToString();
		}

		public static string ToLocalizedShortDateString(this DateTime dateTime)
		{
			return dateTime.ToString(Localized.DateFormat_full);
		}

		#endregion

		#region DependencyObject

		public static T FindAncestor<T>(this DependencyObject dependencyObject)
			where T : class
		{
			T objectT = null;
			do
			{
				dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
				objectT = dependencyObject as T;
			} while(objectT == null && dependencyObject != null);
			return objectT;
		}

		#endregion

		#region Enum

		public static string ToDescriptionString(this Enum enumerationValue)
		{
			return enumerationValue.GetDescription();
		}

		public static string GetDescription(this Enum enumerationValue)
		{
			//Tries to find a DescriptionAttribute for a potential friendly name
			//for the enum
			MemberInfo[] memberInfo = enumerationValue.GetType().GetMember(enumerationValue.ToString());
			if(memberInfo != null && memberInfo.Length > 0)
			{
				object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if(attrs != null && attrs.Length > 0)
				{
					//Pull out the description value
					return ((DescriptionAttribute)attrs[0]).Description;
				}
			}

			//If we have no description attribute, just return the ToString of the enum
			return enumerationValue.ToString();
		}
		
		public static string ToLocalizedDescriptionString(this Enum enumerationValue)
		{
			return enumerationValue.GetLocalizedDescription();
		}

		/// <summary>
		/// In this case the Description attribute will be used as a resource name container
		/// </summary>
		public static string GetLocalizedDescription(this Enum enumerationValue)
		{
			//Tries to find a DescriptionAttribute for a potential friendly name
			//for the enum
			MemberInfo[] memberInfo = enumerationValue.GetType().GetMember(enumerationValue.ToString());
			if(memberInfo != null && memberInfo.Length > 0)
			{
				object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if(attrs != null && attrs.Length > 0)
				{
					//Pull out the description value
					var localizationResourceKey = ((DescriptionAttribute)attrs[0]).Description;
					var result = Localized.ResourceManager.GetString(localizationResourceKey);
					if(result == null)
					{
						ExinLog.ger.LogError(Localized.The_following_enum_is_probably_not_localized_yet,
							new
							{
								@Type = enumerationValue.GetType().ToString(),
								Value = enumerationValue.ToString(),
								Description = localizationResourceKey,
							});
						result = localizationResourceKey;
					}

					return result;
				}
			}

			//If we have no description attribute, just return the ToString of the enum
			return enumerationValue.ToString();
		}

		#endregion

		#region Exception

		public static void WriteWithInnerMessages(this Exception e)
		{
			//int i = 1;
			//while(e != null)
			//{
			//	var indention = string.Concat(Enumerable.Repeat(" - ", i));
			//	Console.WriteLine(indention + e.Message);
			//	e = e.InnerException;
			//	i++;
			//}
			e.WriteWithInnerMessagesRecursive();
		}

		private static void WriteWithInnerMessagesRecursive(this Exception e, int intend = 1)
		{
			// End of recursion
			if(e == null)
				return;

			// Write actual message
			Console.WriteLine(" - ".Repeat(intend) + e.Message);

			// Handle AggregateExceptions
			var ae = e as AggregateException;
			if(ae != null)
			{
				// Call recursive to all inner exceptions
				foreach(var aeInner in ae.InnerExceptions)
				{
					aeInner.WriteWithInnerMessagesRecursive(intend + 1);
				}
			}
			else
			{
				// Call recursive to inner exceptions
				e.InnerException.WriteWithInnerMessagesRecursive(intend + 1);
			}
		}

		public static void WriteWithInnerMessagesColorful(this Exception e, ConsoleColor color)
		{
			var beforeColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			e.WriteWithInnerMessagesRecursive();
			Console.ForegroundColor = beforeColor;
		}

		public static void WriteWithInnerMessagesRed(this Exception e)
		{
			e.WriteWithInnerMessagesColorful(ConsoleColor.Red);
		}

		#endregion

		#region ICollection

		/// <summary>
		/// Compares 2 ICollection, with 2x foreach{foreach}. Do not use this, if not necessary. 
		/// (SequenceEqual can faster compare 2 IList [or in LinQ: IEnumerable, but it's slower], 
		///  if those are sorted the same way)
		/// </summary>
		public static bool ContainsSameData<T>(this ICollection<T> left, ICollection<T> right, IEqualityComparer<T> equalityComparer)
		{
			equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

			foreach(var item in left)
				if(!right.Contains(item, equalityComparer))
					return false;

			foreach(var item in right)
				if(!left.Contains(item, equalityComparer))
					return false;

			return true;
		}

		public static bool Contains(this ICollection iCollection, object value)
		{
			return iCollection.Cast<object>().Contains(value);
		}

		#endregion

		#region IDictionary

		public static TValue GetLocalizedValue<TValue>(this IDictionary<string, TValue> localizedDictionary) where TValue : class
		{
			if (localizedDictionary == null || localizedDictionary.Count == 0)
				return null;

			var localizedValue = GetLocalizedValueImpl(localizedDictionary, Cultures.CurrentCulture.Name);
			if(localizedValue != null)
				return localizedValue;

			var defaultLangValue = GetLocalizedValueImpl(localizedDictionary, Cultures.DefaultCulture.Name);
			if (defaultLangValue != null)
				return defaultLangValue;

			ExinLog.ger.LogError(
				"This localized dictionary did not contain any value nor for the current, nor for the default culture.",
				new {
					CurrentCulture = Cultures.CurrentCulture,
					DefaultCulture = Cultures.DefaultCulture,
					Dictionary = localizedDictionary
				});

			return localizedDictionary.Values.First();
		}

		private static TValue GetLocalizedValueImpl<TValue>(this IDictionary<string, TValue> localizedDictionary, string cultureName) where TValue : class
		{
			if(localizedDictionary.ContainsKey(cultureName)) //"en-US" or "hu-HU"
				return localizedDictionary[cultureName];

			if(cultureName.Length <= 2)
				return null;

			var parentCultureName = cultureName.Substring(0, 2); //"en" or "hu"
			var parentCultureKey = localizedDictionary.Keys.FirstOrDefault(key => key.StartsWith(parentCultureName));
			if(parentCultureKey != null)
				return localizedDictionary[parentCultureKey];

			return null;
		}

		public static void RenameKeyIfExist<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey fromKey, TKey toKey)
		{
			if(dictionary.ContainsKey(fromKey))
				dictionary.RenameKey(fromKey, toKey);
		}

		public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey fromKey, TKey toKey)
		{
			TValue value = dictionary[fromKey];
			dictionary.Remove(fromKey);
			dictionary[toKey] = value;
		}

		public static void PlusEqual<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
		{
			if(dictionary.ContainsKey(key))
			{
				dynamic d1 = dictionary[key];
				dynamic d2 = value;
				dictionary[key] = d1 + d2;
			}
			else
			{
				dictionary[key] = value;
			}
		}

		#endregion

		#region IEnumerable

		public static XElement ToXmlShallow<T>(this IEnumerable<T> listToConvert, Func<T, bool> filter = null, string rootName = C.root)
		{
			var list = (filter == null) ? listToConvert : listToConvert.Where(filter);
			return new XElement(rootName,
				(from node in list
				 select new XElement(typeof(T).Name,
					 from subnode in node.GetType().GetRuntimeProperties()
					 select new XElement(subnode.Name, subnode.GetValue(node, null)))));

		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach(T element in source)
				action(element);
		}

		#endregion

		#region IList

		/// <summary>
		/// Insert sorted into already sorted list. <para />
		/// Works also with WPF non-UI Thread
		/// </summary>
		public static void InsertIntoSorted<T>(this IList<T> list, T newItem) where T : IComparable<T>
		{
			var idx = CalcPosition(list, newItem);

			var dispatcher = Application.Current.Dispatcher; // alias
			if(dispatcher != null)
				dispatcher.Invoke(() => list.Insert(idx, newItem));
			else
				list.Insert(idx, newItem);
		}
		public static int CalcPosition<T>(this IList<T> list, T item) where T : IComparable<T>
		{
			int i;
			bool biggerThanOld;
			CalcPosition(list, item, out i, out biggerThanOld);
			return i;
		}
		private static void CalcPosition<T>(this IList<T> list, T item, out int position, out bool biggerThanOld) where T : IComparable<T>
		{
			biggerThanOld = false;

			int i;
			for(i = 0; i < list.Count; i++)
			{
				var actual = list[i];
				if(ReferenceEquals(item, actual)) // Skip itself
				{
					biggerThanOld = true;
					continue;
				}

				if(!(actual.CompareTo(item) > 0))
					break;
			}
			position = i;
		}

		public static void ReOrder<T>(this ObservableCollection<T> collection, T item) where T : IComparable<T>
		{
			var oldPos = collection.IndexOf(item);

			int newPos;
			bool biggerThanOld;
			collection.CalcPosition(item, out newPos, out biggerThanOld);
			newPos = biggerThanOld ? newPos - 1 : newPos;
			newPos = newPos == collection.Count ? newPos - 1 : newPos;

			if(newPos != oldPos)
				collection.Move(oldPos, newPos);
		}

		public static bool SequenceEqual<T>(this IList<T> left, IList<T> right, IEqualityComparer<T> equalityComparer)
			where T : class
		{
			if(ReferenceEquals(left, right))
				return true;

			if(left == null || right == null)
				return false;

			if(left.Count != right.Count)
				return false;

			equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
			for(int i = 0; i < left.Count; i++)
			{
				if(!equalityComparer.Equals(left[i], right[i]))
					return false;
			}
			return true;
		}

		#endregion

		#region Int

		public static string ToExinStringMonthlyFileName(this int value)
		{
			return value.ToControlledString(padTo: 6, padStr: "  ");
		}

		public static string ToExinStringDailyFileName(this int value)
		{
			//return value.ToControlledString(padTo: 5);
			return value.ToControlledString(padTo: 5, padStr: "  ");
		}

		public static string ToExinStringInFile(this int value)
		{
			return value.ToControlledString(padTo: 1, separateStr: ".", endString: ",-");
		}

		public static string ToExinStringInStatistics(this int value)
		{
			return value.ToControlledString(padTo: 6, separateStr: ".", endString: ",-", padStr: " ");
		}

		public static string ToControlledString(this int value, int padTo, string padStr = "0", string separateStr = " ", int groupSize = 3, string endString = "")
		{
			var numberFormatInfo = new NumberFormatInfo
			{
				NumberGroupSeparator = separateStr,
				NumberGroupSizes = new[] { groupSize },
			};

			var numberStr = value.ToString("N0", numberFormatInfo);
			var xxx = new string('x', groupSize);
			var baseStr = ((xxx + " ").Repeat(padTo / groupSize) + xxx).Substring(groupSize - padTo % groupSize);

			var result = numberStr;
			if(numberStr.Length < baseStr.Length)
			{
				var baseStart = baseStr.Substring(0, baseStr.Length - numberStr.Length);
				result = (baseStart + numberStr).Trim().Replace("x", padStr);
			}

			return result + endString;
		}

		public static string ToHuCurrency(this int value)
		{
			var numberFormatInfo = new NumberFormatInfo
			{
				NumberGroupSeparator = ((char)160).ToString(), // &nbsp;
				NumberGroupSizes = new[] { 3 },
			};

			return value.ToString("n0", numberFormatInfo) + ",-";

			//return value.ToString("c0", Constants.CultureInfoHu).Replace(' ', (char)160); // char 160 <-- &nbsp;
		}

		#endregion

		#region Object

		public static string ToJson(this object value)
		{
			return JsonConvert.SerializeObject(value, Formatting.Indented);
		}

		/// <summary>
		/// Don't use this; the ToXml is a better version
		/// </summary>
		public static XElement ToXmlShallow(this object obj)
		{
			IEnumerable<XElement> xElements =
				from property in obj.GetType().GetRuntimeProperties()
				select new XElement(property.Name, property.GetValue(obj));

			return new XElement(obj.GetType().Name, xElements);
		}

		public static XElement ToXml(this object obj)
		{
			var type = obj.GetType();

			// If obj is value type or string, then return it wrapped
			if(type.IsValueType || (obj is string))
				return new XElement(type.Name.ToFriendlyUrl(), obj);

			// If obj is a class (but not string), iterate it's properties, and serialize them to xml format
			IEnumerable<XElement> xElements =
				type.GetRuntimeProperties()
					.Select(property =>
							{
								object propertyValue = property.GetValue(obj);
								if(propertyValue != null)
								{
									var propertyType = propertyValue.GetType();

									// If the property is not a value type or a string, call .ToXml to the property as well
									if(!propertyType.IsValueType && !(propertyValue is string))
									{
										var enumerablePropertyValue = propertyValue as IEnumerable<object>;
										if(enumerablePropertyValue != null)
										{
											// If an IEnumerable type is empty, don't attach it to the xml
											propertyValue = enumerablePropertyValue.Any()
												? (object)propertyValue.ToXml()
												: null; // enumerablePropertyValue.GetType().Name + " = null";
										}
										else // Non-IEnumberable types
										{
											propertyValue = propertyValue.ToXml();
										}
									}
								}

								return new XElement(property.Name.ToFriendlyUrl(), propertyValue);
							});

			return new XElement(type.Name.ToFriendlyUrl(), xElements);
		}

		/// <summary>
		/// Creates a deep clone from the given object
		/// </summary>
		/// <typeparam name="T">The param type must be marked with the [Serializable] attribute</typeparam>
		public static T DeepClone<T>(this T a)
		{
			using(MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, a);
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}
		}

		/// <summary>
		/// Perform a deep Copy of the object, using Json as a serialisation method.
		/// </summary>
		private static T DeepClone_JsonMethod<T>(this T source)
		{
			// The problem with this now:
			// Serializes an ExpenseItem, which has a Unit (eg kg)
			// By deserialization, it creates a new ExpenseItem, so calls the ctor.
			// In ctor, the default Unit (pc) will be assigned to the ExpenseItem.Unit
			// After that, the deserializer won't drop that instance! Instead overwrites
			// (some of) its properties.
			// The problem with this is, that we use now references, and not deep clones
			// from the Units, and so, the default unit in the cache will be (half-)
			// overwritten
			//
			// So don't use this until that problem gets a solution

			// Don't serialize a null object, simply return the default for that object
			if(Object.ReferenceEquals(source, null))
				return default(T);

			var serializedObj = JsonConvert.SerializeObject(source);
			return JsonConvert.DeserializeObject<T>(serializedObj);
		}

		#endregion

		#region Property

		/// <summary>
		///  Returns the given property's name as a string, so it can be used strongly typed
		/// </summary>
		public static string Property<TClass, TProperty>(this TClass tClass, Expression<Func<TClass, TProperty>> property)
		{
			var memberExpression = property.Body as MemberExpression;
			if(memberExpression == null)
				throw new Exception(Localized.TProperty_must_be_a_member_of_a_class);

			return memberExpression.Member.Name;
		}

		#endregion

		#region StopWatch

		public static string ToFormattedString(this Stopwatch stopwatch)
		{
			var min = stopwatch.Elapsed.Minutes;
			var s = stopwatch.Elapsed.Seconds;
			var ms = stopwatch.Elapsed.Milliseconds;
			var totalUs = (stopwatch.Elapsed.Ticks / 10); // There are 10 ticks in a microSec
														  //var us = totalUs - (ms * 1000 + s * 1000000 + min * 60000000);
			var us = totalUs % 1000;

			return string.Format("{0,3}min {1,3}s {2,3}ms {3,3}us", min, s, ms, us);
		}

		#endregion

		#region String

		public static string Join<T>(this IEnumerable<T> strings, string separator)
		{
			return string.Join(separator, strings);
		}

		public static string Repeat(this string stringToRepeat, int repeat)
		{
			var stringBuilder = new StringBuilder(repeat * stringToRepeat.Length);
			stringBuilder.AppendRepeat(stringToRepeat, repeat);
			return stringBuilder.ToString();
		}

		public static string Truncate(this string text, int maxLetters)
		{
			if(text.Length > maxLetters)
				return new StringBuilder(text, 0, maxLetters, maxLetters + 3).Append("...").ToString();
			else
				return text;
		}

		public static string ToFriendlyUrl(this string text)
		{
			// Normalize the text using full canonical decomposition.
			text = text.Normalize(NormalizationForm.FormD);

			// Remove non-number and non-letter characters.
			Regex nonspace = new Regex("[^0-9A-Za-z ]");
			text = nonspace.Replace(text, String.Empty);

			// Replace space characters with "-", following Google's recommendation.
			text = text.Replace(' ', '-');

			// Remove trailing and leading dashes.
			string replaced = text.Trim('-');

			// Encode the remaining special characters with standard URL encoding.
			string urlEncoded = HttpUtility.UrlEncode(replaced);

			// Return the normalized text or "1" if the normalized text is empty.
			return String.IsNullOrEmpty(urlEncoded) ? "1" : urlEncoded;
		}

		public static T ToEnum<T>(this string stringValue, T defaultValue, bool ignoreCase = false) where T : struct
		{
			try
			{
				return stringValue.ToEnum<T>(ignoreCase);
			}
			catch(Exception)
			{
				return defaultValue;
			}
		}

		public static T ToEnum<T>(this string stringValue, bool ignoreCase = false) where T : struct
		{
			var enumType = typeof(T);
			if(!enumType.IsEnum)
				throw new ArgumentException(Localized.T_must_be_of_Enum_type__);

			var stringValueLower = stringValue.ToLower();

			foreach(T enumValue in Enum.GetValues(enumType))
			{
				var friendlyEnumValue = ((Enum)(object)enumValue).GetDescription();

				if(ignoreCase)
				{
					if(friendlyEnumValue.ToLower() == stringValueLower)
						return enumValue;
				}
				else
				{
					if(friendlyEnumValue == stringValue)
						return enumValue;
				}
			}

			foreach(T enumValue in Enum.GetValues(enumType))
			{
				if(ignoreCase)
				{
					if(enumValue.ToString().ToLower() == stringValueLower)
						return enumValue;
				}
				else
				{
					if(enumValue.ToString() == stringValue)
						return enumValue;
				}
			}

			throw new Exception(string.Format(Localized.The__0__enumeration_does_not_have_any_value_for__1__FORMAT__, enumType.Name, stringValue));
		}

		#region String.Format

		public static string Formatted(this string format, object arg0)
		{
			return string.Format(format, arg0);
		}
		public static string Formatted(this string format, object arg0, object arg1)
		{
			return string.Format(format, arg0, arg1);
		}
		public static string Formatted(this string format, object arg0, object arg1, object arg2)
		{
			return string.Format(format, arg0, arg1, arg2);
		}
		public static string Formatted(this string format, params object[] args)
		{
			return string.Format(format, args);
		}
		public static string Formatted(this string format, IFormatProvider provider, params object[] args)
		{
			return string.Format(provider, format, args);
		}

		#endregion

		#endregion

		#region StringBuilder

		public static StringBuilder AppendRepeat(this StringBuilder stringBuilder, string stringToRepeat, int repeat)
		{
			if(repeat == 0 || String.IsNullOrEmpty(stringToRepeat))
				return stringBuilder;

			for(int i = 0; i < repeat; i++)
			{
				stringBuilder.Append(stringToRepeat);
			}
			return stringBuilder;
		}

		public static StringBuilder AppendDateToQuery(this StringBuilder stringBuilder, DateTime date)
		{
			stringBuilder.Append("'");
			stringBuilder.Append(date.ToString("yyyy-MM-dd"));
			stringBuilder.Append(" 00:00:00");
			stringBuilder.Append("'");
			return stringBuilder;
		}

		public static StringBuilder AppendBoolToQuery(this StringBuilder stringBuilder, bool boolean)
		{
			stringBuilder.Append(boolean ? "1" : "0");
			return stringBuilder;
		}

		#endregion

		#endregion
	}
}
