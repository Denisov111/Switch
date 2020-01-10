using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulThings;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChromeModForNet;
using System.Drawing;

namespace Switch
{
    public class Persona : INotifyPropertyChanged
    {
        Proxy proxy;
        string hashString;

        public string Title { get; set; }
        public string Description { get; set; }
        public string ProfilePath { get; set; }
        public string UserAgent { get; set; }
        public Proxy Proxy
        {
            get { return proxy; }
            set
            {
                proxy = value;
                OnPropertyChanged();
            }
        }

        public ChromeMod ChromeInstance { get; set; }
        public string HashString
        {
            get { return hashString; }
            set
            {
                hashString = value;
                OnPropertyChanged();
            }
        }
        public Bitmap Avatar { get; set; }
        public string AvatarPath { get; set; }

        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion

        public override string ToString()
        {
            string proxy = Proxy == null ? "" : Proxy.ToString();
            return Title + " " + Description + " " + proxy;
        }
    }
}
