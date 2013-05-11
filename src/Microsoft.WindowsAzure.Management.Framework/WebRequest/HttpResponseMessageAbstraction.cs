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

namespace Microsoft.WindowsAzure.Management.Framework.WebRequest
{
    using System.Net;
    using System.Net.Http;

    internal class HttpResponseMessageAbstraction : DisposableObject, IHttpResponseMessageAbstraction
    {
        internal HttpResponseMessage ResponseMessage { get; private set; }

        private string content;

        internal HttpResponseMessageAbstraction(HttpResponseMessage responseMessage)
        {
            this.ResponseMessage = responseMessage;
        }

        public HttpStatusCode StatusCode
        {
            get { return this.ResponseMessage.StatusCode; }
            set { this.ResponseMessage.StatusCode = value; }
        }

        public string Content
        {
            get
            {
                if (this.ResponseMessage.Content.IsNotNull())
                {
                    this.content = this.ResponseMessage.Content.ReadAsStringAsync().WaitForResult();
                }
                return this.content;
            }
        }
    }
}
