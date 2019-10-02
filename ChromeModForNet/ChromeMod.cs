using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDevs.ChromeDevTools;
using Chrome = MasterDevs.ChromeDevTools.Protocol.Chrome;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Network;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Fetch;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Page;
using MasterDevs.ChromeDevTools.Protocol.Chrome.DOM;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Net.Http;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UsefulThings;

namespace ChromeModForNet
{
    public class ChromeMod : INotifyPropertyChanged
    {
        string currentUrl;

        public string CurrentUrl
        {
            get { return currentUrl; }
            set
            {
                if (currentUrl != value)
                    currentUrl = value;
                OnPropertyChanged();
            }
        }


        public IChromeProcess chromeProcess;
        public IChromeSession chromeSession;
        bool isDebug;
        public int port = 0;

        #region Init

        async public Task<CallResult<IChromeSession>> GetChromeSession(string proxisString, string ua, bool isDebug = false, bool headless = true)
        {
            L.Trace();
            CallResult<IChromeSession> chromeSessionResult = new CallResult<IChromeSession>();

            Proxy proxy = (proxisString != null && !String.IsNullOrWhiteSpace(proxisString)) ? new Proxy(proxisString) : null;
            this.isDebug = isDebug;
            var chromeProcessFactory = new ChromeProcessFactory(new StubbornDirectoryCleaner());
            port = GetFreeLocalPort();

            try
            {
                if (proxy != null)
                {
                    string ipPort = proxy.Ip + ":" + proxy.Port;
                    chromeProcess = chromeProcessFactory.Create(port, headless, ipPort);
                }
                else
                {
                    chromeProcess = chromeProcessFactory.Create(port, headless);
                }

                var sessionInfoArray = await chromeProcess.GetSessionInfo();
                var sessionInfo = sessionInfoArray.LastOrDefault();

                var chromeSessionFactory = new ChromeSessionFactory();
                chromeSession = chromeSessionFactory.Create(sessionInfo.WebSocketDebuggerUrl);

                chromeSession.Subscribe<RequestPausedEvent>(requestPausedEvent =>
                {
                    RequestPausedEventHandler(requestPausedEvent);
                });

                chromeSession.Subscribe<AuthRequiredEvent>(authRequiredEvent =>
                {
                    AuthRequiredEventHandler(authRequiredEvent);
                });

                chromeSession.Subscribe<FrameNavigatedEvent>(frameNavigatedEvent =>
                {
                    FrameNavigatedEventHandler(frameNavigatedEvent);
                });

                chromeSession.Subscribe<LoadingFinishedEvent>(loadingFinishedEvent =>
                {
                    LoadingFinishedEventHandler(loadingFinishedEvent);
                });
                /*
                await chromeSession.SendAsync(new SetDeviceMetricsOverrideCommand
                {
                    Width = GlobalVars.ViewPortWidth,
                    Height = GlobalVars.ViewPortHeight,
                    Scale = 1
                });*/
                /*
                //Target.setDiscoverTargets
                var setDiscoverTargetsResult = await chromeSession.SendAsync(new Chrome.Target.SetDiscoverTargetsCommand
                {
                    Discover=true
                });
                //Target.createTarget
                var createTargetResult = await chromeSession.SendAsync(new Chrome.Target.CreateTargetCommand
                {
                    Url= "about:blank"
                });
                string targetId = createTargetResult.Result.TargetId;
                //Target.attachToTarget
                var attachToTargetResult = await chromeSession.SendAsync(new Chrome.Target.AttachToTargetCommand
                {
                    TargetId = targetId,
                    Flatten=true

                });
                string sessionId = attachToTargetResult.Result.SessionId;
                chromeSession.MainSessionId = sessionId;*/
                //enable page
                var pageEnableResult = await chromeSession.SendAsync<Chrome.Page.EnableCommand>();
                //Page.getFrameTree
                var getFrameTreeResult = await chromeSession.SendAsync<Chrome.Page.GetFrameTreeCommand>();
                //Target.setAutoAttach
                var setAutoAttachResult = await chromeSession.SendAsync(new Chrome.Target.SetAutoAttachCommand
                {
                    AutoAttach = true,
                    WaitForDebuggerOnStart = false,
                    Flatten = true

                });
                //Performance.enable
                var performanceEnableResult = await chromeSession.SendAsync<Chrome.Performance.EnableCommand>();
                //enable network
                var enableNetwork = await chromeSession.SendAsync(new Chrome.Network.EnableCommand());
            }
            catch (Exception ex)
            {
                chromeSessionResult.Error = new UsefulThings.Error(ex.Message);
                return chromeSessionResult;
            }
            await Task.Delay(1000);



            //proxy auth
            if (proxy != null)
            {
                if (!String.IsNullOrWhiteSpace(proxy.Login)) await ProxyAuthenticate(proxy.Login, proxy.Pwd);
            }

            await SetUA(ua);
            chromeSessionResult.Data = chromeSession;
            return chromeSessionResult;
        }

