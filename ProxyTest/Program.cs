using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings;
using System.Threading;
namespace ProxyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Go();
            Thread.Sleep(30000);
        }

        async private static Task Go()
        {
            Proxy pr = new Proxy();
            pr.Ip = "45.133.32.223";
            pr.Port = "8000";
            //pr.Login = "jur3VF";
            //pr.Pwd = "WX1aWG";
            pr.ProxyProtocol = ProxyProtocol.SOCKS5;
            string resp = await Net.RespAsync("http://mybot.su/pro", pr);
        }
    }
}
