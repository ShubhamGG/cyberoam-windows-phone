using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace Cyberoam_Client
{
    class CyberoamMethods
    {
        private string KEY_USERNAME = "username";
        private string KEY_PASSWORD = "password";
        private string KEY_SAVEPASS = "ifSavePass";
        private string KEY_SERVERIP = "serverIP";
        private string KEY_AUTO_LOGIN = "ifAutoLogin";
        private string KEY_LOGIN_STATUS = "ifLoggedIn";
        private string KEY_PORT = "serverPort";

        public string USERNAME
        {
            get
            {
                return mSettingsStore.Values[KEY_USERNAME].ToString();
            }
            set
            {
                mSettingsStore.Values[KEY_USERNAME] = (value == null) ? "" : value;
            }
        }

        public string PASSWORD
        {
            get
            {
                return mSettingsStore.Values[KEY_PASSWORD].ToString();
            }
            set
            {
                mSettingsStore.Values[KEY_PASSWORD] = (value == null) ? "" : value;
            }
        }

        public bool IFSAVEPASS
        {
            get
            {
                return (bool) mSettingsStore.Values[KEY_SAVEPASS];
            }
            set
            {
                mSettingsStore.Values[KEY_SAVEPASS] = value;
            }
        }

        public string SERVERIP
        {
            get
            {
                return mSettingsStore.Values[KEY_SERVERIP].ToString();
            }
            set
            {
                mSettingsStore.Values[KEY_SERVERIP] = (value == null) ? "" : value;
            }
        }

        public bool IFAUTOLOGIN
        {
            get
            {
                return (bool) mSettingsStore.Values[KEY_AUTO_LOGIN];
            }
            set
            {
                mSettingsStore.Values[KEY_AUTO_LOGIN] = value;
            }
        }

        public bool LOGINSTATUS
        {
            get
            {
                return (bool) mSettingsStore.Values[KEY_LOGIN_STATUS];
            }
            set
            {
                mSettingsStore.Values[KEY_LOGIN_STATUS] = value;
            }
        }

        public string PORT
        {
            get
            {
                return mSettingsStore.Values[KEY_PORT].ToString();
            }
            set
            {
                mSettingsStore.Values[KEY_PORT] = value;
            }
        }

        private ApplicationDataContainer mSettingsStore;

        public CyberoamMethods(ApplicationDataContainer SettingsStore)
        {
            mSettingsStore = SettingsStore;
            checkSettingsOrMake();
        }
        
        public void checkSettingsOrMake()
        {
            //Check if settings are there. If not, make default settings.
            bool ifUsernameExist = mSettingsStore.Values.ContainsKey(KEY_USERNAME);
            if (!mSettingsStore.Values.ContainsKey(KEY_USERNAME))
                mSettingsStore.Values.Add(new KeyValuePair<string, object>(KEY_USERNAME, ""));
            if (!mSettingsStore.Values.ContainsKey(KEY_AUTO_LOGIN))
                mSettingsStore.Values.Add(new KeyValuePair<string, object>(KEY_AUTO_LOGIN, true));
            if (!mSettingsStore.Values.ContainsKey(KEY_LOGIN_STATUS))
                mSettingsStore.Values.Add(new KeyValuePair<string, object>(KEY_LOGIN_STATUS, false));
            if (!mSettingsStore.Values.ContainsKey(KEY_SAVEPASS))
                mSettingsStore.Values.Add(new KeyValuePair<string, object>(KEY_SAVEPASS, true));
            if (!mSettingsStore.Values.ContainsKey(KEY_SERVERIP))
                mSettingsStore.Values.Add(new KeyValuePair<string, object>(KEY_SERVERIP, ""));
            if (!mSettingsStore.Values.ContainsKey(KEY_PASSWORD))
                mSettingsStore.Values.Add(new KeyValuePair<string, object>(KEY_PASSWORD, ""));
            if (!mSettingsStore.Values.ContainsKey(KEY_PORT))
                mSettingsStore.Values.Add(new KeyValuePair<string, object>(KEY_PORT, "8090"));
        }
        
        public void notifyUser(string heading, string desc)
        {
            //TODO: notify user of msg
            desc = (desc == null) ? "" : desc;
            ToastTemplateType template = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(template);
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(heading));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(desc));
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

    }
}
