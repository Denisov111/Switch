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
using MasterDevs.ChromeDevTools.Protocol.Chrome.Page;
using MasterDevs.ChromeDevTools.Protocol.Chrome.DOM;
using MasterDevs.ChromeDevTools;
using Chrome = MasterDevs.ChromeDevTools.Protocol.Chrome;
using System.Diagnostics;
using System.Collections.Specialized;
using ChromeModForNet;

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

        internal void OnSendEditProfileCommandHandler(string path)
        {
            Persona persona = Persons.Where(pers => pers.ProfilePath == path).FirstOrDefault();
            AddProfiler ap = new AddProfiler(this, persona);
            
        }

        async internal void OnSendCheckProxyCommandHandler(string path)
        {
            Persona persona = Persons.Where(pers => pers.ProfilePath == path).FirstOrDefault();
            if (persona.Proxy == null) return;
            await proxyMod.CheckProxy(persona.Proxy);
            SendMessage("Анонимность: "+ persona.Proxy.Anonimity.ToString() + " доступность: "+ persona.Proxy.Status.ToString());
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

        async internal void OnSendOpenProfileCommandHandler(string path)
        {
            Persona persona = Persons.Where(pers=>pers.ProfilePath==path).FirstOrDefault();
            if(persona==null)
            {
                SendMessage("Не удаётся найти профиль");
                return;
            }

            int port = GetFreeLocalPort();

            string proxyString = null;
            string proxyProtocol = null;
            if (persona.Proxy!=null)
            {
                if(persona.Proxy.Login!=null)
                {
                    proxyString = persona.Proxy.Ip + ":" + persona.Proxy.Port + "@" + persona.Proxy.Login + ":" + persona.Proxy.Pwd;
                }
                else
                {
                    proxyString = persona.Proxy.Ip + ":" + persona.Proxy.Port;
                }
                proxyProtocol = persona.Proxy.ProxyProtocol.ToString().ToLower();
            }

            ChromeMod chromeInstance = new ChromeMod();
            CallResult<IChromeSession> result = await chromeInstance.GetChromeSession(proxyString, persona.UserAgent, proxyProtocol, false, false, persona.ProfilePath);
            persona.ChromeInstance = chromeInstance;

            /*
            var chromeProcessFactory = new ChromeProcessFactory(new StubbornDirectoryCleaner());
            var chromeProcess = chromeProcessFactory.Create(port, false, null, path);
            Process pr = ((RemoteChromeProcess)chromeProcess).Process;
            var sessionInfo = (await chromeProcess.GetSessionInfo()).LastOrDefault();
            var chromeSessionFactory = new ChromeSessionFactory();
            var chromeSession = chromeSessionFactory.Create(sessionInfo.WebSocketDebuggerUrl);*/
        }

        public static int GetFreeLocalPort()
        {
            ProcessStartInfo psiOpt = new ProcessStartInfo("cmd.exe", "/C netstat -a -n -o");
            psiOpt.WindowStyle = ProcessWindowStyle.Hidden;
            psiOpt.RedirectStandardOutput = true;
            psiOpt.UseShellExecute = false;
            psiOpt.CreateNoWindow = true;
            // запускаем процесс
            Process procCommand = Process.Start(psiOpt);
            // получаем ответ запущенного процесса
            StreamReader srIncoming = procCommand.StandardOutput;
            // выводим результат
            string[] ss = srIncoming.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            StringCollection procStr = new StringCollection();
            procCommand.WaitForExit();

            for (int i = GlobalVars.MinDebugPort; i < GlobalVars.MaxDebugPort; i++)
            {
                bool portIsBusy = false;
                string portSignature = ":" + i.ToString();
                foreach (string s in ss)
                {
                    if (s.Contains(portSignature))
                    {
                        portIsBusy = true;
                        break;
                    }
                }
                if (portIsBusy)
                {
                    continue;
                }
                else
                {
                    return i;
                }
            }
            return 0;
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
                case "CheckAllProxy":
                    CheckAllProxy();
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        async private void CheckAllProxy()
        {
            string mess = null;
            for(int i=0;i< Persons.Count;i++)
            {
                if (Persons[i].Proxy == null) continue;
                await proxyMod.CheckProxy(Persons[i].Proxy);
                mess += Persons[i].Proxy.Ip + ":"+Persons[i].Proxy.Port+ " : " + "анонимность: " + Persons[i].Proxy.Anonimity.ToString() + " доступность: " + Persons[i].Proxy.Status.ToString()+"\n";
            }
            if(mess!=null)
                SendMessage(mess);
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
