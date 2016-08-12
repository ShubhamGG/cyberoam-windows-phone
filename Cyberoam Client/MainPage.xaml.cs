using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Filters;
using Windows.Security.Cryptography.Certificates;

namespace Cyberoam_Client
{
    public sealed partial class MainPage : Page
    {

        private CyberoamMethods mSettings;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mSettings = new CyberoamMethods(ApplicationData.Current.LocalSettings);
                
            // navigate to settings page if there is no server IP stored (first run)
            if (mSettings.SERVERIP == "")
            {
                Frame.Navigate(typeof(SettingsPage));
            }

            if (mSettings.LOGINSTATUS)
            { // status is logged in
                setStatusLoggedIn();
            }
            else
            { //status is logged out
                setStatusLoggedOut();
            }

            txtUsername.Text = mSettings.USERNAME;
            if (mSettings.IFSAVEPASS)
            {
                chkSavePassword.IsChecked = true;
                txtPassword.Password = mSettings.PASSWORD;
                if (mSettings.IFAUTOLOGIN)
                    if (!mSettings.LOGINSTATUS)
                        buttLoginLogout_Click(null, null);
            }

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        #region BottomBar Implementation
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
        
        #endregion

        private void buttLoginLogout_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string ip = mSettings.SERVERIP;
            string port = mSettings.PORT;
            
            txtUsername.IsEnabled = false;
            txtPassword.IsEnabled = false;
            setIfBusyStatus(true);
            if (!mSettings.LOGINSTATUS) //Action to perform is Login
            {
                cyberoamLogin(username, password, ip, port);
            }
            else //Action to perform in Logout
            {
                cyberoamLogout(username, ip, port);
            }
            mSettings.USERNAME = username;
            if ((bool)chkSavePassword.IsChecked)
                mSettings.PASSWORD = password;
        }
        
        private async void cyberoamLogin(string usrnm, string passwrd, string ip, string port)
        {
            var filter = new HttpBaseProtocolFilter();
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.IncompleteChain);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
            var httpClient = new HttpClient(filter);
            string data = "mode=191&username=" + usrnm + "&password=" + passwrd;
            Uri url = new Uri("https://" + ip + ":"+port+"/login.xml");
            try
            {
                var response = await httpClient.PostAsync(url, new HttpStringContent(data));

                if (!response.IsSuccessStatusCode)
                {//request failed
                    mSettings.notifyUser("Login failed", "Network Error");
                    setIfBusyStatus(false);
                    return;
                }

                string content = await response.Content.ReadAsStringAsync();

                if (content.IndexOf("successfully logged in") != -1)
                { //login successful
                    setStatusLoggedIn();
                    mSettings.notifyUser("Login Successful", null);
                }
                else if (content.IndexOf("Maximum Login Limit") != -1)
                { //maximum login limit
                    mSettings.notifyUser("Login failed", "Max. login limit reached ");
                }
                else
                { //probably wrong password
                   mSettings.notifyUser("Login failed", "Make sure credentials are correct");
                }
            } catch (Exception e)
            {
                if (e.Message.IndexOf("0x80072F30")!=-1)
                    mSettings.notifyUser("Error", "Please check your Wi-fi connection");
                else
                    mSettings.notifyUser("Error",e.Message);
            }
            setIfBusyStatus(false);
            txtUsername.IsEnabled = !mSettings.LOGINSTATUS;
            txtPassword.IsEnabled = !mSettings.LOGINSTATUS;
        }

        private async void cyberoamLogout(string usrnm, string ip, string port)
        {
            var filter = new HttpBaseProtocolFilter();
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.IncompleteChain);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
            var httpClient = new HttpClient(filter);
            string data = "mode=193&username=" + usrnm;
            Uri url = new Uri("https://" + ip + ":" + port + "/logout.xml");
            
            var response = await httpClient.PostAsync(url,(IHttpContent) new HttpStringContent(data));
            if (!response.IsSuccessStatusCode)
            {//request failed
                mSettings.notifyUser("Logout failed", "Network Error");
                setStatusLoggedOut();
                setIfBusyStatus(false);
                return;
            }

            string content = await response.Content.ReadAsStringAsync();
            // process response.

            if(content.IndexOf("successfully logged off")!=-1)
            {
                mSettings.notifyUser("Logged out successfully", null);
            }
            else
            {
                mSettings.notifyUser("Logout failed",null);
            }
            setStatusLoggedOut();
            setIfBusyStatus(false);
        }

        private async Task<int> cyberoamKeepAlive(string usrnm, string ip, string port)
        {
            var httpClient = new HttpClient();
            string data = "mode=192&username=" + usrnm;
            Uri url = new Uri("https://" + ip + ":" + port + "/live?" + data);

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {//request failed
                return 1;
            }

            string content = await response.Content.ReadAsStringAsync();
            if (content.IndexOf("<ack><![CDATA[ack]]></ack>")==-1)
            {
                return 2;
            }

            return 0;
        }

        private void setIfBusyStatus(bool status)
        {
            prgrssrng.Visibility = (status)?Visibility.Visible:Visibility.Collapsed;
            prgrssrng.IsActive = status;
            buttLoginLogout.IsEnabled = !status;
            chkSavePassword.IsEnabled = !status;
        }

        private void setStatusLoggedIn()
        {
            // set status to logged in
            buttLoginLogout.Content = "Logout";
            mSettings.LOGINSTATUS = true;
            SettingsButton.IsEnabled = false;
            chkSavePassword.IsEnabled = false;
            txtPassword.IsEnabled = false;
            txtUsername.IsEnabled = false;
        }

        private void setStatusLoggedOut()
        {
            // set status to logged out
            buttLoginLogout.Content = "Login";
            txtPassword.IsEnabled = true;
            txtUsername.IsEnabled = true;
            mSettings.LOGINSTATUS = false;
            SettingsButton.IsEnabled = true;
            chkSavePassword.IsEnabled = true;
        }

        private void chkSavePassword_Checked(object sender, RoutedEventArgs e)
        {
            bool ifSavePass = (bool) chkSavePassword.IsChecked;
            if (ifSavePass)
            {//save password
                mSettings.PASSWORD = txtPassword.Password.ToString();
            }
            else
            {//unsave password
                mSettings.PASSWORD = null;
            }
        }

    }
}
