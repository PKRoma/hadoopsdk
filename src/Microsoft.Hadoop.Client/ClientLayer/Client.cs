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
namespace Microsoft.Hadoop.Client
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    /// <summary>
    /// Represents the base of all Client objects.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
        Justification = "DisposableObject implements IDisposable correctly, the implementation of IDisposable in the interfaces is necessary for the design.")]
    public abstract class ClientBase : DisposableObject, IJobSubmissionClientBase
    {
        private ILogger logger;
        private CancellationTokenSource source;
        private IAbstractionContext abstractionContext;

        /// <summary>
        /// Gets the Abstraction context to be used to control cancellation and log writing.
        /// </summary>
        protected IAbstractionContext Context
        {
            get
            {
                return this.abstractionContext;
            }
        }

        /// <summary>
        /// Gets or sets the CancellationTokenSource to be used to control cancellation.
        /// </summary>
        public CancellationTokenSource CancellationSource
        {
            get { return this.source; }
            set { this.SetCancellationSource(value); }
        }

        /// <summary>
        /// Gets the CancellationToken to be used to control cancellation.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get { return this.CancellationSource.Token; }
        }

        /// <summary>
        /// Gets the Logger to be used to log messages.
        /// </summary>
        protected ILogger Logger
        {
            get
            {
                if (this.logger == null)
                {
                    this.logger = new Logger();
                }

                return this.logger;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ClientBase class.
        /// </summary>
        protected ClientBase()
        {
            this.SetCancellationSource(Help.SafeCreate<CancellationTokenSource>());
        }

        /// <inheritdoc />
        public void Cancel()
        {
            var source = this.CancellationSource;
            if (source.IsNotNull())
            {
                source.Cancel();
            }
        }

        /// <inheritdoc />
        public void SetCancellationSource(CancellationTokenSource tokenSource)
        {
            if (ReferenceEquals(tokenSource, null))
            {
                throw new ArgumentNullException("tokenSource");
            }

            var oldSource = this.CancellationSource;
            if (oldSource.IsNotNull())
            {
                oldSource.Dispose();
            }

            this.abstractionContext = Help.SafeCreate(() => new AbstractionContext(tokenSource, this.Logger));
            tokenSource.Token.Register(this.CancellationCallback);
            this.source = tokenSource;
        }

        /// <inheritdoc />
        public void AddLogWriter(ILogWriter logWriter)
        {
            logWriter.ArgumentNotNull("logWriter");
            this.Logger.AddWriter(logWriter);
        }

        private void CancellationCallback()
        {
            this.SetCancellationSource(Help.SafeCreate<CancellationTokenSource>());
        }
    }
}
