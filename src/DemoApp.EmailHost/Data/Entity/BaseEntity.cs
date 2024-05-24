using SharpDevLib.Standard;

namespace DemoApp.EmailHost.Data.Entity;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public long CreateTime { get; set; } = DateTime.Now.ToUtcTimeStamp();
    public bool IsDeleted { get; set; }
}
