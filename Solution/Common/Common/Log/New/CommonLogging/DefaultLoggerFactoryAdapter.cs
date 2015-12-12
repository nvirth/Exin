using System.Diagnostics;
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Simple;

namespace Emasos.Common.Logging
{
    /// <summary>
    ///     Factory for creating <see cref="ILog" /> instances that write data to <see cref="Debug.WriteLine" />.
    /// </summary>
    /// <remarks>
    ///     <example>
    ///         Below is an example how to configure this adapter:
    ///         <code>
    /// &lt;configuration&gt;
    /// 
    ///   &lt;configSections&gt;
    ///     &lt;sectionGroup name=&quot;common&quot;&gt;
    ///       &lt;section name=&quot;logging&quot;
    ///                type=&quot;Common.Logging.ConfigurationSectionHandler, Common.Logging&quot;
    ///                requirePermission=&quot;false&quot; /&gt;
    ///     &lt;/sectionGroup&gt;
    ///   &lt;/configSections&gt;
    /// 
    ///   &lt;common&gt;
    ///     &lt;logging&gt;
    ///       &lt;factoryAdapter type=&quot;Common.Logging.Simple.DefaultLoggerFactoryAdapter, Common.Logging&quot;&gt;
    ///         &lt;arg key=&quot;level&quot; value=&quot;ALL&quot; /&gt;
    ///       &lt;/factoryAdapter&gt;
    ///     &lt;/logging&gt;
    ///   &lt;/common&gt;
    /// 
    /// &lt;/configuration&gt;
    /// </code>
    ///     </example>
    /// </remarks>
    /// <seealso cref="AbstractSimpleLoggerFactoryAdapter" />
    /// <seealso cref="LogManager.Adapter" />
    /// <seealso cref="ConfigurationSectionHandler" />
    /// <author>Gilles Bayon</author>
    /// <author>Mark Pollack</author>
    /// <author>Erich Eichinger</author>
    public class DefaultLoggerFactoryAdapter : AbstractSimpleLoggerFactoryAdapter
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultLoggerFactoryAdapter" /> class using default
        ///     settings.
        /// </summary>
        public DefaultLoggerFactoryAdapter()
            : base(null) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultLoggerFactoryAdapter" /> class.
        /// </summary>
        /// <remarks>
        ///     Looks for level, showDateTime, showLogName, dateTimeFormat items from
        ///     <paramref name="properties" /> for use when the GetLogger methods are called.
        ///     <see cref="ConfigurationSectionHandler" /> for more information on how to use the
        ///     standard .NET application configuraiton file (App.config/Web.config)
        ///     to configure this adapter.
        /// </remarks>
        /// <param name="properties">
        ///     The name value collection, typically specified by the user in
        ///     a configuration section named common/logging.
        /// </param>
        public DefaultLoggerFactoryAdapter(NameValueCollection properties)
            : base(properties) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AbstractSimpleLoggerFactoryAdapter" /> class with
        ///     default settings for the loggers created by this factory.
        /// </summary>
        public DefaultLoggerFactoryAdapter(LogLevel level, bool showDateTime, bool showLogName, bool showLevel, string dateTimeFormat)
            : base(level, showDateTime, showLogName, showLevel, dateTimeFormat) { }
        #endregion

        /// <summary>
        ///     Creates a new <see cref="DefaultLogger" /> instance.
        /// </summary>
        protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            ILog log = new DefaultLogger(name, level, showLevel, showDateTime, showLogName, dateTimeFormat);
            return log;
        }
    }
}