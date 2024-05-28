using Microsoft.AspNetCore.Mvc;
using SharpDevLib.Standard;

namespace ChatServer.Controllers;

public class MessageController : BaseController
{
    public MessageController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public Reply<MessageModel> Get([FromQuery] MessageModel model)
    {
        var user = UserService.Get(x => x.Id == model.UserId);
        if (user is null) return Reply.Failed<MessageModel>("user not found");

        var message = MessageService.Get(x => x.Id == model.MessageId);
        if (message is null) return Reply.Failed<MessageModel>("message not found");

        model.UserName = user.Name;
        model.MessageType = message.Type;
        if (model.MessageType == 1)
        {
            model.Message = message.Message;
        }
        else if (model.MessageType == 2)
        {
            model.Bytes = model.Message!.Base64Decode();
        }
        return Reply.Succeed(model);
    }
}

public class MessageModel
{
    public Guid UserId { get; set; }
    public Guid MessageId { get; set; }
    public string? UserName { get; set; }
    public int MessageType { get; set; }
    public string? Message { get; set; }
    public byte[]? Bytes { get; set; }
}