using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using UsefulThings;

namespace Switch
{
    public class AddProfiler : INotifyPropertyChanged
    {
        private Global global;
        ViewModels.AddProfileViewModel addProfileViewModel;
        public Views.AddProfile view;

        #region MVVM


        #region Fields

        Persona persona;
        bool isUseProxy;
        ObservableCollection<Proxy> proxies;
        Proxy proxy;

        #endregion


        #region Properties

        public Persona Persona
        {
            get { return persona; }
            set
            {
                persona = value;
                OnPropertyChanged();
            }
        }

        public bool IsUseProxy
        {
            get { return isUseProxy; }
            set
            {
                isUseProxy = value;
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

        public Proxy Proxy
        {
            get { return proxy; }
            set
            {
                proxy = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion


        public AddProfiler(Global global)
        {
            persona = new Persona();
            
            this.global = global;
            addProfileViewModel = new ViewModels.AddProfileViewModel(this, global);
            view = new Views.AddProfile(addProfileViewModel);
            Proxies = global.Proxies;
            view.ShowDialog();
        }

        internal static void Add(Global global)
        {
            
            //Views.AddProfile f = new Views.AddProfile();
            //f.ShowDialog();
            //Persona persona=f.persona;
        }

        internal void OnSendCommandHandler(string commandName)
        {
            switch (commandName)
            {
                case "AddPerson":
                    AddPerson();
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AddPerson()
        {
            if(isUseProxy) Persona.Proxy = Proxy;
            global.Persons.Add(Persona);
        }

        internal void OnSendCommandWithObjectCommandHandler(object objectValue)
        {
            switch (objectValue)
            {
                case "Del":
                    throw new NotImplementedException();
                    return;
                case "AddProfile":
                    throw new NotImplementedException();
                    return;
                case "ProxySettings":
                    throw new NotImplementedException();
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            addProfileViewModel.OnPropertyChanged(prop);
        }

        #endregion
    }
}
