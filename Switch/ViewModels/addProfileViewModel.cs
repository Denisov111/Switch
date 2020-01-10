using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using UsefulThings;
using System.Windows.Media.Imaging;

namespace Switch.ViewModels
{
    public class AddProfileViewModel : INotifyPropertyChanged
    {
        private AddProfiler addProfiler;
        Global global;


        public Persona Persona
        {
            get { return addProfiler.Persona; }
            set
            {
                addProfiler.Persona = value;
                OnPropertyChanged();
            }
        }

        //public bool IsUseProxy
        //{
        //    get { return addProfiler.IsUseProxy; }
        //    set
        //    {
        //        addProfiler.IsUseProxy = value;
        //        OnPropertyChanged();
        //    }
        //}

        public ObservableCollection<Proxy> Proxies
        {
            get { return addProfiler.Proxies; }
            set
            {
                addProfiler.Proxies = value;
                OnPropertyChanged();
            }
        }

        public Proxy Proxy
        {
            get { return addProfiler.Proxy; }
            set
            {
                addProfiler.Proxy = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage Avatar
        {
            get { return addProfiler.Avatar; }
            set
            {
                addProfiler.Avatar = value;
                OnPropertyChanged();
            }
        }


        public AddProfileViewModel(AddProfiler addProfiler, Global global)
        {
            this.addProfiler = addProfiler;
            this.global = global;
            OnSendCommand += addProfiler.OnSendCommandHandler;
            OnSendCommandWithObject += addProfiler.OnSendCommandWithObjectCommandHandler;
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

        //public RelayCommand ToBlackListUniversalCommand
        //{
        //    get
        //    {
        //        RelayCommand rc = new RelayCommand(obj =>
        //        {
        //            OnSendCommandWithObject(obj);
        //        });
        //        return rc;
        //    }
        //}

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
