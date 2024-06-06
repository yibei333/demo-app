using SharpDevLib.Standard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ChatClient.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        static readonly string LoginUrl = App.ApiUrl.CombinePath("Login?Name=foo&Password=foo_password");

        public LoginPage()
        {
            InitializeComponent();
            MainWindow = MainWindow.Instance;
            browser.Navigated += Browser_Navigated;
            browser.Source = new Uri(LoginUrl);
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                if (e.Uri.AbsolutePath.EndsWith("Callback", StringComparison.OrdinalIgnoreCase) && e.Uri.Query.NotNullOrWhiteSpace())
                {
                    var query = e.Uri.Query.TrimStart('?');
                    var array = query.SplitToList('&');
                    array.ForEach(x =>
                    {
                        var nameValue = x.SplitToList('=');
                        if (nameValue.Count != 2) throw new Exception($"error data:{x}");
                        if (nameValue[0].Equals("Name", StringComparison.OrdinalIgnoreCase))
                        {
                            App.UserName = nameValue[1];
                        }
                        else if (nameValue[0].Equals("Token", StringComparison.OrdinalIgnoreCase))
                        {
                            App.UserId = nameValue[1].Base64Decode().ToUtf8String().ToGuid();
                        }
                    });
                    MainWindow.Navigate(nameof(HomePage));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                browser.Source = new Uri(LoginUrl);
            }
        }

        MainWindow MainWindow { get; }
    }
}
