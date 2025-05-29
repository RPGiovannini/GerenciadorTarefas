using System.Net;

namespace GerenciadorTarefas.Domain.Exceptions
{
    public class CustomException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string? Details { get; }

        public CustomException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }
    }
}
