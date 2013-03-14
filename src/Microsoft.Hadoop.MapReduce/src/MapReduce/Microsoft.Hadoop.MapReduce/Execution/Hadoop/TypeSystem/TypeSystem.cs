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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Hadoop.MapReduce
{
    internal static class TypeSystem
    {
        static TypeSystem()
        {
            s_systemAssemblies = new HashSet<string>();
            s_systemAssemblies.Add("mscorlib");
            s_systemAssemblies.Add("System");
            s_systemAssemblies.Add("Accessibility");
            s_systemAssemblies.Add("SMDiagnostics");
            s_systemAssemblies.Add("MRRunner");
        }
        
        private static HashSet<string> s_systemAssemblies;

        internal static IEnumerable<string> GetLoadedNonSystemAssemblyPaths()
        {
            List<string> names = new List<string>();
            var assemblies = TypeSystem.GetAllAssemblies();
            foreach (Assembly asm in assemblies)
            {
                if (!TypeSystem.IsSystemAssembly(asm))
                {
                    names.Add(asm.Location);
                }
            }
            return names.ToArray();
        }

        internal static bool IsSystemAssembly(Assembly asm)
        {
            string name = asm.GetName().Name;

            // don't include GAC assemblies (due to path bloat)  @@TODO: review
            if(asm.Location.IndexOf(@"Windows\assembly",StringComparison.OrdinalIgnoreCase) >=0){
                return true;
            }

            //during dev at least, our API DLLs need to be shipped. @@TODO: review deployment plan.
            // NEIN: This needs to be undone.
            if (name.StartsWith("Microsoft.Hadoop") || name.StartsWith("Microsoft.CompilerServices.AsyncTargetingPack"))
            {
                return false;
            }

            return (s_systemAssemblies.Contains(name) ||
                    name.StartsWith("Microsoft.", StringComparison.Ordinal) || 
                    name.StartsWith("System.", StringComparison.Ordinal) ||
                    name == "WindowsBase"
                    );
        }

        /// <summary>
        /// Compute all referenced assemblies (transitive closure) and cache them.
        /// </summary>
        /// <returns>List of all referenced assemblies.</returns>
        internal static HashSet<Assembly> GetAllAssemblies()
        {
            if (s_allReferencedAssemblies == null)
            {
                // compute transitive closure
                HashSet<Assembly> referencedAssemblies = new HashSet<Assembly>(new AssemblyComparer());
                Queue<Assembly> toscan = new Queue<Assembly>(10);

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly asm in assemblies)
                {
                    if (!referencedAssemblies.Contains(asm) && !IsDynamicAssembly(asm))
                    {
                        toscan.Enqueue(asm);
                        referencedAssemblies.Add(asm);
                    }
                }

                while (toscan.Count > 0)
                {
                    Assembly asm = toscan.Dequeue();
                    AssemblyName[] names = asm.GetReferencedAssemblies();
                    foreach (AssemblyName asmName in names)
                    {
                        try
                        {
                            Assembly refAssembly = Assembly.ReflectionOnlyLoad(asmName.FullName);
                            if (!referencedAssemblies.Contains(refAssembly, new AssemblyComparer()))
                            {
                                toscan.Enqueue(refAssembly);
                                referencedAssemblies.Add(refAssembly);
                            }
                        }
                        catch (Exception)
                        {
                            // Console.WriteLine("Warning: Could not load referenced assembly " + asmName);
                        }
                    }
                }

                // Due to the use of FullName in the ReflectionOnlyLoad call, we may end up with multiple versions of the same assembly in the list 
                // We need to filter the list by selecting the newest version of each assembly
                var newestAssemblies = referencedAssemblies.GroupBy(asm => asm.GetName().Name).Select(grp => grp.OrderByDescending(asm => asm.GetName().Version).First());
                s_allReferencedAssemblies = new HashSet<Assembly>(new AssemblyComparer());
                foreach (var asm in newestAssemblies)
                {
                    s_allReferencedAssemblies.Add(asm);
                }
            }
            return s_allReferencedAssemblies;
        }

        /// <summary>
        /// Compare two assemblies for equality.
        /// </summary>
        internal class AssemblyComparer : IEqualityComparer<Assembly>
        {
            public bool Equals(Assembly x, Assembly y)
            {
                // some Assembly objects loaded as reflection only may have different pointers
                // what matters is that their fully qualified names match

                return ReferenceEquals(x, y) || x.FullName == y.FullName;
            }

            public int GetHashCode(Assembly obj)
            {
                return obj.FullName.GetHashCode();
            }
        }

        private static bool IsDynamicAssembly(Assembly asm)
        {
            if (asm is AssemblyBuilder) return true;

            try
            {
                if (asm.Location != null) return false;
            }
            catch
            {
                // if we gen an exception from asm.Location it's a dynamic assembly.
                return true;
            }

            return false;
        }

        private static HashSet<Assembly> s_allReferencedAssemblies = null;

        internal static void CheckUserType(Type userType, Type baseClass)
        {
            if (!(userType.IsPublic || userType.IsNestedPublic))
            {
                throw new StreamingException(string.Format("Type is not public: {0}", userType));
            }
            
            if (!baseClass.IsAssignableFrom(userType))
            {
                throw new StreamingException(string.Format("Type does not inherit from correct base class. actual class:{0}, expected base:{1}", userType, baseClass.FullName));
            }
        }
    }


}
