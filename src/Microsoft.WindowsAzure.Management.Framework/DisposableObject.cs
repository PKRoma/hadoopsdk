﻿// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Microsoft.WindowsAzure.Management.Framework
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
   ///    Base implementation of a disposable object.
   /// </summary>
   public abstract class DisposableObject : IQueryDisposable
   {
      private InterlockedBoolean disposed = new InterlockedBoolean(false);

      /// <summary>
      ///    Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Altered to be thread-safe(MWP)")]
      public void Dispose()
      {
         if (this.disposed.ExchangeValue(true))
         {
            // already disposed or disposing;
            return;
         }

         this.Dispose(true);

         // Use SupressFinalize in case a subclass
         // of this type implements a finalizer.
         GC.SuppressFinalize(this);
      }

      /// <summary>
      ///    Releases unmanaged and - optionally - managed resources.
      /// </summary>
      /// <param name="disposing"> Use <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources. </param>
      protected virtual void Dispose(bool disposing)
      {
         if (disposing)
         {
            // Clean up managed resources if disposing
            this.ReleaseManagedResources();
         }

         // Clean up native resources always
         this.ReleaseUnmanagedResources();
      }

      /// <summary>
      ///    Finalizes an instance of the <see cref="DisposableObject" /> class.
      /// </summary>
      /// <remarks>
      ///    Releases unmanaged resources and performs other cleanup operations before the
      ///    <see cref="DisposableObject" /> is reclaimed by garbage collection.
      /// </remarks>
      ~DisposableObject()
      {
         this.Dispose(false);
      }

      /// <inheritdoc />
      public bool IsDisposed()
      {
         return this.disposed.GetValue();
      }

      /// <summary>
      ///    Releases the managed resources.
      /// </summary>
      protected virtual void ReleaseManagedResources()
      {
          var derrivedType = this.GetType();
          var fields = derrivedType.GetFields();
          foreach (var field in fields)
          {
              var value = field.GetValue(this);
              if (value.IsNotNull())
              {
                  var valueType = value.GetType();
                  if (!valueType.IsValueType)
                  {
                      var asDisposable = value as IDisposable;
                      if (asDisposable.IsNotNull())
                      {
                          asDisposable.Dispose();
                          field.SetValue(this, null);
                      }
                  }
              }
          }
      }

      /// <summary>
      ///    Releases the unmanaged resources.
      /// </summary>
      protected virtual void ReleaseUnmanagedResources()
      {
      }

      /// <summary>
      ///    Checks the disposed.
      /// </summary>
      protected void CheckDisposed()
      {
         if (this.disposed.GetValue())
         {
            throw new ObjectDisposedException(this.GetType().Name);
         }
      }
   }
}
