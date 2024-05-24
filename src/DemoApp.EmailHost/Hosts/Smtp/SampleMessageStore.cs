using DemoApp.EmailHost.Common;
using DemoApp.EmailHost.Data.Entity;
using MimeKit;
using SharpDevLib.Standard;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System.Buffers;
using System.Text;

namespace DemoApp.EmailHost.Hosts.Smtp;

public class SampleMessageStore(IServiceProvider serviceProvider) : SmtpBase(serviceProvider), IMessageStore
{
    public async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
    {
        var fromUser = UserRepository.Get(x => x.Name == transaction.From.User) ?? throw new NullReferenceException();
        var toUsers = UserRepository.GetMany(x => transaction.To.Select(x => x.User).Contains(x.Name)).ToList();
        var notExistToUsers = transaction.To.Select(x => x.User).Except(toUsers.Select(x => x.Name)).ToList();
        if (notExistToUsers.Count != 0) Logger.LogInformation("external user:{users}", string.Join(",", notExistToUsers));
        toUsers = toUsers.Where(x => !notExistToUsers.Contains(x.Name)).ToList();
        var array = buffer.ToArray();

        //save file
        var id = Guid.NewGuid();
        var path = Statics.EmailDirectory.CombinePath($"{id}.txt");
        await array.SaveToFileAsync(path, cancellationToken);

        //save data
        using var memoryStream = new MemoryStream(array);
        var message = MimeMessage.Load(new ParserOptions { CharsetEncoding = Encoding.UTF8 }, memoryStream, cancellationToken);
        var email = new EmailEntity(message.Subject, message.MessageId, path, array.ToUtf8String()) { Id = id, ReferenceMessageIds = message.MessageId };
        DataContext.Emails.Add(email);

        var fromDetail = new EmailListEntity(email.Id, fromUser.Id, 1);
        DataContext.EmailLists.Add(fromDetail);
        toUsers.ForEach(x =>
        {
            var toDetail = new EmailListEntity(email.Id, x.Id, 2);
            DataContext.EmailLists.Add(toDetail);
        });
        await DataContext.SaveChangesAsync(cancellationToken);

        return SmtpResponse.Ok;
    }
}

