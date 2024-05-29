using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharpDevLib.Standard;

namespace ChatClient.Pages;

/// <summary>
/// Interaction logic for HomePage.xaml
/// </summary>
public partial class HomePage : Page
{
    internal Socket Socket { get; }

    public HomePage()
    {
        InitializeComponent();
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        MainWindow = MainWindow.Instance;
        ViewModel = new HomeViewModel(this) { Name = App.UserName, Id = App.UserId, DeviceId = App.DeviceId };
        DataContext = ViewModel;
    }

    MainWindow MainWindow { get; }
    HomeViewModel ViewModel { get; }
}


public partial class HomeViewModel : ObservableObject
{
    public HomeViewModel(HomePage page)
    {
        Page = page;
        ConnectCommand = new AsyncRelayCommand(Connect);
    }

    [ObservableProperty]
    string? name;

    [ObservableProperty]
    Guid id;

    [ObservableProperty]
    Guid deviceId;

    [ObservableProperty]
    string serverHost = "127.0.0.1";

    [ObservableProperty]
    int serverPort = 7654;

    [ObservableProperty]
    ObservableCollection<IdNameDto> users = [];

    [ObservableProperty]
    Guid selectedUserId;

    [ObservableProperty]
    string message;

    public HomePage Page { get; }

    public IAsyncRelayCommand ConnectCommand { get; }

    async Task Connect()
    {
        await Task.Yield();
        try
        {
            Page.Socket.BeginConnect(ServerHost, ServerPort, (result) =>
            {
                Page.Socket.EndConnect(result);

                try
                {
                    Receive();

                    var method = BitConverter.GetBytes(1);
                    var content = $"{Id},{DeviceId}".ToUtf8Bytes();
                    var data = method.Concat(content).ToArray();
                    Page.Socket.Send(data);
                }
                catch (Exception ex)
                {
                    LogText(ex.Message);
                }
            }, null);
        }
        catch (Exception ex)
        {
            LogText(ex.Message);
        }
    }

    void Receive()
    {
        var buffer = new byte[2048];
        Page.Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, async (res) =>
        {
            try
            {
                var length = Page.Socket.EndReceive(res);
                if (length > 0)
                {
                    await HandleMessage(buffer[..length]);
                    Receive();
                }
                else
                {
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                LogText(ex.Message);
            }
        }, null);
    }

    async Task HandleMessage(byte[] bytes)
    {
        try
        {
            if (bytes.Length < 36) return;
            var userIdSegmentLength = 36;
            var userIdSegmentData = bytes[..userIdSegmentLength];
            var userId = userIdSegmentData.ToUtf8String().ToGuid();
            var messageId = bytes[userIdSegmentLength..].ToUtf8String().ToGuid();
            if (userId == Guid.Empty || messageId == Guid.Empty) return;

            var reply = await App.Instance.ServiceProvider.GetRequiredService<IHttpService>().GetAsync<Reply<MessageModel>>(new HttpKeyValueRequest(App.ApiUrl.CombinePath("Message/get"), new Dictionary<string, string>() { { "UserId", userId.ToString() }, { "MessageId", messageId.ToString() } }));
            if (!reply.IsSuccess) LogText(reply.Message);
            else
            {
                if (!reply.Data!.Success) LogText(reply.Data.Description);
                else LogText($"{reply.Data.Data!.UserName}->{reply.Data.Data.Message}");
            }
        }
        catch (Exception ex)
        {
            LogText(ex.Message);
        }
    }

    [RelayCommand]
    async Task Disconnect()
    {
        await Page.Socket.DisconnectAsync(true);
        LogText("已断开连接");
    }

    void LogText(string? text)
    {
        Page.Dispatcher.Invoke(() => Page.Log.Text += $"{text}\r\n");
    }

    [RelayCommand]
    async Task GetUsersAsync()
    {
        var reply = await App.Instance.ServiceProvider.GetRequiredService<IHttpService>().GetAsync<Reply<List<IdNameDto>>>(new HttpKeyValueRequest(App.ApiUrl.CombinePath("User/get")));
        if (!reply.IsSuccess)
        {
            LogText(reply.Message);
            return;
        }
        if (!reply.Data!.Success)
        {
            LogText(reply.Data.Description);
            return;
        }
        Users.Clear();
        reply.Data.Data!.ForEach(x => Users.Add(x));
    }

    [RelayCommand]
    async Task SendMessageAsync()
    {
        if (SelectedUserId == Guid.Empty)
        {
            LogText("请选择用户");
            return;
        }

        if (Message.IsNullOrWhiteSpace())
        {
            LogText("消息不能为空");
            return;
        }

        if (!Page.Socket.Connected)
        {
            LogText("请先连接服务器");
            return;
        }

        var json = new { Message = message }.Serialize();
        var reply = await App.Instance.ServiceProvider.GetRequiredService<IHttpService>().PostAsync<Reply<Guid>>(new HttpJsonRequest(App.ApiUrl.CombinePath("Message/CreateTextMessage"), json));
        if (!reply.IsSuccess)
        {
            LogText(reply.Message.IsNullOrWhiteSpace() ? reply.Code.ToString() : reply.Message);
            return;
        }
        if (!reply.Data!.Success)
        {
            LogText(reply.Data.Description);
            return;
        }

        SendMessage(SelectedUserId, reply.Data.Data);
    }

    void SendMessage(Guid toUserId, Guid messageId)
    {
        var toUserData = toUserId.ToString().ToUtf8Bytes();
        var contentBytes = messageId.ToString().ToUtf8Bytes();
        var data = BitConverter.GetBytes(2).Concat(toUserData).Concat(contentBytes).ToArray();
        Page.Socket.Send(data);
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