        /// <summary>
        /// Для присоединения к хрому без запуска процесса
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        async public Task<CallResult<IChromeSession>> GetChromeSession(int port)
        {
            CallResult<IChromeSession> chromeSessionResult = new CallResult<IChromeSession>();

            ChromeSessionInfo sessionInfo = null;
            CallResult<ChromeSessionInfo[]> result = await GetSessionInfo(port);
            if (result.Success)
            {
                sessionInfo = result.Data[0];
            }
            else
            {
                chromeSessionResult.Error = new UsefulThings.Error(result.Error.Message);
                return chromeSessionResult;
            }

            try
            {
                var chromeSessionFactory = new ChromeSessionFactory();
                chromeSession = chromeSessionFactory.Create(sessionInfo.WebSocketDebuggerUrl);
            }
            catch (Exception ex)
            {
                chromeSessionResult.Error = new UsefulThings.Error(ex.Message);
                return chromeSessionResult;
            }

            chromeSession.Subscribe<RequestPausedEvent>(requestPausedEvent =>
            {
                RequestPausedEventHandler(requestPausedEvent);
            });

            chromeSession.Subscribe<AuthRequiredEvent>(authRequiredEvent =>
            {
                AuthRequiredEventHandler(authRequiredEvent);
            });

            chromeSession.Subscribe<FrameNavigatedEvent>(frameNavigatedEvent =>
            {
                FrameNavigatedEventHandler(frameNavigatedEvent);
            });

            chromeSession.Subscribe<LoadingFinishedEvent>(loadingFinishedEvent =>
            {
                LoadingFinishedEventHandler(loadingFinishedEvent);
            });

            //enable page
            var pageEnableResult = await chromeSession.SendAsync<Chrome.Page.EnableCommand>();
            //enable network
            var enableNetwork = await chromeSession.SendAsync(new Chrome.Network.EnableCommand());

            await SetUA(null);
            chromeSessionResult.Data = chromeSession;
            return chromeSessionResult;
        }

        public async Task<CallResult<ChromeSessionInfo[]>> GetSessionInfo(int port)
        {
            CallResult<ChromeSessionInfo[]> result = new CallResult<ChromeSessionInfo[]>();
            try
            {
                HttpClient http = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:" + port.ToString())
                };

                string json = await http.GetStringAsync("/json");
                result.Data = JsonConvert.DeserializeObject<ChromeSessionInfo[]>(json);
            }
            catch (Exception ex)
            {
                result.Error = new UsefulThings.Error(ex.Message);
                L.LW(ex);
            }
            return result;
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

            for (int i = Settings.MinDebugPort; i < Settings.MaxDebugPort; i++)
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

        async public Task SetCookies(IAcc acc)
        {
            L.Trace();
            string json = acc.Cookies.Value;
            JArray ja = JArray.Parse(json);
            List<Cookie> cookiesList = new List<Cookie>();
            foreach (var cookie in ja)
            {
                double expires = Double.Parse(cookie["Expires"].ToString());
                bool secure = (cookie["Secure"].ToString() == "true") ? true : false;
                bool httpOnly = (cookie["HttpOnly"].ToString() == "true") ? true : false;

                var setCookie = await chromeSession.SendAsync(new SetCookieCommand
                {
                    Name = cookie["Name"].ToString(),
                    Value = cookie["Value"].ToString(),
                    Domain = cookie["Domain"].ToString(),
                    Path = cookie["Path"].ToString(),
                    Secure = secure,
                    HttpOnly = httpOnly,
                    SameSite = cookie["SameSite"].ToString(),
                    Expires = expires
                });
            }
        }

