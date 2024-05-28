using ChatClient.Pages;
using System.Windows;

namespace ChatClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static MainWindow Instance = null!;

    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
        if (App.UserId == Guid.Empty)
        {
            Navigate(nameof(LoginPage));
        }
        else
        {
            Navigate(nameof(HomePage));
        }
    }

    public void Navigate(string name)
    {
        mainFrame.Source = new Uri($"Pages/{name}.xaml", UriKind.Relative);
    }
}