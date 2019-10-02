using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Switch
{
    public class Lang : INotifyPropertyChanged
    {
        ResourceManager res_man;    // declare Resource manager to access to specific cultureinfo
        CultureInfo cul;

        string createProfileLang = "Создать профиль";
        string proxySettingsLang = "Настройки прокси";
        string addProxy = "Добавить прокси";
        string addProxyFromFile = "Добавить прокси из файла";
        string checkProxy = "Проверить все прокси";
        string delAllProxy = "Удалить все прокси";
        //string createProfileLang;
        //string proxySettingsLang;



        public string CreateProfileLang
        {
            get { return createProfileLang; }
            set
            {
                createProfileLang = value;
                OnPropertyChanged();
            }
        }

        public string ProxySettingsLang
        {
            get { return proxySettingsLang; }
            set
            {
                proxySettingsLang = value;
                OnPropertyChanged();
            }
        }

        public string AddProxy
        {
            get { return addProxy; }
            set
            {
                addProxy = value;
                OnPropertyChanged();
            }
        }

        public string AddProxyFromFile
        {
            get { return addProxyFromFile; }
            set
            {
                addProxyFromFile = value;
                OnPropertyChanged();
            }
        }

        public string CheckProxy
        {
            get { return checkProxy; }
            set
            {
                checkProxy = value;
                OnPropertyChanged();
            }
        }

        public string DelAllProxy
        {
            get { return delAllProxy; }
            set
            {
                delAllProxy = value;
                OnPropertyChanged();
            }
        }


        public Lang()
        {

            //var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = assembly.GetManifestResourceNames();
            //res_man = new ResourceManager(resourceName[0], Assembly.GetExecutingAssembly());
            //var res_man1 = new ResourceManager(resourceName[1], Assembly.GetExecutingAssembly());
            //var res_man2 = new ResourceManager(resourceName[2], Assembly.GetExecutingAssembly());
            //cul = CultureInfo.CreateSpecificCulture("en");
            //CreateProfileLang = res_man2.GetString("CreateProfileLang");

            //CreateProfileLang = res_man2.GetString("CreateProfileLang");
        }




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
