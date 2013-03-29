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
// permissions and limitations under the License.using System;

namespace Microsoft.Hadoop.WebClient.OozieClient
{
    internal static class OozieResources
    {
        internal const string RelativePath = "oozie/v1/";
        internal const string Status = "admin/status";
        internal const string Jobs = "jobs";
        internal const string Job = "job";
        internal const string ActionStart = "start";
        internal const string ActionKill = "kill";
        internal const string ActionSuspend = "suspend";
        internal const string ActionResume = "resume";
        internal const string ShowInfo = "info";
    }
}
