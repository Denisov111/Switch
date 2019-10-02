using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeModForNet
{
    enum BrowserType
    {
        ChromiumFX,
        Chromium,
        HeadlessChromium
    }

    class Settings
    {
        public static string AplicationPath { get { return System.Windows.Forms.Application.StartupPath; } }
        public static string UseragentOverride { get { return "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36"; } }
        public static string AcceptLanguages { get { return "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3"; } }
        public static int MinDebugPort = 9222;
        public static int MaxDebugPort = 59999;
        public static int ViewPortWidth = 800;
        public static int ViewPortHeight = 600;
        public static BrowserType BrType { get; set; } = BrowserType.ChromiumFX;
    }
}