        async public Task<CommandResponse> SetUA(string ua)
        {
            if (ua == null) ua = Settings.UseragentOverride;
            var setUserAgentOverrideCommandResponse = await chromeSession.SendAsync(new SetUserAgentOverrideCommand
            {
                UserAgent = ua
            });
            return setUserAgentOverrideCommandResponse;
        }

        async private Task ProxyAuthenticate(string proxyUser, string proxyPass)
        {
            chromeSession.ProxyAuthenticate(proxyUser, proxyPass);

            await chromeSession.SendAsync(new SetCacheDisabledCommand { CacheDisabled = true });

            RequestPattern[] patterns = { new RequestPattern { UrlPattern = "*" } };
            await chromeSession.SendAsync(new Chrome.Fetch.EnableCommand { HandleAuthRequests = true, Patterns = patterns });
        }

        async private void LoadingFinishedEventHandler(LoadingFinishedEvent loadingFinishedEvent)
        {
            WriteObject(loadingFinishedEvent);
            CurrentUrl = await GetCurrentUrl();
        }

        private void FrameNavigatedEventHandler(FrameNavigatedEvent frameNavigatedEvent)
        {
            WriteObject(frameNavigatedEvent);
            if (String.IsNullOrWhiteSpace(frameNavigatedEvent.Frame.ParentId))
            {
                CurrentUrl = frameNavigatedEvent.Frame.Url;
            }
            //Console.WriteLine(frameNavigatedEvent.Frame.ParentId);
            //Console.WriteLine(frameNavigatedEvent.Frame.Name);
            //Console.WriteLine(frameNavigatedEvent.Frame.UnreachableUrl);
            Console.WriteLine(frameNavigatedEvent.Frame.Url);
        }

        private void AuthRequiredEventHandler(AuthRequiredEvent authRequiredEvent)
        {
            WriteObject(authRequiredEvent);
            string requestId = authRequiredEvent.RequestId;

            Chrome.Fetch.AuthChallengeResponse acr = new Chrome.Fetch.AuthChallengeResponse
            {
                Response = "ProvideCredentials",
                Username = chromeSession.ProxyUser,
                Password = chromeSession.ProxyPass
            };

            var auth = chromeSession.SendAsync(new ContinueWithAuthCommand
            {
                RequestId = requestId,
                AuthChallengeResponse = acr
            });
        }

        private void RequestPausedEventHandler(RequestPausedEvent requestPausedEvent)
        {
            WriteObject(requestPausedEvent);
            string requestId = requestPausedEvent.RequestId;
            var cont = chromeSession.SendAsync(new ContinueRequestCommand { RequestId = requestId });
        }

        #endregion


        async public Task Navigate(string url)
        {
            var navigateResponse = await chromeSession.SendAsync(new NavigateCommand
            {
                Url = url
            });
        }

        async public Task Reload()
        {
            try
            {
                var reloadResponse = await chromeSession.SendAsync<ReloadCommand>();
            }
            catch (Exception ex)
            {
                L.LW(ex);
            }
        }

        async public Task GoBack()
        {
            try
            {
                var historyResponseResult = ((CommandResponse<GetNavigationHistoryCommandResponse>)(await chromeSession.SendAsync<GetNavigationHistoryCommand>())).Result;
                if (historyResponseResult.CurrentIndex > 0)
                {
                    var entryId = historyResponseResult.Entries[historyResponseResult.CurrentIndex - 1].Id;
                    var qs = await chromeSession.SendAsync(new NavigateToHistoryEntryCommand
                    {
                        EntryId = long.Parse(entryId.ToString())
                    });
                }
            }
            catch (Exception ex)
            {
                L.LW(ex);
            }
        }

