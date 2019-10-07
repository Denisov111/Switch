using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using UsefulThings;

namespace Switch.ViewModels
{
    public class ProxiesViewModel : INotifyPropertyChanged
    {
        private ProxyMod mod;




        public string ProfileLang
        {
            get { return mod.ProfileLang; }
            set
            {
                mod.ProfileLang = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Proxy> ProxiesColl
        {
            get { return mod.ProxiesColl; }
            set
            {
                mod.ProxiesColl = value;
                OnPropertyChanged();
            }
        }

        public ProxiesViewModel(ProxyMod mod)
        {
            this.mod = mod;
            OnSendCommand += mod.OnSendCommandHandler;
            OnSendCommandWithObject += mod.OnSendCommandWithObjectCommandHandler;
        }


        #region Commands

        public delegate void CommandHandler(string commandName);
        public event CommandHandler OnSendCommand;

        public delegate void CommandHandlerWithObject(object objectValue);
        public event CommandHandlerWithObject OnSendCommandWithObject;

        public RelayCommand UniversalCommand
        {
            get
            {
                RelayCommand rc = new RelayCommand(obj =>
                {
                    string commandName = obj.ToString();
                    OnSendCommand(commandName);
                });
                return rc;
            }
        }

        public RelayCommand ToBlackListUniversalCommand
        {
            get
            {
                RelayCommand rc = new RelayCommand(obj =>
                {
                    OnSendCommandWithObject(obj);
                });
                return rc;
            }
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
