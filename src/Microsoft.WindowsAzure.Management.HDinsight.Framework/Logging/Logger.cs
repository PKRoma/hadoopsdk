// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.
namespace Microsoft.WindowsAzure.Management.HDInsight.Framework.Logging
{
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

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
            this.LogMessage(message, Severity.Message, Verbosity.Normal);
        }

        public void LogMessage(string message, Severity severity, Verbosity verbosity)
        {
            foreach (var logWriter in this.writers)
            {
                logWriter.Log(severity, verbosity, message);
            }
        }

        /// <inheritdoc />
        public void AddWriter(ILogWriter writer)
        {
            this.writers.Add(writer);
        }
    }
}
