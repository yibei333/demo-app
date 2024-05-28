
namespace ChatServer.Services;

public class MessageService : BaseService<MessageEntity>
{
    public static Guid LoginSuccessMessageId = Guid.NewGuid();

    public override List<MessageEntity> SeedData()
    {
        return
        [
            new MessageEntity{ Id=LoginSuccessMessageId,Type=1,Message="login success" },
        ];
    }
}

public class MessageEntity
{
    public Guid Id { get; set; }
    /// <summary>
    /// 1-txt,2-file
    /// </summary>
    public int Type { get; set; }
    public string? Message { get; set; }
}