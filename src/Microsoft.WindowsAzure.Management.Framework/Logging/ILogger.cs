namespace Microsoft.WindowsAzure.Management.Framework.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The main interfaces used for logging within the system.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a "message" level severity with "normal" verbosity.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        void LogMessage(string message);

        /// <summary>
        /// Adds a log writer to the logger.
        /// </summary>
        /// <param name="writer">
        /// The log writer.
        /// </param>
        void AddWriter(ILogWriter writer);
    }
}
