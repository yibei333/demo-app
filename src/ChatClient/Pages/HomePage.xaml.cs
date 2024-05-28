using System.Net.Sockets;
using System.Windows;
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

    public HomePage Page { get; }

    public IAsyncRelayCommand ConnectCommand { get; }

    async Task Connect()
    {
        await Task.Yield();
        try
        {
            Page.Socket.Connect(ServerHost, ServerPort);
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
    }


    void Receive()
    {
        var buffer = new byte[2048];
        Page.Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, async (res) =>
        {
            try
            {
                var length = Page.Socket.EndReceive(res);
                await HandleMessage(buffer[..length]);
                Receive();
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

    void LogText(string? text)
    {
        Page.Dispatcher.Invoke(() => Page.Log.Text += $"{text}\r\n");
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