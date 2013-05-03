namespace Microsoft.WindowsAzure.Management.Framework.InversionOfControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Allows an assembly to register services with the IoC container.
    /// </summary>
    public interface IServiceLocationRegistrar : IRegisteringService
    {
    }
}
