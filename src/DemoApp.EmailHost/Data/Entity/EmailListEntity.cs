namespace DemoApp.EmailHost.Data.Entity;

public class EmailListEntity(Guid emailId, Guid userId, int type) : BaseEntity
{
    public Guid EmailId { get; set; } = emailId;
    public Guid UserId { get; set; } = userId;
    /// <summary>
    /// 1-from,2-to
    /// </summary>
    public int Type { get; set; } = type;
}
