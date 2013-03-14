//
// <copyright file="RestHelpers.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Specialized;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;

namespace Microsoft.Samples.ServiceHosting.StorageClient
{
    internal static partial class Utilities
    {
        internal static HttpWebRequest CreateHttpRequest(Uri uri, string httpMethod, TimeSpan timeout)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Timeout = (int)timeout.TotalMilliseconds;
            request.ReadWriteTimeout = (int)timeout.TotalMilliseconds;
            request.Method = httpMethod;
            request.ContentLength = 0;
            request.Headers.Add(StorageHttpConstants.HeaderNames.StorageDateTime,
                                Utilities.ConvertDateTimeToHttpString(DateTime.UtcNow));
            return request;
        }

        /// <summary>
        /// Converts the date time to a valid string form as per HTTP standards
        /// </summary>
        internal static string ConvertDateTimeToHttpString(DateTime dateTime)
        {
            // On the wire everything should be represented in UTC. This assert will catch invalid callers who
            // are violating this rule.
            Debug.Assert(dateTime == DateTime.MaxValue || dateTime == DateTime.MinValue || dateTime.Kind == DateTimeKind.Utc);

            // 'R' means rfc1123 date which is what our server uses for all dates...
            // It will be in the following format:
            // Sun, 28 Jan 2008 12:11:37 GMT
            return dateTime.ToString("R", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parse a string having the date time information in acceptable formats according to HTTP standards
        /// </summary>
        internal static bool TryGetDateTimeFromHttpString(string dateString, out DateTime? result)
        {
            DateTime dateTime;
            result = null;

            // 'R' means rfc1123 date which is the preferred format used in HTTP
            bool parsed = DateTime.TryParseExact(dateString, "R", null, DateTimeStyles.None, out dateTime);
            if (parsed)
            {
                // For some reason, format string "R" makes the DateTime.Kind as Unspecified while it's actually
                // Utc. Specifying DateTimeStyles.AssumeUniversal also doesn't make the difference. If we also
                // specify AdjustToUniversal it works as expected but we don't really want Parse to adjust 
                // things automatically.
                result = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Copies from one stream to another
        /// </summary>
        /// <param name="sourceStream">The stream to copy from</param>
        /// <param name="destinationStream">The stream to copy to</param>
        internal static long CopyStream(Stream sourceStream, Stream destinationStream)
        {
            const int BufferSize = 0x10000;
            byte[] buffer = new byte[BufferSize];
            int n = 0;
            long totalRead = 0;
            do
            {
                n = sourceStream.Read(buffer, 0, BufferSize);
                if (n > 0)
                {
                    totalRead += n;
                    destinationStream.Write(buffer, 0, n);
                }
                
            } while (n > 0);
            return totalRead;
        }

        internal static void CopyStream(Stream sourceStream, Stream destinationStream, long length)
        {
            const int BufferSize = 0x10000;
            byte[] buffer = new byte[BufferSize];
            int n = 0;
            long amountLeft = length;           

            do
            {
                amountLeft -= n;
                n = sourceStream.Read(buffer, 0, (int)Math.Min(BufferSize, amountLeft));
                if (n > 0)
                {
                    destinationStream.Write(buffer, 0, n);
                }

            } while (n > 0);
        }

        internal static int CopyStreamToBuffer(Stream sourceStream, byte[] buffer, int bytesToRead)
        {
            int n = 0;
            int amountLeft = bytesToRead;
            do
            {
                n = sourceStream.Read(buffer, bytesToRead - amountLeft, amountLeft);
                amountLeft -= n;
            } while (n > 0);
            return bytesToRead - amountLeft;
        }

        internal static Uri CreateRequestUri(
                                Uri baseUri,
                                bool usePathStyleUris,
                                string accountName,
                                string containerName,
                                string blobName,
                                TimeSpan timeout,
                                NameValueCollection queryParameters,
                                out ResourceUriComponents uriComponents
                                )
        {
            uriComponents = 
                new ResourceUriComponents(accountName, containerName, blobName);
            Uri uri = HttpRequestAccessor.ConstructResourceUri(baseUri, uriComponents, usePathStyleUris);

            if (queryParameters != null)
            {
                UriBuilder builder = new UriBuilder(uri);

                if (queryParameters.Get(StorageHttpConstants.QueryParams.QueryParamTimeout) == null)
                {
                    queryParameters.Add(StorageHttpConstants.QueryParams.QueryParamTimeout,
                    timeout.TotalSeconds.ToString(CultureInfo.InvariantCulture));
                }

                StringBuilder sb = new StringBuilder();
                bool firstParam = true;
                foreach (string queryKey in queryParameters.AllKeys)
                {
                    if (!firstParam)
                        sb.Append("&");
                    sb.Append(HttpUtility.UrlEncode(queryKey));
                    sb.Append('=');
                    sb.Append(HttpUtility.UrlEncode(queryParameters[queryKey]));
                    firstParam = false;
                }

                if (sb.Length > 0)
                {
                    builder.Query = sb.ToString();
                }
                return builder.Uri;
            }
            else
            {
                return uri;
            }
        }

        internal static bool StringIsIPAddress(string address)
        {
            try
            {
                IPAddress.Parse(address);

                // No exception means it's a valid IP address. 
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        internal static void AddMetadataHeaders(HttpWebRequest request, NameValueCollection metadata)
        {
            foreach (string key in metadata.Keys)
            {
                request.Headers.Add(
                    StorageHttpConstants.HeaderNames.PrefixForMetadata + key,
                    metadata[key]
                    );
            }
        }

        internal static bool IsValidTableName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            Regex reg = new Regex(StorageHttpConstants.RegularExpressionStrings.ValidTableNameRegex);
            if (reg.IsMatch(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static bool IsValidContainerOrQueueName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            Regex reg = new Regex(StorageHttpConstants.RegularExpressionStrings.ValidContainerNameRegex);
            if (reg.IsMatch(name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
