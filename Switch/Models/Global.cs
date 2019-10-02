using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using UsefulThings;
using System.Xml.Linq;
using System.IO;

namespace Switch
{
    public class Global : INotifyPropertyChanged
    {
        public delegate void MessageHandler(string message);
        public event MessageHandler SendMessage;

        #region MVVM


        #region Fields

        Lang lang;
        ObservableCollection<Persona> persons = new ObservableCollection<Persona>();
        ObservableCollection<Proxy> proxies = new ObservableCollection<Proxy>();

        #endregion


        #region Properties

        public Lang Lang
        {
            get { return lang; }
            set
            {
                lang = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Persona> Persons
        {
            get { return persons; }
            set
            {
                persons = value;
                OnPropertyChanged();
            }
        }

        internal void OnSendOpenProfileCommandHandler(string path)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<Proxy> Proxies
        {
            get { return proxies; }
            set
            {
                proxies = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region fields

        public ViewModels.GlobalViewModel globalViewModel;
        public Views.MainWindow view;
        public ProxyMod proxyMod;

        #endregion

        public void Run()
        {
            lang = new Lang();
            proxyMod = new ProxyMod();
            globalViewModel = new ViewModels.GlobalViewModel(this);
            view = new Views.MainWindow(globalViewModel);
            SendMessage += view.onSendMessage;
            Proxies = proxyMod.ProxiesColl;
            InitPersons();
        }

        private void InitPersons()
        {
            XDocument doc = new XDocument();
            string profilesFile = @"profiles.xml";
            if (!File.Exists(profilesFile)) return;

            try
            {
                doc = XDocument.Load(profilesFile);

                foreach (XElement el in doc.Root.Elements())
                {
                    string title = el.Element("title").Value;
                    string description = el.Element("description").Value;
                    string profilePath = el.Element("profile_path").Value;
                    string userAgent = el.Element("user_agent").Value;

                    Persona pers = new Persona() { Title=title, Description = description, ProfilePath = profilePath, UserAgent = userAgent };

                    if(el.Element("proxy")!=null)
                    {
                        XElement proxyEl = el.Element("proxy");
                        string ip = proxyEl.Element("ip").Value;
                        string port = proxyEl.Element("port").Value;
                        string login = proxyEl.Element("login").Value;
                        string pwd = proxyEl.Element("pwd").Value;
                        string protocol_ = proxyEl.Element("protocol_type").Value;
                        ProxyProtocol protocol = (protocol_ == "HTTP") ? ProxyProtocol.HTTP : ProxyProtocol.SOCKS5;
                        Proxy pr = new Proxy(ip, port, login, pwd, protocol);
                        pers.Proxy = pr;
                    }
                    Persons.Add(pers);
                }
            }
            catch (Exception ex)
            {
                if (SendMessage != null)
                    SendMessage("Не удалось загрузить профили, возможно файл profiles.xml повреждён");
                L.LW(ex);
            }
        }

        #region Commands

        internal void OnSendCommandHandler(string commandName)
        {
            switch (commandName)
            {
                case "Del":
                    Del();
                    return;
                case "AddProfile":
                    AddProfile();
                    return;
                case "ProxySettings":
                    ProxySettings();
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ProxySettings()
        {
            proxyMod.Run(this);
        }

        private void AddProfile()
        {
            AddProfiler ap = new AddProfiler(this);
        }

        internal void OnSendCommandWithObjectCommandHandler(object objectValue)
        {
            Console.WriteLine(objectValue);
        }

        private void Del()
        {
            //ProfileLang = ProfileLang.Substring(0, ProfileLang.Length - 1);
        }

        #endregion

        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            globalViewModel.OnPropertyChanged(prop);
        }

        #endregion
    }
}
