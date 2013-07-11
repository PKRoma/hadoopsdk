namespace Microsoft.WindowsAzure.Management.Framework.WebRequest
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;

    /// <summary>
    /// Abstracts HttpResponseHeaders when using the Http abstraction for http comunication.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "This name is correct for the context of abstracting a previously named class. [tgs]")]
    public class HttpResponseHeadersAbstraction : IHttpResponseHeadersAbstraction
    {
        private Dictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();

        /// <summary>
        /// Initializes a new instance of the HttpResponseHeadersAbstraction class.
        /// </summary>
        public HttpResponseHeadersAbstraction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpResponseHeadersAbstraction class.
        /// </summary>
        /// <param name="headers">
        /// An actual HttpResponseHeaders from which this class should get its values.
        /// </param>
        public HttpResponseHeadersAbstraction(HttpResponseHeaders headers)
        {
            this.headers.AddRange(headers);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            return this.headers.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.headers.GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(string name, IEnumerable<string> values)
        {
            this.headers.Add(name, values);
        }

        /// <inheritdoc />
        public void Add(string name, string value)
        {
            this.headers.Add(name, value.MakeEnumeration());
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.headers.Clear();
        }

        /// <inheritdoc />
        public bool Contains(string name)
        {
            return this.headers.ContainsKey(name);
        }

        /// <inheritdoc />
        public void Remove(string name)
        {
            this.headers.Remove(name);
        }

        /// <inheritdoc />
        public bool TryGetValue(string name, out IEnumerable<string> values)
        {
            return this.headers.TryGetValue(name, out values);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetValues(string name)
        {
            return this.headers[name];
        }

        /// <inheritdoc />
        public IEnumerable<string> this[string name]
        {
            get { return this.headers[name]; }
            set { this.headers[name] = value; }
        }
    }
}
