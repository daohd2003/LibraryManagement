using System.Net;

namespace LibraryManagement.Exceptions
{
    public class MismatchedCategoryIdException : DomainException
    {
        public MismatchedCategoryIdException(int routeId, int bodyId)
            : base($"Mismatched category ID: Route ID = {routeId}, Body ID = {bodyId}.", HttpStatusCode.BadRequest) { }
    }
}