        async public Task GoForward()
        {
            try
            {
                var historyResponseResult = ((CommandResponse<GetNavigationHistoryCommandResponse>)(await chromeSession.SendAsync<GetNavigationHistoryCommand>())).Result;
                long targetIndex = historyResponseResult.CurrentIndex + 1;
                long availableIndex = historyResponseResult.Entries.Count() - 1;
                if (targetIndex <= availableIndex)
                {
                    var entryId = historyResponseResult.Entries[historyResponseResult.CurrentIndex + 1].Id;
                    var qs = await chromeSession.SendAsync(new NavigateToHistoryEntryCommand
                    {
                        EntryId = long.Parse(entryId.ToString())
                    });
                }
            }
            catch (Exception ex)
            {
                L.LW(ex);
            }
        }

        async public Task<bool> IsElementExist(string cssSelector)
        {
            long id = await IsElementExist(cssSelector, new StringCollection());
            if (id == 0)
                return false;
            else
                return true;
        }

        async public Task<long> IsElementExist(string cssSelector, StringCollection tampax = null)
        {
            try
            {
                long docNodeId = GetDocNodeID();

                var qs = await chromeSession.SendAsync(new QuerySelectorCommand
                {
                    NodeId = docNodeId,
                    Selector = cssSelector
                });
                var elementNodeId = qs.Result.NodeId;
                return elementNodeId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        async public Task<bool> IsElementExist(string cssSelector, string text)
        {
            long id = await IsElementExist(cssSelector, text, new StringCollection());
            if (id == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Только что выяснил, что для перегрузки обязательно менять принимаемые параметры, думал что можно сменить тип возвращаемого значения,
        /// но tampax даже мне поможет двигаться уверенно и непринуждённо в критические дни
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <param name="text"></param>
        /// <param name="tampax"></param>
        /// <returns></returns>
        async public Task<long> IsElementExist(string cssSelector, string text, StringCollection tampax = null)
        {
            try
            {
                long docNodeId = GetDocNodeID();

                var qs = await chromeSession.SendAsync(new QuerySelectorAllCommand
                {
                    NodeId = docNodeId,
                    Selector = cssSelector
                });
                var nodeIds = qs.Result.NodeIds;

                long nodeId = 0;
                foreach (var id in nodeIds)
                {
                    Console.WriteLine(id);
                    var getHTML = await chromeSession.SendAsync(new GetOuterHTMLCommand
                    {
                        NodeId = id
                    });
                    string outer = getHTML.Result.OuterHTML;
                    if (outer.Contains(text))
                    {
                        nodeId = id;
                        break;
                    }
                }

                return nodeId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        async public Task<string> GetCurrentUrl()
        {
            string url = "";
            try
            {
                var qs = await chromeSession.SendAsync<GetFrameTreeCommand>();
                var qs2 = chromeSession.SendAsync<GetNavigationHistoryCommand>().Result;

                ICommandResponse cr = chromeSession.SendAsync<GetNavigationHistoryCommand>().Result;
                long index = ((CommandResponse<GetNavigationHistoryCommandResponse>)cr).Result.CurrentIndex;
                url = ((CommandResponse<GetNavigationHistoryCommandResponse>)cr).Result.Entries[index].Url;

                //var rrr = (GetNavigationHistoryCommandResponse)qs2;
                //url = qs2.res
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return url;
        }

        async public Task<CDPResponse> InsertText(string cssSelector, string text)
        {
            CDPResponse cdpr = new CDPResponse();
            try
            {
                long docNodeId = GetDocNodeID();

                var qs = await chromeSession.SendAsync(new QuerySelectorCommand
                {
                    NodeId = docNodeId,
                    Selector = cssSelector
                });
                var elementNodeId = qs.Result.NodeId;

                var sv = await chromeSession.SendAsync(new FocusCommand
                {
                    NodeId = elementNodeId
                });

                var it = await chromeSession.SendAsync(new Chrome.Input.InsertTextCommand
                {
                    Text = text
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cdpr.ErrorMessage = ex.Message;
            }
            return cdpr;
        }

        async public Task<Cookie[]> GetCookies()
        {
            Cookie[] cookies = null;
            try
            {
                cookies = (await chromeSession.SendAsync(new GetAllCookiesCommand())).Result.Cookies;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return cookies;
        }

        async public Task<long> GetNodeId(string cssSelector)
        {
            long elementNodeId = 0;
            try
            {
                long docNodeId = GetDocNodeID();

                var qs = await chromeSession.SendAsync(new QuerySelectorCommand
                {
                    NodeId = docNodeId,
                    Selector = cssSelector
                });
                elementNodeId = qs.Result.NodeId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return elementNodeId;
        }

        async public Task<string> GetAttribute(long nodeId, string attrName)
        {
            string attrValue = null;
            try
            {
                var qs = await chromeSession.SendAsync(new GetAttributesCommand
                {
                    NodeId = nodeId
                });
                var attrArray = qs.Result;
                for (int i = 0; i < qs.Result.Attributes.Count<string>(); i = i + 2)
                {
                    if (qs.Result.Attributes[i] == attrName)
                    {
                        attrValue = qs.Result.Attributes[i + 1];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return attrValue;
        }

        async public Task RunScript(string script)
        {
            try
            {
                var qs = await chromeSession.SendAsync(new AddScriptToEvaluateOnNewDocumentCommand
                {
                    Source = script
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async public Task<string> GetHtml()
        {
            string outerHtml = "";
            try
            {
                long docNodeId = GetDocNodeID();
                var htmlRes = await chromeSession.SendAsync(new GetOuterHTMLCommand
                {
                    NodeId = docNodeId
                });
                outerHtml = htmlRes.Result.OuterHTML;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return outerHtml;
        }

        async public Task ScrollToElement(string cssSelector, int plus = 0)
        {
            try
            {
                long nodeId = await GetNodeId(cssSelector);
                await ScrollToElement(nodeId, plus);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async public Task ScrollToElement(long nodeId, int plus = 0)
        {
            try
            {
                var elementBox = await chromeSession.SendAsync(new GetBoxModelCommand
                {
                    NodeId = nodeId
                });
                int lowPosition = (int)elementBox.Result.Model.Border[5];

                var metrics = await chromeSession.SendAsync(new GetLayoutMetricsCommand { });
                int height = (int)metrics.Result.LayoutViewport.ClientHeight;
                int delta = lowPosition - height + plus;

                var scroll = await chromeSession.SendAsync(new Chrome.Input.DispatchMouseEventCommand
                {
                    Type = "mouseWheel",
                    X = 1,
                    Y = 2,
                    DeltaX = 0,
                    DeltaY = delta
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async public Task Scroll(int plus)
        {
            try
            {
                var scroll = await chromeSession.SendAsync(new Chrome.Input.DispatchMouseEventCommand
                {
                    Type = "mouseWheel",
                    X = 1,
                    Y = 2,
                    DeltaX = 0,
                    DeltaY = plus
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private long GetDocNodeID()
        {
            long docNodeId = 0;
            try
            {
                ICommandResponse cr = chromeSession.SendAsync(new GetDocumentCommand
                {
                    //Depth=-1
                }).Result;
                docNodeId = ((CommandResponse<GetDocumentCommandResponse>)cr).Result.Root.NodeId;
                return docNodeId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        async public Task<CDPResponse> Click(string cssSelector)
        {
            CDPResponse cdpr = new CDPResponse();
            try
            {
                long docNodeId = GetDocNodeID();
                var qsButton = chromeSession.SendAsync(new QuerySelectorCommand
                {
                    NodeId = docNodeId,
                    Selector = cssSelector
                }).Result;
                var elementNodeId = qsButton.Result.NodeId;

                var elementBox = chromeSession.SendAsync(new GetBoxModelCommand
                {
                    NodeId = elementNodeId
                });
                var elementBoxRes = elementBox.Result.Result.Model;

                double leftBegin = elementBoxRes.Border[0];
                double leftEnd = elementBoxRes.Border[2];
                double topBegin = elementBoxRes.Border[1];
                double topEnd = elementBoxRes.Border[5];

                double x = Math.Round((leftBegin + leftEnd) / 2, 2);
                double y = Math.Round((topBegin + topEnd) / 2, 2);

                var click = chromeSession.SendAsync(new Chrome.Input.DispatchMouseEventCommand
                {
                    Type = "mousePressed",
                    X = x,
                    Y = y,
                    ClickCount = 1,
                    Button = "left"
                });

                await Task.Delay(100);

                var clickReleased = chromeSession.SendAsync(new Chrome.Input.DispatchMouseEventCommand
                {
                    Type = "mouseReleased",
                    X = x,
                    Y = y,
                    ClickCount = 1,
                    Button = "left"
                });

                Console.WriteLine("midle " + x + " " + y);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cdpr.ErrorMessage = ex.Message;
            }
            return cdpr;
        }

        async public Task<CDPResponse> Click(long nodeId)
        {
            CDPResponse cdpr = new CDPResponse();
            try
            {
                var elementBox = await chromeSession.SendAsync(new GetBoxModelCommand
                {
                    NodeId = nodeId
                });
                var elementBoxRes = elementBox.Result.Model;

                double leftBegin = elementBoxRes.Border[0];
                double leftEnd = elementBoxRes.Border[2];
                double topBegin = elementBoxRes.Border[1];
                double topEnd = elementBoxRes.Border[5];

                double x = Math.Round((leftBegin + leftEnd) / 2, 2);
                double y = Math.Round((topBegin + topEnd) / 2, 2);

                var click = chromeSession.SendAsync(new Chrome.Input.DispatchMouseEventCommand
                {
                    Type = "mousePressed",
                    X = x,
                    Y = y,
                    ClickCount = 1,
                    Button = "left"
                });

                await Task.Delay(100);

                var clickReleased = chromeSession.SendAsync(new Chrome.Input.DispatchMouseEventCommand
                {
                    Type = "mouseReleased",
                    X = x,
                    Y = y,
                    ClickCount = 1,
                    Button = "left"
                });

                Console.WriteLine("midle " + x + " " + y);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cdpr.ErrorMessage = ex.Message;
            }
            return cdpr;
        }

        private static void WriteObject(Object ob)

        {
            return;
            string obString = JsonConvert.SerializeObject(ob);
            Console.WriteLine("RECIVE <<< " + ob.GetType() + " " + obString);
        }


        #region Screen

        async public Task<CDPResponse> GetNodeScreenshot(string cssSelector, string pathForSave)
        {
            long nodeId = await GetNodeId(cssSelector);
            CDPResponse resp = await GetNodeScreenshot(nodeId, pathForSave);
            return resp;
        }

        async public Task<CDPResponse> GetNodeScreenshot(long nodeId, string pathForSave)
        {
            CDPResponse cdpr = new CDPResponse();
            try
            {
                var box = await chromeSession.SendAsync(new GetBoxModelCommand
                {
                    NodeId = nodeId
                });
                var boxRes = box.Result.Model;

                double leftBegin = boxRes.Border[0];
                double leftEnd = boxRes.Border[2];
                double topBegin = boxRes.Border[1];
                double topEnd = boxRes.Border[5];

                var screenshot = await chromeSession.SendAsync(new CaptureScreenshotCommand { Format = "png" });
                var byteArr = Convert.FromBase64String(screenshot.Result.Data);
                //File.WriteAllBytes("outputButton.png", byteArr);

                int leftTopX = Convert.ToInt32(leftBegin);
                int leftTopY = Convert.ToInt32(topBegin);
                int X = Convert.ToInt32(leftEnd - leftBegin);
                int Y = Convert.ToInt32(topEnd - topBegin);

                Rectangle rectangle = new Rectangle(leftTopX, leftTopY, X, Y);
                //Bitmap bmp = new Bitmap("outputButton.png");

                Bitmap bmp;
                using (var ms = new MemoryStream(byteArr))
                {
                    bmp = new Bitmap(ms);
                }
                Bitmap bmp2 = bmp.Clone(rectangle, PixelFormat.Format64bppPArgb);
                bmp2.Save(pathForSave, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cdpr.ErrorMessage = ex.Message;
            }
            return cdpr;
        }

        public BitmapImage CaptureScreenshot(string format)
        {
            var screenshot = chromeSession.SendAsync(new CaptureScreenshotCommand { Format = "png" }).Result;
            var byteArr = Convert.FromBase64String(screenshot.Result.Data);

            Bitmap bmp;
            using (var ms = new MemoryStream(byteArr))
            {
                bmp = new Bitmap(ms);
            }
            return ConvertBitmap(bmp);
        }

        public BitmapImage ConvertBitmap(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
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
