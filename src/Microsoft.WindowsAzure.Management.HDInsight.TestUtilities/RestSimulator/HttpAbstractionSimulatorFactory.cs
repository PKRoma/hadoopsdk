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
namespace Microsoft.WindowsAzure.Management.HDInsight.TestUtilities.RestSimulator
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library.WebRequest;

    internal class HttpAbstractionSimulatorFactory : DisposableObject, IHttpClientAbstractionFactory
    {
        public HttpAbstractionSimulatorFactory(IHttpClientAbstractionFactory underlying)
        {
            this.underlying = underlying;
            this.Clients = new List<Tuple<IHttpClientAbstraction, IHttpResponseMessageAbstraction>>();
        }

        public Func<IHttpClientAbstraction, IHttpResponseMessageAbstraction> AsyncMock { get; set; }

        private IHttpClientAbstractionFactory underlying;

        public ICollection<Tuple<IHttpClientAbstraction, IHttpResponseMessageAbstraction>> Clients { get; private set; }

        public IHttpClientAbstraction Create(X509Certificate2 cert)
        {
            var loc = this.AsyncMock;
            if (loc.IsNotNull())
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(cert), loc);
            }
            else
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(cert), null);
            }
        }

        public IHttpClientAbstraction Create(X509Certificate2 cert, HDInsight.IAbstractionContext context)
        {
            var loc = this.AsyncMock;
            if (loc.IsNotNull())
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(cert, context), loc);
            }
            else
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(cert, context), null);
            }
        }

        public IHttpClientAbstraction Create(string token)
        {
            var loc = this.AsyncMock;
            if (loc.IsNotNull())
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(token), loc);
            }
            else
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(token), null);
            }
        }

        public IHttpClientAbstraction Create(string token, HDInsight.IAbstractionContext context)
        {
            var loc = this.AsyncMock;
            if (loc.IsNotNull())
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(token, context), loc);
            }
            else
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(token, context), null);
            }
        }

        public IHttpClientAbstraction Create()
        {
            var loc = this.AsyncMock;
            if (loc.IsNotNull())
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(), loc);
            }
            else
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(), null);
            }
        }

        public IHttpClientAbstraction Create(HDInsight.IAbstractionContext context)
        {
            var loc = this.AsyncMock;
            if (loc.IsNotNull())
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(context), loc);
            }
            else
            {
                return new HttpAbstractionSimulatorClient(this, this.underlying.Create(context), null);
            }
        }
    }
}
