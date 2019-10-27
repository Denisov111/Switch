using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Threading;
using System.Net;
//using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using xNet.Net;

namespace UsefulThings
{
    public class Net
    {
        public static string Resp(string requestPath)    //на вход подаем URL API
        {
            string response = String.Empty;
            for (int i = 0; i <= 5; i++)
            {
                try
                {
                    HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(requestPath);   //отправление запроса к серверу API

                    HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();      //получение ответа от сервера API
                    StreamReader myStream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
                    string responseString = myStream.ReadToEnd();
                    Response.Close();

                    return responseString;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(500);
                    L.LW("Сбой метода Resp :\n" + ex.ToString());
                }
            }
            return response;
        }

        async public static Task<string> RespAsync(string requestPath, Proxy proxy = null, int attemptCount=5)   
        {

            if (proxy == null)
            {
                string response = string.Empty;
                for (int i = 0; i < attemptCount; i++)
                {
                    try
                    {
                        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(requestPath);  
                        WebResponse resp = await Request.GetResponseAsync();
                        HttpWebResponse Response = (HttpWebResponse)resp;      
                        StreamReader myStream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
                        string responseString = myStream.ReadToEnd();
                        Response.Close();

                        return responseString;
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(500);
                        L.LW("Сбой метода Resp :\n" + ex.ToString());
                    }
                }
                return null;
            }
            else
            {
                string response = string.Empty;
                for (int i = 0; i < attemptCount; i++)
                {
                    try
                    {
                        HttpRequest request = new HttpRequest();
                        ProxyType proxyType = (proxy.ProxyProtocol == ProxyProtocol.HTTP) ? ProxyType.Http: ProxyType.Socks5;
                        ProxyClient client = ProxyHelper.CreateProxyClient(proxyType, proxy.Ip, Convert.ToInt32(proxy.Port), proxy.Login, proxy.Pwd);
                        request.Proxy = client;

                        string content = "";
                        await Task.Run(() =>
                        {
                            try
                            {
                                content = request.Get(requestPath).ToString();
                            }
                            catch (Exception ex)
                            {
                                L.LW(ex);
                            }

                        });
                        if (content == "")
                        {
                            await Task.Delay(500);
                            continue;
                        }
                        return content;
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(500);
                        L.LW("Сбой метода Resp :\n" + ex.ToString());
                    }
                }
                return null;
            }
        }

        //public static string Resp(string request_path, string proxy)
        //{
        //    Console.WriteLine("Строка прокси: " + proxy);
        //    if (proxy == String.Empty || proxy == "" || proxy == null)
        //    {
        //        Thread.Sleep(500);
        //        string response = string.Empty;
        //        for (int i = 0; i <= 5; i++)
        //        {
        //            try
        //            {
        //                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(request_path);   //отправление запроса к серверу API

        //                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();      //получение ответа от сервера API
        //                StreamReader myStream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
        //                string responseX = myStream.ReadToEnd();
        //                Response.Close();

        //                return responseX;
        //            }
        //            catch (Exception ex)
        //            {
        //                Thread.Sleep(1000);
        //                L.LW("Сбой метода Resp :\n" + ex.ToString());
        //            }
        //        }
        //        L.LW("Что-то пошло не так, VK не отвечает\n");
        //        return null;
        //    }
        //    else
        //    {
        //        Thread.Sleep(500);
        //        string response = string.Empty;
        //        for (int i = 0; i <= 5; i++)
        //        {
        //            try
        //            {
        //                xNet.Net.HttpRequest request = new xNet.Net.HttpRequest();
        //                Proxy.MakeClient(request, proxy);
        //                string content = request.Get(request_path).ToString();

        //                return content;
        //            }
        //            catch (Exception ex)
        //            {
        //                Thread.Sleep(1000);
        //                Global.f.EventMonitor("Сбой метода Resp с прокси.", ex);
        //                Console.WriteLine(proxy);
        //            }
        //        }
        //        L.LW("Что-то пошло не так, VK не отвечает\n");
        //        return null;
        //    }

        //}

        //public static string Resp(string request_path, Acc acc)    //на вход подаем URL API
        //{
        //    if (acc == null)
        //    {
        //        System.Windows.MessageBox.Show("Не назначен аккаунт для запроса.");
        //        return null;
        //    }
        //    Console.WriteLine("Строка прокси: " + acc.proxy);
        //    if (acc.proxy == String.Empty || acc.proxy == "" || acc.proxy == null)
        //    {
        //        Thread.Sleep(500);
        //        string response = string.Empty;
        //        for (int i = 0; i <= 5; i++)
        //        {
        //            try
        //            {
        //                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(request_path);   //отправление запроса к серверу API

