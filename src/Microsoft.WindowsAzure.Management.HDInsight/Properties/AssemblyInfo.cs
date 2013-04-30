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

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Microsoft.Hadoop.WebClient")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Microsoft.Hadoop.WebClient")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: Guid("018f9996-42cc-4b94-8206-3353f1689318")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]
[assembly: NeutralResourcesLanguage("en-US")]

#if SIGN
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=002400000480000094000000060200000024000052534131000400000100010077F5E87030DADCCCE6902C6ADAB7A987BD69CB5819991531F560785EACFC89B6FCDDF6BB2A00743A7194E454C0273447FC6EEC36474BA8E5A3823147D214298E4F9A631B1AFEE1A51FFEAE4672D498F14B000E3D321453CDD8AC064DE7E1CF4D222B7E81F54D4FD46725370D702A05B48738CC29D09228F1AA722AE1A9CA02FB")]
#else
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif