using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Switch.ViewModels
{
    public class GlobalViewModel : INotifyPropertyChanged
    {
        private Global global;



        public Lang Lang
        {
            get { return global.Lang; }
            set
            {
                global.Lang = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Persona> Persons
        {
            get { return global.Persons; }
            set
            {
                global.Persons = value;
                OnPropertyChanged();
            }
        }


        public GlobalViewModel(Global global)
        {
            this.global = global;
            OnSendCommand += global.OnSendCommandHandler;
            OnSendCommandWithObject += global.OnSendCommandWithObjectCommandHandler;
            OnSendOpenProfileCommand+=global.OnSendOpenProfileCommandHandler;
        }


        #region Commands

        public delegate void CommandHandler(string commandName);
        public event CommandHandler OnSendCommand;
        public event CommandHandler OnSendOpenProfileCommand;

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

        public RelayCommand OpenProfileCommand
        {
            get
            {
                RelayCommand rc = new RelayCommand(obj =>
                {
                    string path = obj.ToString();
                    OnSendOpenProfileCommand(path);
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
