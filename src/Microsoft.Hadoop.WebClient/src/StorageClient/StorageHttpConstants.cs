using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.ServiceHosting.StorageClient.StorageHttpConstants
{
   internal static class ConstChars
   {
      internal const string Linefeed = "\n";
      internal const string CarriageReturnLinefeed = "\r\n";
      internal const string Colon = ":";
      internal const string Comma = ",";
      internal const string Slash = "/";
      internal const string BackwardSlash = @"\";
      internal const string Space = " ";
      internal const string Ampersand = "&";
      internal const string QuestionMark = "?";
      internal const string Equal = "=";
      internal const string Bang = "!";
      internal const string Star = "*";
      internal const string Dot = ".";
   }

   internal static class RequestParams
   {
      internal const string NumOfMessages = "numofmessages";
      internal const string VisibilityTimeout = "visibilitytimeout";
      internal const string PeekOnly = "peekonly";
      internal const string MessageTtl = "messagettl";
      internal const string Messages = "messages";
      internal const string PopReceipt = "popreceipt";
   }

   internal static class QueryParams
   {
      internal const string SeparatorForParameterAndValue = "=";
      internal const string QueryParamTimeout = "timeout";
      internal const string QueryParamComp = "comp";

      // Other query string parameter names
      internal const string QueryParamBlockId = "blockid";
      internal const string QueryParamPrefix = "prefix";
      internal const string QueryParamMarker = "marker";
      internal const string QueryParamMaxResults = "maxresults";
      internal const string QueryParamDelimiter = "delimiter";
      internal const string QueryParamModifiedSince = "modifiedsince";
      internal const string QueryParemInclude = "include";
   }

   internal static class CompConstants
   {
      internal const string Metadata = "metadata";
      internal const string List = "list";
      internal const string BlobList = "bloblist";
      internal const string BlockList = "blocklist";
      internal const string Block = "block";
      internal const string Acl = "acl";
   }

   internal static class XmlElementNames
   {
      internal const string BlockList = "BlockList";
      internal const string Block = "Block";
      internal const string EnumerationResults = "EnumerationResults";
      internal const string Prefix = "Prefix";
      internal const string Marker = "Marker";
      internal const string MaxResults = "MaxResults";
      internal const string Delimiter = "Delimiter";
      internal const string NextMarker = "NextMarker";
      internal const string Containers = "Containers";
      internal const string Container = "Container";
      internal const string ContainerName = "Name";
      internal const string ContainerNameAttribute = "ContainerName";
      internal const string AccountNameAttribute = "AccountName";
      internal const string LastModified = "LastModified";
      internal const string Etag = "Etag";
      internal const string Url = "Url";
      internal const string CommonPrefixes = "CommonPrefixes";
      internal const string ContentType = "ContentType";
      internal const string ContentEncoding = "ContentEncoding";
      internal const string ContentLanguage = "ContentLanguage";
      internal const string Size = "Size";
      internal const string Blobs = "Blobs";
      internal const string Blob = "Blob";
      internal const string BlobName = "Name";
      internal const string BlobPrefix = "BlobPrefix";
      internal const string BlobPrefixName = "Name";
      internal const string Name = "Name";
      internal const string Queues = "Queues";
      internal const string Queue = "Queue";
      internal const string QueueName = "QueueName";
      internal const string QueueMessagesList = "QueueMessagesList";
      internal const string QueueMessage = "QueueMessage";
      internal const string MessageId = "MessageId";
      internal const string PopReceipt = "PopReceipt";
      internal const string InsertionTime = "InsertionTime";
      internal const string ExpirationTime = "ExpirationTime";
      internal const string TimeNextVisible = "TimeNextVisible";
      internal const string MessageText = "MessageText";

      // Error specific constants
      internal const string ErrorRootElement = "Error";
      internal const string ErrorCode = "Code";
      internal const string ErrorMessage = "Message";
      internal const string ErrorException = "ExceptionDetails";
      internal const string ErrorExceptionMessage = "ExceptionMessage";
      internal const string ErrorExceptionStackTrace = "StackTrace";
      internal const string AuthenticationErrorDetail = "AuthenticationErrorDetail";

      //The following are for table error messages
      internal const string DataWebMetadataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
      internal const string TableErrorCodeElement = "code";
      internal const string TableErrorMessageElement = "message";
   }

   internal static class HeaderNames
   {
      internal const string PrefixForStorageProperties = "x-ms-prop-";
      internal const string PrefixForMetadata = "x-ms-meta-";
      internal const string PrefixForStorageHeader = "x-ms-";
      internal const string PrefixForTableContinuation = "x-ms-continuation-";

      //
      // Standard headers...
      //
      internal const string ContentLanguage = "Content-Language";
      internal const string ContentLength = "Content-Length";
      internal const string ContentType = "Content-Type";
      internal const string ContentEncoding = "Content-Encoding";
      internal const string ContentMD5 = "Content-MD5";
      internal const string ContentRange = "Content-Range";
      internal const string LastModifiedTime = "Last-Modified";
      internal const string Server = "Server";
      internal const string Allow = "Allow";
      internal const string ETag = "ETag";
      internal const string Range = "Range";
      internal const string Date = "Date";
      internal const string Authorization = "Authorization";
      internal const string IfModifiedSince = "If-Modified-Since";
      internal const string IfUnmodifiedSince = "If-Unmodified-Since";
      internal const string IfMatch = "If-Match";
      internal const string IfNoneMatch = "If-None-Match";
      internal const string IfRange = "If-Range";
      internal const string NextPartitionKey = "NextPartitionKey";
      internal const string NextRowKey = "NextRowKey";
      internal const string NextTableName = "NextTableName";

      //
      // Storage specific custom headers...
      //
      internal const string StorageDateTime = PrefixForStorageHeader + "date";
      internal const string PublicAccess = PrefixForStorageProperties + "publicaccess";
      internal const string StorageRange = PrefixForStorageHeader + "range";

      internal const string CreationTime = PrefixForStorageProperties + "creation-time";
      internal const string ForceUpdate = PrefixForStorageHeader + "force-update";
      internal const string ApproximateMessagesCount = PrefixForStorageHeader + "approximate-messages-count";
   }

   internal static class HeaderValues
   {
      internal const string ContentTypeXml = "application/xml";

      /// <summary>
      /// This is the default content-type xStore uses when no content type is specified
      /// </summary>
      internal const string DefaultContentType = "application/octet-stream";

      // The Range header value is "bytes=start-end", both start and end can be empty
      internal const string RangeHeaderFormat = "bytes={0}-{1}";

   }

   internal static class AuthenticationSchemeNames
   {
      internal const string SharedKeyAuthSchemeName = "SharedKey";
      internal const string SharedKeyLiteAuthSchemeName = "SharedKeyLite";
   }

   internal static class HttpMethod
   {
      internal const string Get = "GET";
      internal const string Put = "PUT";
      internal const string Post = "POST";
      internal const string Head = "HEAD";
      internal const string Delete = "DELETE";
      internal const string Trace = "TRACE";
      internal const string Options = "OPTIONS";
      internal const string Connect = "CONNECT";
   }

   internal static class BlobBlockConstants
   {
      internal const int KB = 1024;
      internal const int MB = 1024 * KB;
      /// <summary>
      /// When transmitting a blob that is larger than this constant, this library automatically
      /// transmits the blob as individual blocks. I.e., the blob is (1) partitioned
      /// into separate parts (these parts are called blocks) and then (2) each of the blocks is 
      /// transmitted separately.
      /// The maximum size of this constant as supported by the real blob storage service is currently 
      /// 64 MB; the development storage tool currently restricts this value to 2 MB.
      /// Setting this constant can have a significant impact on the performance for uploading or
      /// downloading blobs.
      /// As a general guideline: If you run in a reliable environment increase this constant to reduce
      /// the amount of roundtrips. In an unreliable environment keep this constant low to reduce the 
      /// amount of data that needs to be retransmitted in case of connection failures.
      /// </summary>
      internal const long MaximumBlobSizeBeforeTransmittingAsBlocks = 2 * MB;
      /// <summary>
      /// The size of a single block when transmitting a blob that is larger than the 
      /// MaximumBlobSizeBeforeTransmittingAsBlocks constant (see above).
      /// The maximum size of this constant is currently 4 MB; the development storage 
      /// tool currently restricts this value to 1 MB.
      /// Setting this constant can have a significant impact on the performance for uploading or 
      /// downloading blobs.
      /// As a general guideline: If you run in a reliable environment increase this constant to reduce
      /// the amount of roundtrips. In an unreliable environment keep this constant low to reduce the 
      /// amount of data that needs to be retransmitted in case of connection failures.
      /// </summary>
      internal const long BlockSize = 1 * MB;
   }

   internal static class ListingConstants
   {
      internal const int MaxContainerListResults = 5000;
      internal const int MaxBlobListResults = 5000;
      internal const int MaxQueueListResults = 50;
      internal const int MaxTableListResults = 50;
   }

   /// <summary>
   /// Contains regular expressions for checking whether container and table names conform
   /// to the rules of the storage REST protocols.
   /// </summary>
   public static class RegularExpressionStrings
   {
      /// <summary>
      /// Container or queue names that match against this regular expression are valid.
      /// </summary>
      public const string ValidContainerNameRegex = @"^([a-z]|\d){1}([a-z]|-|\d){1,61}([a-z]|\d){1}$";

      /// <summary>
      /// Table names that match against this regular expression are valid.
      /// </summary>
      public const string ValidTableNameRegex = @"^([a-z]|[A-Z]){1}([a-z]|[A-Z]|\d){2,62}$";
   }

   internal static class StandardPortalEndpoints
   {
      internal const string BlobStorage = "blob";
      internal const string QueueStorage = "queue";
      internal const string TableStorage = "table";
      internal const string StorageHostSuffix = ".core.windows.net";
      internal const string BlobStorageEndpoint = BlobStorage + StorageHostSuffix;
      internal const string QueueStorageEndpoint = QueueStorage + StorageHostSuffix;
      internal const string TableStorageEndpoint = TableStorage + StorageHostSuffix;
   }
}
