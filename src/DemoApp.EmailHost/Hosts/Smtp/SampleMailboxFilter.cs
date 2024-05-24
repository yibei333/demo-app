using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Storage;

namespace DemoApp.EmailHost.Hosts.Smtp;

public class SampleMailboxFilter(IServiceProvider serviceProvider) : SmtpBase(serviceProvider), IMailboxFilter
{
    Task<bool> IMailboxFilter.CanAcceptFromAsync(ISessionContext context, IMailbox from, int size, CancellationToken cancellationToken) => Task.FromResult(true);

    Task<bool> IMailboxFilter.CanDeliverToAsync(ISessionContext context, IMailbox to, IMailbox from, CancellationToken cancellationToken) => Task.FromResult(true);
}

