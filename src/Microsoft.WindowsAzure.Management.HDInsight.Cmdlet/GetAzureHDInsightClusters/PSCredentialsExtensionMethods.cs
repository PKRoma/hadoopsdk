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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System.Management.Automation;
    using System.Runtime.InteropServices;
    using System.Security;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    internal static class PsCredentialsExtensionMethods
    {
        internal static string GetCleartextPassword(this PSCredential credentials)
        {
            credentials.ArgumentNotNull("credentials");
            return GetCleartextFromSecureString(credentials.Password);
        }

        internal static string GetCleartextFromSecureString(SecureString secureString)
        {
            secureString.ArgumentNotNull("secureString");
            var bstr = Marshal.SecureStringToBSTR(secureString);
            string clearTextValue;
            try
            {
                clearTextValue = Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }

            return clearTextValue;
        }
    }
}
