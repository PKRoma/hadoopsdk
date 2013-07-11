namespace Microsoft.WindowsAzure.Management.Framework.WebRequest
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a set of Http response headers returned from an http request.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "This name is correct for the context of abstracting a previously named class. [tgs]")]
    public interface IHttpResponseHeadersAbstraction : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
    {
        /// <summary>
        /// Adds a set of values for a header.
        /// </summary>
        /// <param name="name">
        /// The name of the header.
        /// </param>
        /// <param name="values">
        /// The values to associate with the header.
        /// </param>
        void Add(string name, IEnumerable<string> values);

        /// <summary>
        /// Adds a single value to a header.
        /// </summary>
        /// <param name="name">
        /// The name of the header.
        /// </param>
        /// <param name="value">
        /// The value to add as the only element in the headers enumeration of values.
        /// </param>
        void Add(string name, string value);

        /// <summary>
        /// Clears the list of headers.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines if a header exists in the collection.
        /// </summary>
        /// <param name="name">
        /// The name of the header.
        /// </param>
        /// <returns>
        /// True if the header exists otherwise false.
        /// </returns>
        bool Contains(string name);

        /// <summary>
        /// Removes a header from the collection.
        /// </summary>
        /// <param name="name">
        /// The name of the header to remove.
        /// </param>
        void Remove(string name);

        /// <summary>
        /// Tries to get the values associated with a header name.
        /// </summary>
        /// <param name="name">
        /// The name of the header.
        /// </param>
        /// <param name="values">
        /// An out parameter into which the header values will be placed.
        /// </param>
        /// <returns>
        /// True if the header exists in the collection otherwise false.
        /// </returns>
        bool TryGetValue(string name, out IEnumerable<string> values);

        /// <summary>
        /// Gets the values associated with a specific header.
        /// </summary>
        /// <param name="name">
        /// The name of the header.
        /// </param>
        /// <returns>
        /// The values associated with the header.
        /// </returns>
        IEnumerable<string> GetValues(string name);

        /// <summary>
        /// Gets the headers values associated with a header name.
        /// </summary>
        /// <param name="name">
        /// The name of the header.
        /// </param>
        /// <returns>
        /// The values associated with the header.
        /// </returns>
        IEnumerable<string> this[string name] { get; set; }
    }
}
