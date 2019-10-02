using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using UsefulThings;
using System.IO;
using System.Xml.Linq;

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
            if (isUseProxy) Persona.Proxy = Proxy;
            Persona.ProfilePath = Path.GetRandomFileName();
            global.Persons.Add(Persona);
            SavePersons();
            view.Close();
        }

        private void SavePersons()
        {
            string profilesFile = @"profiles.xml";
            XDocument doc = new XDocument();
            XElement ps = new XElement("profiles");
            doc.Add(ps);

            foreach (Persona pers in global.Persons)
            {
                XElement persona = new XElement("profile",
                                new XElement("title", pers.Title),
                                new XElement("description", pers.Description),
                                new XElement("profile_path", pers.ProfilePath),
                                new XElement("user_agent", pers.UserAgent));

                if(pers.Proxy!=null)
                {
                    XElement proxy = new XElement("proxy",
                                new XElement("ip", pers.Proxy.Ip),
                                new XElement("port", pers.Proxy.Port),
                                new XElement("login", pers.Proxy.Login),
                                new XElement("pwd", pers.Proxy.Pwd),
                                new XElement("protocol_type", pers.Proxy.ProxyProtocol.ToString()));
                    persona.Add(proxy);
                }
                doc.Root.Add(persona);
            }

            doc.Save(profilesFile);
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
