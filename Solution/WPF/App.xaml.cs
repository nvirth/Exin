using System.Windows;
using System.Windows.Navigation;
using Common.Utils.Helpers;

namespace WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			// The controls will init with hungarian CultureInfo
			//Helpers.SetDefaultCultureToHungarian();
			Helpers.SetDefaultCultureToEnglish();
		}
		
		protected override void OnLoadCompleted(NavigationEventArgs e)
		{
			base.OnLoadCompleted(e);

			// The exceptions' message will be english 
			Helpers.SetDefaultCultureToEnglish();
		}
	}
}
