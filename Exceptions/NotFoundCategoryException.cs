using System.Net;

namespace LibraryManagement.Exceptions
{
    public class NotFoundCategoryException : DomainException
    {
        public NotFoundCategoryException()
        : base($"Category was not found.", HttpStatusCode.NotFound) { }
    }
}
