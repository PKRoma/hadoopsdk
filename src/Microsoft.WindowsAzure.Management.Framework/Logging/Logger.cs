namespace Microsoft.WindowsAzure.Management.Framework.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;

    /// <summary>
    /// The default implementation of the logger.  All messages are 
    /// simply passed into the LogWriter.
    /// </summary>
    internal class Logger : ILogger
    {
        private List<ILogWriter> writers = new List<ILogWriter>();

        /// <inheritdoc />
        public void LogMessage(string message)
        {
            foreach (var logWriter in this.writers)
            {
                logWriter.Log(Severity.Message, Verbosity.Normal, message);
            }
        }

        /// <inheritdoc />
        public void AddWriter(ILogWriter writer)
        {
            this.writers.Add(writer);
        }
    }
}
