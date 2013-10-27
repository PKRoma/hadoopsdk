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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System;
    using System.Management.Automation;
    using System.Text;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;

    /// <summary>
    /// The base class for HDInsight Cmdlets.
    /// </summary>
    public abstract class AzureHDInsightCmdlet : PSCmdlet
    {
        private ILogWriter logger;

        /// <inheritdoc />
        protected abstract override void StopProcessing();

        /// <summary>
        /// Gets or sets a value indicating whether logging should be enabled.
        /// </summary>
        public ILogWriter Logger
        {
            get
            {
                if (this.logger == null)
                {
                    if (this.MyInvocation.BoundParameters.ContainsKey("Debug"))
                    {
                        this.logger = ServiceLocator.Instance.Locate<IBufferingLogWriterFactory>().Create();
                    }
                    else
                    {
                        this.logger = new NullLogWriter();
                    }
                }

                return this.logger;
            }

            set
            {
                this.logger = value;
            }
        }

        /// <summary>
        /// Formats an exception to be placed in the debug output.
        /// </summary>
        /// <param name="ex">
        /// The exception.
        /// </param>
        /// <returns>
        /// A string that represents the message to display for the exception.
        /// </returns>
        protected string FormatException(Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            if (ex.IsNotNull())
            {
                builder.AppendLine(ex.Message);
                builder.AppendLine(ex.StackTrace);
                var aggex = ex as AggregateException;
                if (aggex.IsNotNull())
                {
                    foreach (var innerException in aggex.InnerExceptions)
                    {
                        builder.AppendLine(this.FormatException(innerException));
                    }
                }
                else if (ex.InnerException.IsNotNull())
                {
                    builder.AppendLine(this.FormatException(ex.InnerException));
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Writes any collected log messages to the debug output.
        /// </summary>
        protected void WriteDebugLog()
        {
            var bufferingLogWriter = this.Logger as IBufferingLogWriter;
            if (bufferingLogWriter.IsNotNull())
            {
                foreach (var line in bufferingLogWriter.DequeueBuffer())
                {
                    this.WriteDebug(line);
                }
            }
        }
    }
}
