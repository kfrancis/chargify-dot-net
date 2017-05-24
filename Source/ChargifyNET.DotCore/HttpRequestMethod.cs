
namespace ChargifyNET
{
    /// <summary>
    /// The type of REST request
    /// </summary>
    public enum HttpRequestMethod
    {
        /// <summary>
        /// Requests a representation of the specified resource
        /// </summary>
        Get,
        /// <summary>
        /// Requests that the server accept the entity enclosed in the request as a new subordinate of the web resource identified by the URI
        /// </summary>
        Post,
        /// <summary>
        /// Requests that the enclosed entity be stored under the supplied URI
        /// </summary>
        Put,
        /// <summary>
        /// Deletes the specified resource
        /// </summary>
        Delete
    }
}