        //                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();      //получение ответа от сервера API
        //                StreamReader myStream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
        //                string responseX = myStream.ReadToEnd();
        //                Response.Close();

        //                return responseX;
        //            }
        //            catch (Exception ex)
        //            {
        //                Thread.Sleep(1000);
        //                L.LW("Сбой метода Resp :\n" + ex.ToString());
        //                if (ex.ToString().Contains("Удаленный сервер возвратил ошибку: (401) Несанкционированный"))
        //                {
        //                    return "Сервер ВК вернул ошибку, если это происходит во время авторизации, то возможно приложение ВК было удалено, перезагрузите программу и попытайтесь авторизоваться снова";
        //                }
        //            }
        //        }
        //        L.LW("Что-то пошло не так, VK не отвечает\n");
        //        return null;
        //    }
        //    else
        //    {
        //        Thread.Sleep(500);
        //        string response = string.Empty;
        //        for (int i = 0; i <= 5; i++)
        //        {
        //            try
        //            {
        //                xNet.Net.HttpRequest request = new xNet.Net.HttpRequest();
        //                Proxy.MakeClient(request, acc.proxy);
        //                string content = request.Get(request_path).ToString();

        //                return content;
        //            }
        //            catch (Exception ex)
        //            {
        //                Thread.Sleep(1000);
        //                Global.f.EventMonitor("Сбой метода Resp с прокси.", ex);
        //                if (ex.ToString().Contains("Удаленный сервер возвратил ошибку: (401) Несанкционированный"))
        //                {
        //                    return "Сервер ВК вернул ошибку, если это происходит во время авторизации, то возможно приложение ВК было удалено, перезагрузите программу и попытайтесь авторизоваться снова";
        //                }
        //            }
        //        }
        //        L.LW("Что-то пошло не так, VK не отвечает\n");
        //        return null;
        //    }
        //}

        //public static string Resp_POST(string request_path, string parameters, string referrer = null)    //на вход подаем URL API
        //{
        //    Thread.Sleep(500);
        //    string response = string.Empty;
        //    for (int i = 0; i <= 5; i++)
        //    {
        //        int k = i + 1;
        //        L.LW("Отправляем запрос на сервер ВК(" + k.ToString() + ")");
        //        try
        //        {
        //            //Отправляем данные на сервер
        //            byte[] b = System.Text.Encoding.UTF8.GetBytes(parameters);

        //            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(request_path);
        //            myReq.Timeout = 15000;
        //            myReq.Method = "POST";
        //            if (referrer != null) myReq.Referer = referrer;
        //            myReq.ContentType = "application/x-www-form-urlencoded";
        //            myReq.ContentLength = b.Length;
        //            myReq.GetRequestStream().Write(b, 0, b.Length);

        //            HttpWebResponse Response = (HttpWebResponse)myReq.GetResponse();      //получение ответа от сервера API
        //            StreamReader myStream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
        //            response = myStream.ReadToEnd();
        //            Response.Close();

        //            return response;
        //        }
        //        catch
        //        {
        //            Thread.Sleep(1000);
        //        }
        //    }
        //    L.LW("Что-то пошло не так, VK не отвечает\n");
        //    return null;
        //}

        //public static string Resp_POST(string request_path, string parameters, Acc acc)
        //{
        //    if (acc == null)
        //    {
        //        return Resp_POST(request_path, parameters);
        //    }
        //    if (acc.proxy == String.Empty || acc.proxy == "" || acc.proxy == null)
        //    {
        //        Thread.Sleep(500);
        //        string response = string.Empty;
        //        for (int i = 0; i <= 5; i++)
        //        {
        //            int k = i + 1;
        //            L.LW("Отправляем запрос на сервер ВК(" + k.ToString() + ")");
        //            try
        //            {
        //                //Отправляем данные на сервер
        //                byte[] b = System.Text.Encoding.UTF8.GetBytes(parameters);

        //                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(request_path);
        //                myReq.Timeout = 15000;
        //                myReq.Method = "POST";
        //                myReq.ContentType = "application/x-www-form-urlencoded";
        //                myReq.ContentLength = b.Length;
        //                myReq.GetRequestStream().Write(b, 0, b.Length);

        //                HttpWebResponse Response = (HttpWebResponse)myReq.GetResponse();      //получение ответа от сервера API
        //                StreamReader myStream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
        //                response = myStream.ReadToEnd();
        //                Response.Close();

