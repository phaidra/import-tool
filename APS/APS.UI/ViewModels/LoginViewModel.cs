using APS.Lib.Import;
using System;
using System.Collections.Generic;
using System.Text;

namespace APS.UI
{
    public class LoginViewModel : VMBase
    {

        private string _username;
        private string _password;
        private string _loginStatus;

        public Command LoginCommand { get; }

        private string _url;

        public string Url
        {
            get { return _url; }
            set
            {
                if (_url != value)
                {
                    _url = value;
                    RaisePropertyChanged(nameof(Url));
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public void ExecuteAutoLogon()
        {
            if (AutoLogon && LoginCanExecute(null))
            {
                LoginExecute(null);
            }
        }

        private string _searchEngineUrl;
        private bool _isBusy;

        public string SearchEngineUrl
        {
            get { return _searchEngineUrl; }
            set
            {
                if (_searchEngineUrl != value)
                {
                    _searchEngineUrl = value;
                    RaisePropertyChanged(nameof(SearchEngineUrl));
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged(nameof(Username));
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged(nameof(Password));
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool AutoLogon { get; set; }

        public string LoginStatus
        {
            get { return _loginStatus; }
            set
            {
                if (_loginStatus != value)
                {
                    _loginStatus = value;
                    RaisePropertyChanged(nameof(LoginStatus));
                }
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged(nameof(IsBusy));
                    RaisePropertyChanged(nameof(IsEnabled));
                }
            }
        }

        public bool IsEnabled => !IsBusy;
        public bool DialogResult { get; set; }
        public Action CloseAction { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(LoginExecute, LoginCanExecute);
            Url = PhaidraConfig.Instance.Url;
            SearchEngineUrl = PhaidraConfig.Instance.SearchEngineUrl;
            Username = PhaidraConfig.Instance.Username;
            Password = PhaidraConfig.Instance.Password;
            AutoLogon = PhaidraConfig.Instance.AutoLogon;
            LoginStatus = "-";
        }

        private async void LoginExecute(object obj)
        {
            try
            {
                IsBusy = true;
                LoginStatus = "Login...";
                PhaidraClient client = new PhaidraClient(Url, SearchEngineUrl, Username, Password);
                var searchEngineTestResult = await client.FindCollectionBySystemTag("/");
                if (!searchEngineTestResult.Success)
                {
                    LoginStatus = "Search engine URL invalid";
                    return;
                }
                bool loginSuccessful = await client.SigninTest();
                if (!loginSuccessful)
                {
                    LoginStatus = "Login failed";
                    return;
                }

                LoginStatus = "Login successful";
                PhaidraConfig.Instance.Url = Url;
                PhaidraConfig.Instance.Username = Username;
                PhaidraConfig.Instance.SearchEngineUrl = SearchEngineUrl;
                if (Password != PhaidraConfig.Instance.Password)
                {
                    // if stored password is different than login password -> delete stored password
                    PhaidraConfig.Instance.Password = null;
                }
                PhaidraConfig.Save();
                DialogResult = true;
                CloseAction?.Invoke();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool LoginCanExecute(object arg)
        {
            return !string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(SearchEngineUrl) && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }
    }
}
