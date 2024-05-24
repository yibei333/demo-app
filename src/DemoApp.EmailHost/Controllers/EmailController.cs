using DemoApp.EmailHost.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SharpDevLib.Standard;
using System.Text;

namespace DemoApp.EmailHost.Controllers;

public class EmailController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    public IActionResult Index()
    {
        var model = _emailRepository.GetMany(x => !x.IsDeleted).Select(x => new EmailViewModel { Id = x.Id, Subject = x.Subject, Data = x.Data, CreateTime = x.CreateTime }).ToList();
        return View(model);
    }

    public IActionResult Detail(Guid id)
    {
        var model = _emailRepository.GetMany(x => !x.IsDeleted && x.Id == id).Select(x => new EmailViewModel { Subject = x.Subject, Data = x.Data, CreateTime = x.CreateTime }).FirstOrDefault();
        if (model is null || model.Data is null) return View(model);

        using var memoryStream = new MemoryStream(model.Data.ToUtf8Bytes());
        var message = MimeMessage.Load(new ParserOptions { CharsetEncoding = Encoding.UTF8 }, memoryStream);

        model.From = string.Join(",", message.From.Select(x => x.Name));
        model.To = string.Join(",", message.To.Select(x => x.Name));
        model.CC = string.Join(",", message.Cc.Select(x => x.Name));
        model.Body = message.HtmlBody;

        message.Attachments.ToList().ForEach(x =>
        {
            if (x is MimePart part)
            {
                using var partStream = new MemoryStream();
                part.Content.Stream.CopyTo(partStream);
                model.Attachments.Add(new NameData(part.FileName, partStream.ToArray().ToUtf8String(), part.ContentType.MimeType));
            }
            else
            {
                _logger.LogWarning("unkonw part:{type}", x.GetType().FullName);
            }
        });

        return View(model);
    }
}
