﻿using System;
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace Switch
{
    public enum ProfilerMode
    {
        Add,
        Edit
    }

    public class AddProfiler : INotifyPropertyChanged
    {
        private Global global;
        ViewModels.AddProfileViewModel addProfileViewModel;
        public Views.AddProfile view;

        #region MVVM


        #region Fields

        Persona persona;
        //bool isUseProxy;
        ObservableCollection<Proxy> proxies;
        Proxy proxy;
        BitmapImage avatar;
        string hashString;

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

        //public bool IsUseProxy
        //{
        //    get { return isUseProxy; }
        //    set
        //    {
        //        isUseProxy = value;
        //        OnPropertyChanged();
        //    }
        //}

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

        public BitmapImage Avatar
        {
            get {return avatar;}
            set
            {
                avatar = value;
                OnPropertyChanged();
            }
        }

        public string HashString
        {
            get { return hashString; }
            set
            {
                hashString = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        public ProfilerMode ProfilerMode;

        public AddProfiler(Global global)
        {
            persona = new Persona();

            this.global = global;
            addProfileViewModel = new ViewModels.AddProfileViewModel(this, global);
            UpdateAvatar();
            view = new Views.AddProfile(addProfileViewModel);
            Proxies = global.Proxies;
            view.ShowDialog();
        }

        public AddProfiler(Global global, Persona persona)
        {
            ProfilerMode = ProfilerMode.Edit;
            this.persona = persona;
            this.global = global;
            addProfileViewModel = new ViewModels.AddProfileViewModel(this, global);

            HashString = persona.HashString;
            Bitmap avaBitmap = AvatarGen.GenerateAvatar(HashString);
            Avatar = GetBitmapImage(avaBitmap);

            view = new Views.AddProfile(addProfileViewModel);
            Proxies = global.Proxies;

            //нужно сделать, чтобы если на акке был прокси, то его можно было удалить
            if(persona.Proxy!=null)
            {
                var pr = Proxies.Where(p => p.Ip == persona.Proxy.Ip && p.Port == persona.Proxy.Port).FirstOrDefault();
                if(pr!=null)
                    Proxy = pr;
                else
                    Proxy = persona.Proxy;
            }
           
            
            view.ShowDialog();
        }

        internal void OnSendCommandHandler(string commandName)
        {
            switch (commandName)
            {
                case "AddPerson":
                    AddPerson();
                    return;
                case "DelProxyFromProfile":
                    DelProxyFromProfile();
                    return;
                case "UpdateAvatar":
                    UpdateAvatar();
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        private void UpdateAvatar()
        {
            HashString = AvatarGen.GenerateHash();
            Bitmap avaBitmap = AvatarGen.GenerateAvatar(HashString);
            Avatar = GetBitmapImage(avaBitmap);
            
        }

        private BitmapImage GetBitmapImage(Bitmap avaBitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                avaBitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void DelProxyFromProfile()
        {
            Proxy = null;
        }

        private void AddPerson()
        {
            if(ProfilerMode == ProfilerMode.Add)
            {
                Persona.Proxy = Proxy;
                Persona.ProfilePath = Path.GetRandomFileName();
                Persona.HashString = HashString;
                global.Persons.Add(Persona);
                SavePersons();
                view.Close();
            }

            if (ProfilerMode == ProfilerMode.Edit)
            {
                Persona.Proxy = Proxy;
                SavePersons();
                view.Close();
            }
            
        }

        public void SavePersons()
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
                                new XElement("user_agent", pers.UserAgent),
                                new XElement("hex", pers.HashString));

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
