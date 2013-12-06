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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Logging
{
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    internal class PowershellLogWriter : IBufferingLogWriter
    {
        private readonly List<string> buffer = new List<string>();
        private readonly Queue<string> messageLines = new Queue<string>();

        public IEnumerable<string> Buffer
        {
            get { return this.buffer; }
        }

        public IEnumerable<string> DequeueBuffer()
        {
            var results = new List<string>();
            lock (this.messageLines)
            {
                while (this.messageLines.Count > 0)
                {
                    results.Add(this.messageLines.Dequeue());
                }
            }
            return results;
        }

        public void Log(Severity severity, Verbosity verbosity, string content)
        {
            string msg = string.Format(CultureInfo.InvariantCulture, "Severity: {0}\r\n{1}", severity, content);
            this.Write(msg);
        }

        protected void Write(string content)
        {
            lock (this.messageLines)
            {
                this.messageLines.Enqueue(content);
                this.buffer.Add(content);
            }
        }
    }
}
