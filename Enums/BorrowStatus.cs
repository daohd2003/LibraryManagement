using System.Text.Json.Serialization;

namespace LibraryManagement.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BorrowStatus
    {
        Borrowed,
        Returned,
        Overdue
    }
}
