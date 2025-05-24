using LibraryManagement.Models;

namespace LibraryManagement.Services.AI
{
    public interface IAiSearchService
    {
        Task<string> AskAboutLibraryAsync(string question);
    }
}
