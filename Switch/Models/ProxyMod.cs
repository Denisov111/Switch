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

        public static Dictionary<string, bool> Revealingheader { get; set; }
        StringCollection proxiesStrings;
        ProxyProtocol protocol;

        #region MVVM


        #region Fields

        string textProxyList;
        int proxyFormatIndex;
        int proxyTypeIndex;
        ObservableCollection<Proxy> proxiesColl = new ObservableCollection<Proxy>();
        public string profileLang;

        #endregion


        #region Properties

        public string ProfileLang
        {
            get { return profileLang; }
            set
            {
                profileLang = value;
                OnPropertyChanged();
            }
        }

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

        #region Fields

        public ViewModels.ProxiesViewModel proxiesViewModel;
        public ProxiesView view;

        #endregion

        #region Main code

        public ProxyMod()
        {
            ProxyCollInit();

        }

        internal void Run(Global global)
        {
            proxiesViewModel = new ViewModels.ProxiesViewModel(this);
            view = new ProxiesView(global.Lang, proxiesViewModel);
            //ProxiesView f = new ProxiesView(global.Lang, this);
            SendMessage += view.onSendMessage;
            view.ShowDialog();
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
                if (SendMessage != null)
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

        async internal Task CheckProxy(Proxy proxy)
        {
            ProxyJudge pj = new ProxyJudge();
            string ownIp = null;
            string judge = "http://azenv.net/";

            string result_ = await Net.RespAsync(judge);
            if(String.IsNullOrEmpty(result_))
            {
                proxy.Status = ProxyWorkStatus.NotAvailable;
                return;
            }
            else
            {
                proxy.Status = ProxyWorkStatus.IsAvailable;
            }
            string ip = "";

            Dictionary<string, string> values_ = pj.parse(result_);

            if (values_.TryGetValue("REMOTE_ADDR", out ip))
            {
                ownIp = ip;
                L.LW("Found own ip: " + ip);
            }
            else
            {
                throw new Exception("REMOTE_ADDR missing in field");
            }

            if (Revealingheader == null) RevealingheaderInit();

            string result = Net.Resp(judge);
            Dictionary<string, string> values = pj.parse(result);

            // Test to se if our ip adress exist in the result

            proxy.Anonimity = Anonimity.Unknow;
            if (result == "")
            {
                proxy.Anonimity = Anonimity.Unknow;
                throw new Exception("Empty response");
            }

            if (result.IndexOf("REQUEST_URI") == -1)
            {
                proxy.Anonimity = Anonimity.Unknow;
                throw new Exception("Did not find header info. The proxy may tamper with the response");
            }

            // Will go troght black and whitlist to see what level of anonymity this server provides
            foreach (KeyValuePair<string, string> value in values)
            {
                bool revealing;
                if (Revealingheader.TryGetValue(value.Key, out revealing)) // Returns true.
                {
                    if (revealing)
                    {
                        proxy.Anonimity = Anonimity.Low;
                        // Debug: _Global.log("Found revealing header " + value.Key + " : " + value.Value);
                        L.LW("Found revealing header " + value.Key + " = " + value.Value + ". ");
                    }
                }
                else
                {
                    proxy.Anonimity = Anonimity.Low;
                    // Debug: _Global.log("Have unknown header '" + value.Key + " : " + value.Value);
                    L.LW("Have unknown header '" + value.Key + " : " + value.Value + ". ");
                }
            }

            if (result.IndexOf(proxy.Ip) != -1)
            {
                // Debug: _Global.log(server.Ip + ": Have your ip in results");
                L.LW("Have your ip in results. ");
                proxy.Anonimity = Anonimity.None;
            }

            Console.WriteLine(proxy.Anonimity);
        }

        private void RevealingheaderInit()
        {
            // Hardcoded list of bad and good headers. 
            Revealingheader = new Dictionary<string, bool>()
            {
                {"HTTP_CACHE_CONTROL", true},
                {"HTTP_CDN_SRC_IP", true},
                {"HTTP_CLIENT_IP", true},
                {"HTTP_REFERER", true},
                {"HTTP_IF_NONE_MATCH", true},
                {"HTTP_IF_MODIFIED_SINCE", true},
                {"HTTP_MAX_FORWARDS", true},
                {"HTTP_OCT_MAX_AGE", true},
                {"HTTP_PROXY_AGENT", true},
                {"HTTP_PROXY_CONNECTION", true},
                {"HTTP_VIA", true},
                {"HTTP_X_ACCEPT_ENCODING_PRONTOWIFI", true},
                {"HTTP_X_BLUECOAT_VIA", true},
                {"HTTP_X_FORWARDED_FOR", true},
                {"HTTP_X_FORWARD_FOR", true},
                {"HTTP_X_FORWARDED_HOST", true},
                {"HTTP_X_FORWARDED_SERVER", true},
                {"HTTP_X_MATO_PARAM", true},
                {"HTTP_X_NAI_ID", true},
                {"HTTP_X_PROXY_ID", true},
                {"HTTP_X_REAL_IP", true},
                {"HTTP_X_VIA", true},
                {"HTTP_XCNOOL_REMOTE_ADDR", true},
                {"HTTP_XROXY_CONNECTION", true},
                {"HTTP_XXPECT", true},
                {"HTTP_ACCEPT", false},
                {"HTTP_ACCEPT_ENCODING", false},
                {"HTTP_ACCEPT_LANGUAGE", false},
                {"HTTP_CONNECTION", false},
                {"HTTP_HOST", false},
                {"HTTP_USER_AGENT", false},
                {"REMOTE_ADDR", false},
                {"REMOTE_PORT", false},
                {"REQUEST_METHOD", false},
                {"REQUEST_TIME", false},
                {"REQUEST_TIME_FLOAT", false},
                {"REQUEST_SCHEME", false},
                {"REQUEST_URI", false},

            };
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
                default:
                    throw new NotImplementedException();
            }
        }

        private void AddProfile()
        {
            throw new NotImplementedException();
        }

        async internal void OnSendCommandWithObjectCommandHandler(object objectValue)
        {
            Proxy proxy = (Proxy)((System.Windows.Controls.Button)objectValue).DataContext;
            try
            {
                await CheckProxy(proxy);
            }
            catch(Exception ex)
            {
                L.LW(ex);
            }
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
        }

        #endregion
    }
}
