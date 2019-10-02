using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UsefulThings;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.IO;

namespace Switch
{
    public class ProxyMod : INotifyPropertyChanged
    {
        public delegate void MessageHandler(string message);
        public event MessageHandler SendMessage;


        StringCollection proxiesStrings;
        ProxyProtocol protocol;

        #region MVVM


        #region Fields

        string textProxyList;
        int proxyFormatIndex;
        int proxyTypeIndex;
        ObservableCollection<Proxy> proxiesColl = new ObservableCollection<Proxy>();

        #endregion


        #region Properties

        public string TextProxyList
        {
            get { return textProxyList; }
            set
            {
                textProxyList = value;
                OnPropertyChanged();
            }
        }

        public int ProxyFormatIndex
        {
            get { return proxyFormatIndex; }
            set
            {
                proxyFormatIndex = value;
                OnPropertyChanged();
            }
        }

        public int ProxyTypeIndex
        {
            get { return proxyTypeIndex; }
            set
            {
                proxyTypeIndex = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Proxy> ProxiesColl
        {
            get { return proxiesColl; }
            set
            {
                proxiesColl = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion



        #region Main code

        public ProxyMod()
        {
            ProxyCollInit();
        }

        internal void Run(Global global)
        {
            
            ProxiesView f = new ProxiesView(global.Lang, this);
            SendMessage += f.onSendMessage;
            f.ShowDialog();
        }

        public void ProxyCollInit()
        {
            XDocument doc = new XDocument();
            string proxiesFile = @"proxies.xml";

            if (!File.Exists(proxiesFile))
            {
                return;
            }

            try
            {
                doc = XDocument.Load(proxiesFile);

                foreach (XElement el in doc.Root.Elements())
                {
                    string ip = el.Element("ip").Value;
                    string port = el.Element("port").Value;
                    string login = el.Element("login").Value;
                    string pwd = el.Element("pwd").Value;
                    string protocol_ = el.Element("protocol_type").Value;
                    protocol = (protocol_ == "HTTP") ? ProxyProtocol.HTTP : ProxyProtocol.SOCKS5;
                    Proxy pr = new Proxy(ip, port, login, pwd, protocol);
                    AddNewProxy(pr);
                }
            }
            catch (Exception ex)
            {
                if(SendMessage!=null)
                    SendMessage("Не удалось загрузить прокси, возможно файл proxies.xml повреждён");
                L.LW(ex);
            }

        }

        private void AddNewProxy(Proxy pr)
        {
            var proxies = ProxiesColl.Where(proxy => proxy.Ip == pr.Ip && proxy.Port == pr.Port).FirstOrDefault();
            if (proxies == null) ProxiesColl.Add(pr);
        }

        internal void AddProxyList()
        {
            proxiesStrings = Helpers.GetStringCollection(TextProxyList);
            AddProxiesToMod();
        }

        internal void AddProxiesToMod()
        {
            //if (proxiesStrings.Count == 0) return;

            protocol = (ProxyTypeIndex == 0) ? ProxyProtocol.HTTP : ProxyProtocol.SOCKS5;
            switch (ProxyFormatIndex)
            {
                case 0:
                    Format4();
                    break;
                case 1:
                    Format0('@');
                    break;
                case 2:
                    Format0('|');
                    break;
                case 3:
                    Format2();
                    break;
                case 4:
                    Format3();
                    break;
            }
            AddProxies(proxiesColl);
        }

        private void AddProxies(ObservableCollection<Proxy> proxiesColl)
        {
            string proxiesFile = @"proxies.xml";
            XDocument doc = new XDocument();
            XElement ps = new XElement("proxies");
            doc.Add(ps);

            foreach (Proxy pr in proxiesColl)
            {
                //AddProxy(pr.Ip, pr.Port, pr.Login, pr.Pwd, pr.ProxyProtocol.ToString());

                XElement proxy = new XElement("proxy",
                                new XElement("ip", pr.Ip),
                                new XElement("port", pr.Port),
                                new XElement("login", pr.Login),
                                new XElement("pwd", pr.Pwd),
                                new XElement("protocol_type", pr.ProxyProtocol.ToString()));

                doc.Root.Add(proxy);
            }

            doc.Save(proxiesFile);
        }

        private void Format0(char ch)
        {
            //ip:port@login:password
            //ip:port|login:password
            string errorsMessage = "";

            foreach (string proxy in proxiesStrings)
            {
                string ip = "";
                string port = "";
                string login = "";
                string password = "";

                bool isError = false;
                string[] s = proxy.Split(ch);
                if (s.Count<string>() == 2)
                {
                    string[] ss = s[0].Split(':');
                    if (ss.Count<string>() == 2)
                    {
                        ip = ss[0];
                        port = ss[1];

                        string[] sss = s[1].Split(':');
                        if (sss.Count<string>() == 2)
                        {
                            login = sss[0];
                            password = sss[1];
                        }
                        else
                        {
                            isError = true;
                        }
                    }
                    else
                    {
                        isError = true;
                    }
                }
                else
                {
                    isError = true;
                }
                if (isError)
                    errorsMessage += "Строка " + proxy + " не соответствует выбранному формату ip:port@login:password\n";
                else
                {
                    Proxy pr = new Proxy(ip, port, login, password, protocol);
                    AddNewProxy(pr);
                }
            }
            if (!String.IsNullOrEmpty(errorsMessage)) SendMessage(errorsMessage);
        }


        private void Format2()
        {
            //login:password@ip:port
            string errorsMessage = "";

            foreach (string proxy in proxiesStrings)
            {
                string ip = "";
                string port = "";
                string login = "";
                string password = "";

                bool isError = false;
                string[] s = proxy.Split('@');
                if (s.Count<string>() == 2)
                {
                    string[] ss = s[0].Split(':');
                    if (ss.Count<string>() == 2)
                    {
                        login = ss[0];
                        password = ss[1];

                        string[] sss = s[1].Split(':');
                        if (sss.Count<string>() == 2)
                        {
                            ip = sss[0];
                            port = sss[1];
                        }
                        else
                        {
                            isError = true;
                        }
                    }
                    else
                    {
                        isError = true;
                    }
                }
                else
                {
                    isError = true;
                }
                if (isError)
                    errorsMessage += "Строка " + proxy + " не соответствует выбранному формату ip:port@login:password\n";
                else
                {
                    Proxy pr = new Proxy(ip, port, login, password, protocol);
                    AddNewProxy(pr);
                }
            }
            if (!String.IsNullOrEmpty(errorsMessage)) SendMessage(errorsMessage);
        }

        private void Format3()
        {
            //login:password:ip:port
            string errorsMessage = "";

            foreach (string proxy in proxiesStrings)
            {
                string ip = "";
                string port = "";
                string login = "";
                string password = "";

                bool isError = false;
                string[] s = proxy.Split(':');
                if (s.Count<string>() == 4)
                {
                    login = s[0];
                    password = s[1];
                    ip = s[2];
                    port = s[3];
                }
                else
                {
                    isError = true;
                }
                if (isError)
                    errorsMessage += "Строка " + proxy + " не соответствует выбранному формату ip:port@login:password\n";
                else
                {
                    Proxy pr = new Proxy(ip, port, login, password, protocol);
                    AddNewProxy(pr);
                }
            }
            if (!String.IsNullOrEmpty(errorsMessage)) SendMessage(errorsMessage);
        }

        private void Format4()
        {
            //ip:port:login:password
            string errorsMessage = "";

            foreach (string proxy in proxiesStrings)
            {
                string ip = "";
                string port = "";
                string login = "";
                string password = "";

                bool isError = false;
                string[] s = proxy.Split(':');
                if (s.Count<string>() == 4)
                {
                    ip = s[0];
                    port = s[1];
                    login = s[2];
                    password = s[3];
                }
                else
                {
                    isError = true;
                }
                if (isError)
                    errorsMessage += "Строка " + proxy + " не соответствует выбранному формату ip:port@login:password\n";
                else
                {
                    Proxy pr = new Proxy(ip, port, login, password, protocol);
                    AddNewProxy(pr);
                }
            }
            if (!String.IsNullOrEmpty(errorsMessage)) SendMessage(errorsMessage);
        }

        internal void AddProxyFromFile()
        {
            proxiesStrings = Helpers.GetStringCollectionFromFile();
            AddProxiesToMod();
        }

        internal void CheckProxy()
        {
            throw new NotImplementedException();
        }

        internal void DelAllProxy()
        {
            ProxiesColl.Clear();
            string proxiesFile = @"proxies.xml";
            XDocument doc = new XDocument();
            XElement ps = new XElement("proxies");
            doc.Add(ps);
            doc.Save(proxiesFile);
        }

        #endregion



        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
