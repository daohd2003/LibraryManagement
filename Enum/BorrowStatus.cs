using System.Text.Json.Serialization;

namespace LibraryManagement.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BorrowStatus
    {
        Borrowed,
        Returned,
        Overdue
    }
}
