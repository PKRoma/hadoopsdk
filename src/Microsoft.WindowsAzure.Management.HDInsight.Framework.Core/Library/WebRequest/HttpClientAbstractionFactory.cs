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
namespace Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library.WebRequest
{
    using System.Security.Cryptography.X509Certificates;

    internal class HttpClientAbstractionFactory : HttpClientAbstractionFactoryBase
    {
        public override IHttpClientAbstraction Create(IAbstractionContext context)
        {
            return HttpClientAbstraction.Create(context);
        }

        public override IHttpClientAbstraction Create(X509Certificate2 cert)
        {
            return HttpClientAbstraction.Create(cert);
        }

        public override IHttpClientAbstraction Create(X509Certificate2 cert, IAbstractionContext context)
        {
            return HttpClientAbstraction.Create(cert, context);
        }

        public override IHttpClientAbstraction Create(string token)
        {
            return HttpClientAbstraction.Create(token);
        }

        public override IHttpClientAbstraction Create(string token, IAbstractionContext context)
        {
            return HttpClientAbstraction.Create(token, context);
        }

        public override IHttpClientAbstraction Create()
        {
            return HttpClientAbstraction.Create();
        }
    }
}
