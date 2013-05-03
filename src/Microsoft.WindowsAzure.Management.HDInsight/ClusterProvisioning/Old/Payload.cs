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

namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using Microsoft.WindowsAzure.Management.Framework;

    [DataContract]
    internal abstract class Payload
    {
        [DataMember(EmitDefaultValue = false)]
        internal string ExtendedProperties { get; set; }

        internal XmlReader SerializeToXmlReader()
        {
            var ser = new DataContractSerializer(this.GetType());
            var ms = Help.SaveCreate<MemoryStream>();
            try
            {
                ser.WriteObject(ms, this);
                ms.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception)
            {
                ms.Dispose();
                throw;
            }
            return Help.SaveCreate(() => XmlReader.Create(ms));
        }

        internal XmlNode SerializeToXmlNode()
        {
            var doc = new XmlDocument();
            doc.Load(this.SerializeToXmlReader());
            return doc.DocumentElement;
        }

        internal static T DeserializeFromXml<T>(string data) where T : Payload, new()
        {
            var ser = new DataContractSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                return (T)ser.ReadObject(ms);
            }
        }
    }
}