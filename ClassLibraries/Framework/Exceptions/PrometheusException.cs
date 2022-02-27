using System;
using System.Net;

namespace Framework.Exceptions
{
    public class PrometheusException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public PrometheusException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }
    }
}