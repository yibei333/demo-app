namespace DemoApp.EmailHost.Data.Entity;

public class EmailEntity(string subject, string messageId, string filePath, string data) : BaseEntity
{
    public string FilePath { get; set; } = filePath;
    public string Subject { get; set; } = subject;
    public string MessageId { get; set; } = messageId;
    public string? ReferenceMessageIds { get; set; }
    public string Data { get; set; } = data;
}
