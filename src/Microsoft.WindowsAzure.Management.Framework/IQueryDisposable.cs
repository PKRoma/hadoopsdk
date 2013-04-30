// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Microsoft.WindowsAzure.Management.Framework
{
    using System;

    /// <summary>
   ///    Extends <see cref="IDisposable" /> to expose the status.
   /// </summary>
   public interface IQueryDisposable : IDisposable
   {
      /// <summary>
      ///    Determines whether this instance is disposed.
      /// </summary>
      /// <returns> <c>true</c> if this instance is disposed; otherwise, <c>false</c> . </returns>
      bool IsDisposed();
   }
}
