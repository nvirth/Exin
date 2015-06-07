using System.Windows;
using System.Windows.Navigation;
using Common.Configuration;
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
			//Cultures.SetDefaultCultureToHungarian();
			Cultures.SetToEnglish();
		}
		
		protected override void OnLoadCompleted(NavigationEventArgs e)
		{
			base.OnLoadCompleted(e);

			// The exceptions' message will be english 
			Cultures.SetToEnglish();
		}

	    protected override void OnStartup(StartupEventArgs e)
	    {
            // Do not run multiple instances from the app
            ApplicationRunningHelper.SwitchToRunningInstanceIfExists();
        }
    }
}
