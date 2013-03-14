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
    // todo - make abstract
    public class FileChecksum : Resource
    {
        // todo - should makt these immutable.
        public string Algorithm { get; set; }
        public string Checksum { get; set; }
        public int Length { get; set; }

        public FileChecksum(JObject value)
        {
            Algorithm = value.Value<string>("algorithm");
            Checksum = value.Value<string>("bytes");
            Length = value.Value<int>("length");
            Info = value;
        }
    }
}