        //                return response;
        //            }
        //            catch
        //            {
        //                Thread.Sleep(1000);
        //            }
        //        }
        //        L.LW("Что-то пошло не так, VK не отвечает\n");
        //        return null;
        //    }
        //    else
        //    {
        //        Thread.Sleep(500);
        //        string response = string.Empty;
        //        for (int i = 0; i <= 5; i++)
        //        {
        //            int k = i + 1;
        //            L.LW("Отправляем запрос на сервер ВК(" + k.ToString() + ")");
        //            try
        //            {
        //                xNet.Net.HttpRequest request = new xNet.Net.HttpRequest();
        //                Proxy.MakeClient(request, acc.proxy);

        //                byte[] b = System.Text.Encoding.UTF8.GetBytes(parameters);
        //                string content = request.Post(request_path, parameters, "application/x-www-form-urlencoded").ToString();
        //                return content;
        //            }
        //            catch
        //            {
        //                Thread.Sleep(1000);
        //                Console.WriteLine("Вызвано исключение в методе Resp_POST");
        //            }
        //        }
        //        L.LW("Что-то пошло не так, VK не отвечает\n");
        //        return null;
        //    }
        //}

        //async public static Task<string> Resp_POSTAsync(string request_path, string parameters, Acc acc)
        //{
        //    if (acc == null)
        //    {
        //        return await Resp_POSTAsync(request_path, parameters);
        //    }
        //    if (acc.proxy == String.Empty || acc.proxy == "" || acc.proxy == null)
        //    {
        //        string response = string.Empty;
        //        for (int i = 0; i <= 5; i++)
        //        {
        //            int k = i + 1;
        //            L.LW("Отправляем запрос на сервер ВК(" + k.ToString() + ")");
        //            try
        //            {
        //                //Отправляем данные на сервер
        //                byte[] b = Encoding.UTF8.GetBytes(parameters);

        //                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(request_path);
        //                myReq.Timeout = 15000;
        //                myReq.Method = "POST";
        //                myReq.ContentType = "application/x-www-form-urlencoded";
        //                myReq.ContentLength = b.Length;
        //                myReq.GetRequestStream().Write(b, 0, b.Length);

        //                WebResponse Response = await myReq.GetResponseAsync();
        //                StreamReader myStream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
        //                response = myStream.ReadToEnd();
        //                Response.Close();

        //                return response;
        //            }
        //            catch
        //            {
        //                await Task.Delay(500);
        //            }
        //        }
        //        L.LW("Что-то пошло не так, VK не отвечает\n");
        //        return null;
        //    }
        //    else
        //    {
        //        string response = string.Empty;
        //        for (int i = 0; i <= 5; i++)
        //        {
        //            int k = i + 1;
        //            L.LW("Отправляем запрос на сервер ВК(" + k.ToString() + ")");
        //            try
        //            {
        //                HttpRequest request = new HttpRequest();
        //                Proxy.MakeClient(request, acc.proxy);

        //                byte[] b = Encoding.UTF8.GetBytes(parameters);
        //                string content = "";
        //                await Task.Run(() => content = request.Post(request_path, parameters, "application/x-www-form-urlencoded").ToString());
        //                //string content = request.Post(request_path, parameters, "application/x-www-form-urlencoded").ToString();
        //                if (content == "")
        //                {
        //                    await Task.Delay(500);
        //                    continue;
        //                }

        //                return content;
        //            }
        //            catch
        //            {
        //                await Task.Delay(500);
        //                Console.WriteLine("Вызвано исключение в методе Resp_POST");
        //            }
        //        }
        //        L.LW("Что-то пошло не так, VK не отвечает\n");
        //        return null;
        //    }
        //}

        //async public static Task<string> Resp_POSTAsync(string request_path, string parameters, string referrer = null)
        //{
        //    string response = string.Empty;
        //    for (int i = 0; i <= 5; i++)
        //    {
        //        int k = i + 1;
        //        L.LW("Отправляем запрос на сервер ВК(" + k.ToString() + ")");
        //        try
        //        {
        //            //Отправляем данные на сервер
        //            byte[] b = Encoding.UTF8.GetBytes(parameters);

        //            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(request_path);
        //            myReq.Timeout = 15000;
        //            myReq.Method = "POST";
        //            if (referrer != null) myReq.Referer = referrer;
        //            myReq.ContentType = "application/x-www-form-urlencoded";
        //            myReq.ContentLength = b.Length;
        //            myReq.GetRequestStream().Write(b, 0, b.Length);

        //            WebResponse Response = await myReq.GetResponseAsync();      //получение ответа от сервера API
        //            StreamReader myStream = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
        //            response = myStream.ReadToEnd();
        //            Response.Close();

        //            return response;
        //        }
        //        catch
        //        {
        //            await Task.Delay(500);
        //        }
        //    }
        //    L.LW("Что-то пошло не так, VK не отвечает\n");
        //    return null;
        //}
    }
}
