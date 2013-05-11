namespace Microsoft.WindowsAzure.Management.Framework.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;

    /// <summary>
    /// Registers services with the IOC for use by this assembly.
    /// </summary>
    public class HttpAbstractionServiceLocationRegistrar : IServiceLocationRegistrar 
    {
        /// <inheritdoc />
        public void Register(IIocServiceLocationRuntimeManager manager)
        {
            if (manager.IsNull())
            {
                throw new ArgumentNullException("manager");
            }

            manager.RegisterType<IHttpClientAbstractionFactory, HttpClientAbstractionFactory>();
        }
    }
}
