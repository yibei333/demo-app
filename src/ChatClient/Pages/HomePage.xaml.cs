using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SharpDevLib.Standard;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ChatClient.Pages;

/// <summary>
/// Interaction logic for HomePage.xaml
/// </summary>
public partial class HomePage : Page
{
    internal TcpClientFactory Factory { get; }

    public HomePage()
    {
        InitializeComponent();

        ViewModel = new HomeViewModel(this) { Name = App.UserName, Id = App.UserId, DeviceId = App.DeviceId };
        DataContext = ViewModel;
        Factory = new TcpClientFactory();
    }

    HomeViewModel ViewModel { get; }
}


public partial class HomeViewModel : ObservableObject
{
    TcpClient? _client;

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
    string? message;

    public HomePage Page { get; }

    public IAsyncRelayCommand ConnectCommand { get; }

    async Task Connect()
    {
        await Task.Yield();
        _client = Page.Factory.Create(System.Net.IPAddress.Parse(ServerHost), ServerPort);
        _client.Received += Receive;
        _client.Error += (s, e) => LogText(e.Exception.Message);
        _client.StateChanged += (s, e) =>
        {
            if (e.Current == TcpClientStates.Connected)
            {
                var method = BitConverter.GetBytes(1);
                var content = $"{Id},{DeviceId}".ToUtf8Bytes();
                var data = method.Concat(content).ToArray();
                _client.Send(data);
            }
        };
        ConnectAsync();
    }

    async void ConnectAsync()
    {
        if (_client is null) return;
        await _client.ConnectAndReceiveAsync();
    }

    private void Receive(object? sender, TcpClientDataEventArgs e)
    {
        HandleMessage(e.Bytes);
    }

    async void HandleMessage(byte[] bytes)
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
    void Disconnect()
    {
        _client?.Dispose();
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

        if (_client?.State != TcpClientStates.Connected)
        {
            LogText("请先连接服务器");
            return;
        }

        var json = new { Message }.Serialize();
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
        _client?.Send(data);
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