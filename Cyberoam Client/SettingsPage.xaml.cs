using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Cyberoam_Client
{

    public sealed partial class SettingsPage : Page
    {
        private CyberoamMethods mSettings;

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mSettings = new CyberoamMethods(ApplicationData.Current.LocalSettings);
            txtServerIp.Text = mSettings.SERVERIP;
            txtServerPort.Text = mSettings.PORT;
            chkAutoLogin.IsChecked = mSettings.IFAUTOLOGIN;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            mSettings.SERVERIP = txtServerIp.Text;
            mSettings.PORT = txtServerPort.Text;
            mSettings.IFAUTOLOGIN = (bool) chkAutoLogin.IsChecked;
            Frame.GoBack();
        }
        
    }
}
