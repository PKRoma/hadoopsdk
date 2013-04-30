namespace Microsoft.WindowsAzure.Management.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Adds Extension methods to the ICollection(of T) objects.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Allows a range of objects IEnumerable(of T) to be added to the collection.
        /// </summary>
        /// <typeparam name="T">
        /// The Type of objects in the collection.
        /// </typeparam>
        /// <param name="collection">
        /// The collection that this extension method is extending.
        /// </param>
        /// <param name="items">
        /// The IEnumerable(of T) of objects in the collection.
        /// </param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection.IsNull())
            {
                throw new ArgumentNullException("collection");
            }
            if (items.IsNull())
            {
                throw new ArgumentNullException("items");
            }

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
