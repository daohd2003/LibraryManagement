using System.Net;

namespace LibraryManagement.Exceptions
{
    public class DuplicateCategoryException : DomainException
    {
        public DuplicateCategoryException(string name)
        : base($"Category '{name}' already exists", HttpStatusCode.Conflict)
        {
        }
    }
}
