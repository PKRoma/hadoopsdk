// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Microsoft.WindowsAzure.Management.Framework
{
    using System;

    /// <summary>
   ///    Instructs Code Analysis to treat a method as a validation
   ///    method for a given parameter and not fire 1062 when it is used.
   /// </summary>
   [AttributeUsage(AttributeTargets.Parameter)]
   public sealed class ValidatedNotNullAttribute : Attribute
   {
   }
}
