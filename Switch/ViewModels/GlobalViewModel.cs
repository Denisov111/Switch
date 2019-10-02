﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Switch.ViewModels
{
    public class GlobalViewModel : INotifyPropertyChanged
    {
        private Global global;



        /*
        public string ProfileLang
        {
            get { return global.ProfileLang; }
            set
            {
                global.ProfileLang = value;
                OnPropertyChanged();
            }
        }

        public string ProxyLang
        {
            get { return global.ProxyLang; }
            set
            {
                global.ProxyLang = value;
                OnPropertyChanged();
            }
        }*/

        public Lang Lang
        {
            get { return global.Lang; }
            set
            {
                global.Lang = value;
                OnPropertyChanged();
            }
        }


        public GlobalViewModel(Global global)
        {
            this.global = global;
            OnSendCommand += global.OnSendCommandHandler;
            OnSendCommandWithObject += global.OnSendCommandWithObjectCommandHandler;
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
