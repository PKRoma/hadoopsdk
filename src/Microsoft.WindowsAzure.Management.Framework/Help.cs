namespace Microsoft.WindowsAzure.Management.Framework
{
    using System;

    /// <summary>
    /// Helper class to provide some useful functions.
    /// </summary>
    public static class Help
    {
        /// <summary>
        /// Safely creates a disposable object with a default constructor.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to create.
        /// </typeparam>
        /// <returns>
        /// The disposable object that has been safely created.
        /// </returns>
        public static T SaveCreate<T>() where T : class, IDisposable, new()
        {
            T retval = null;
            try
            {
                retval = new T();
            }
            catch (Exception)
            {
                if (retval.IsNotNull())
                {
                    retval.Dispose();
                }
                throw;
            }
            return retval;
        }

        /// <summary>
        /// Safely creates a disposable object with a custom constructor.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to create.
        /// </typeparam>
        /// <param name="factory">
        /// The factory method used to construct the object.
        /// </param>
        /// <returns>
        /// The disposable object that has been safely created.
        /// </returns>
        public static T SaveCreate<T>(Func<T> factory) where T : class, IDisposable
        {
            if (factory.IsNull())
            {
                throw new ArgumentNullException("factory");
            }
            T retval = null;
            try
            {
                retval = factory();
            }
            catch (Exception)
            {
                if (retval.IsNotNull())
                {
                    retval.Dispose();
                }
                throw;
            }
            return retval;
        }
    }
}
