namespace Microsoft.WindowsAzure.Management.Framework.InversionOfControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class ServiceLocationAssemblySweep
    {
        public IEnumerable<IServiceLocationRegistrar> GetRegistrars()
        {
            var objects = (from t in this.GetRegistrarTypes()
                           select (IServiceLocationRegistrar)Activator.CreateInstance(t)).ToList();
            return objects;
        }

        internal IEnumerable<Type> GetRegistrarTypes()
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            var types = (from a in asms
                         from t in a.GetTypes()
                         select t).ToList();
            var registrarTypes = (from t in types
                                  where typeof(IServiceLocationRegistrar).IsAssignableFrom(t) &&
                                        t.IsInterface == false
                                  select t).ToList();
            return registrarTypes;
        }
    }
}
