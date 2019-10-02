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

namespace Switch
{
    public class Global : INotifyPropertyChanged
    {
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
            Proxies = proxyMod.ProxiesColl;
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
