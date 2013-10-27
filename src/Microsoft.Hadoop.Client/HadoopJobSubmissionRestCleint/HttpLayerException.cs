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
    using System.Globalization;
    using System.Net;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception that will be raise either when there is an exception on 
    /// the Rest client or when the results != OK.
    /// </summary>
    [Serializable]
    public class HttpLayerException : Exception
    {
        /// <summary>
        /// Gets the StatusCode of the request.
        /// </summary>
        public HttpStatusCode RequestStatusCode { get; private set; }

        /// <summary>
        /// Gets error details of the exception.
        /// Only used per inharitance request.
        /// </summary>
        public string RequestContent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HttpLayerException class.
        /// </summary>
        public HttpLayerException() :
            base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpLayerException class.
        /// </summary>
        /// <param name="message">Message for the base class.</param>
        public HttpLayerException(string message) :
            base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpLayerException class.
        /// </summary>
        /// <param name="message">Message for the base class.</param>
        /// <param name="innerException">Inner Exception.</param>
        public HttpLayerException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpLayerException class.
        /// </summary>
        /// <param name="statusCode">Status code recieved from the failed request.</param>
        /// <param name="content">Content of the failed request.</param>
        public HttpLayerException(System.Net.HttpStatusCode statusCode, string content) :
            base(string.Format(
                               CultureInfo.InvariantCulture,
                               "Request failed with code:{0}\r\nContent:{1}",
                                statusCode,
                                content ?? "(null)"))
        {
            this.RequestStatusCode = statusCode;
            this.RequestContent = content;
        }

        /// <summary>
        /// Initializes a new instance of the HttpLayerException class from deserialization.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected HttpLayerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.RequestStatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), info.GetString("RequestStatusCode"));
            this.RequestContent = info.GetString("RequestContent");
        }

        /// <summary>
        /// Serializes this object.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        public new virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("RequestStatusCode", this.RequestStatusCode);
            info.AddValue("RequestContent", this.RequestContent);
        }
    }
}