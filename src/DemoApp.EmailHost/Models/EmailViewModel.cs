using SharpDevLib.Standard;

namespace DemoApp.EmailHost.Models;

public class EmailViewModel
{
    public Guid Id { get; set; }
    public string? Subject { get; set; }
    public string? Data { get; set; }
    public long CreateTime { get; set; }
    public string CreateTimeValue => CreateTime.ToUtcTime().ToTimeString();
    public string? Body { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public string? CC { get; set; }
    public List<AttachmentModel> Attachments { get; set; } = [];
}

public class AttachmentModel(string name, string data, string mimeType)
{
    public string Name { get; set; } = name;
    public string Data { get; set; } = data;
    public string MimeType { get; set; } = mimeType;
}