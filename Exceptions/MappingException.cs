using System.Net;

namespace LibraryManagement.Exceptions
{
    public class MappingException : DomainException
    {
        public MappingException(string message)
        : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}