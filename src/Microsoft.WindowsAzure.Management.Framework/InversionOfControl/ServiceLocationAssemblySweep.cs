﻿namespace Microsoft.WindowsAzure.Management.Framework.InversionOfControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal class ServiceLocationAssemblySweep
    {
        private List<Type> knownRegistrars = new List<Type>();

        public bool NewAssembliesPresent()
        {
            var registrars = this.GetRegistrarTypes().ToList();
            this.knownRegistrars.All(r => registrars.Remove(r));
            return registrars.Any();
        }

        public IEnumerable<IServiceLocationRegistrar> GetRegistrars()
        {
            var registrars = this.GetRegistrarTypes().ToList();
            this.knownRegistrars.All(r => registrars.Remove(r));
            this.knownRegistrars.AddRange(registrars);

            var objects = (from t in registrars
                           select (IServiceLocationRegistrar)Activator.CreateInstance(t)).ToList();
            return objects;
        }

        internal IEnumerable<Type> GetRegistrarTypes()
        {
            List<Type> types = new List<Type>();
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in asms)
            {
                try
                {
                    types.AddRange(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }
            var registrarTypes = (from t in types
                                  where typeof(IServiceLocationRegistrar).IsAssignableFrom(t) &&
                                        t.IsInterface == false
                                  select t).ToList();
            return registrarTypes;
        }
    }
}
