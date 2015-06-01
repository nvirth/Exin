using System;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml.Linq;
using System.Xml.Serialization;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using UtilsShared;

namespace WEB
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
		}

		/// <summary>
		/// !IMPORTANT! In debug mode, an error can cause many HttpErrorCodes, eg: 404, 500...
		/// But in release mode, all type of errors will Redirect to an error page, so the
		/// HttpErrorCode will be always 302. 
		/// This information is very important by handling error types by Ajax responses: 
		/// the original error code is in the query string
		/// </summary>
		protected void Application_Error(object sender, EventArgs e)
		{
			if(isDebug())	// In debug mode, we want to see the "yellow death" page
				return;

			var httpException = Server.GetLastError() as HttpException;
			int? httpErrorCode = httpException == null ? (int?)null : httpException.GetHttpCode();
			Response.Clear();

			Response.Redirect(String.Format("~/Error?httpErrorCode={0}", httpErrorCode));

			/* By using the below code, no exceptions will reach the UI - but nor Elmah
			 
			Server.GetLastError();
			Server.ClearError();
			 */
		}

		//protected void Session_Start(object sender, EventArgs e) { }
		//protected void Application_BeginRequest(object sender, EventArgs e) { }
		//protected void Application_AuthenticateRequest(object sender, EventArgs e) { }
		//protected void Session_End(object sender, EventArgs e) { }
		//protected void Application_End(object sender, EventArgs e) { }

		private bool isDebug()	// Placed out to a method, to not ruin the code coloring
		{
			bool isDebug = false;
#if DEBUG
			isDebug = true;
#endif
			return isDebug;
		}

	}
}