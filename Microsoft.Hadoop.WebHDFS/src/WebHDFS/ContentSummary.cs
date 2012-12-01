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


using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.WebHDFS
{
    public class ContentSummary
    {
        public int DirectoryCount { get; set; }
        public int FileCount { get; set; }
        public int Length { get; set; }
        public int Quota { get; set; }
        public int SpaceConsumed { get; set; }
        public int SpaceQuota { get; set; }

        public ContentSummary(JToken value)
        {
            DirectoryCount = value.Value<int>("directoryCount");
            FileCount = value.Value<int>("fileCount");
            Length = value.Value<int>("length");
            Quota = value.Value<int>("quota");
            SpaceConsumed = value.Value<int>("spaceConsumed");
            SpaceQuota = value.Value<int>("spaceQuota");
        }
    }
}
