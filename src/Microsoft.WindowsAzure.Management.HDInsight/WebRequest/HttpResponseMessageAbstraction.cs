namespace Microsoft.WindowsAzure.Management.HDInsight.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework;

    internal class HttpResponseMessageAbstraction : DisposableObject, IHttpResponseMessageAbstraction
    {
        internal HttpResponseMessage ResponseMessage { get; private set; }
        
        private string content = null;

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
