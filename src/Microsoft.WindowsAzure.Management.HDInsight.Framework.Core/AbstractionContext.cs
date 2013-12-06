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
namespace Microsoft.WindowsAzure.Management.HDInsight
{
    using System;
    using System.Threading;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    /// <summary>
    /// Abstraction context that containst context to be passed into the internal abstractions.
    /// </summary>
    public class AbstractionContext : DisposableObject, IAbstractionContext
    {
        private readonly bool useUserToken = false;
        private CancellationToken userSetToken;

        /// <summary>
        /// Initializes a new instance of the AbstractionContext class.
        /// </summary>
        /// <param name="tokenSource">A Cancellation token source.</param>
        /// <param name="logger">A logger instance.</param>
        public AbstractionContext(CancellationTokenSource tokenSource, ILogger logger)
        {
            tokenSource.ArgumentNotNull("tokenSource");
            logger.ArgumentNotNull("logger");
            this.CancellationTokenSource = tokenSource;
            this.Logger = logger;
        }

        /// <summary>
        /// Initializes a new instance of the AbstractionContext class.
        /// </summary>
        /// <param name="token">A Cancellation token.</param>
        internal AbstractionContext(CancellationToken token)
        {
            this.userSetToken = token;
            this.useUserToken = true;
            this.Logger = new Logger();
        }

        /// <summary>
        /// Initializes a new instance of the AbstractionContext class.
        /// </summary>
        /// <param name="original">An abstraction context to clone.</param>
        public AbstractionContext(IAbstractionContext original)
        {
            if (ReferenceEquals(original, null))
            {
                throw new ArgumentNullException("original");
            }

            this.userSetToken = original.CancellationToken;
            this.Logger = original.Logger;
        }

        /// <summary>
        /// Gets a cancellation token to cancel any running requests.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get
            {
                if (this.userSetToken.IsNotNull() && this.useUserToken)
                {
                    return this.userSetToken;
                }

                return this.CancellationTokenSource.Token;
            }
        }

        /// <summary>
        /// Gets a source to generate cancellation token to cancel any running requests.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>
        /// Gets a logger to write log messages to.
        /// </summary>
        public ILogger Logger { get; private set; }
    }
}
