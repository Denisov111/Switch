using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Switch
{
    public class Global : INotifyPropertyChanged
    {
        #region MVVM


        #region Fields

        public string profileLang;

        #endregion


        #region Properties

        public string ProfileLang
        {
            get { return profileLang; }
            set
            {
                profileLang = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region fields

        public ViewModels.GlobalViewModel globalViewModel;
        public Views.MainWindow view;

        #endregion

        public void Run()
        {
            globalViewModel = new ViewModels.GlobalViewModel(this);
            view = new Views.MainWindow(globalViewModel);
            Console.WriteLine("HW");
            ProfileLang = "Создать профиль";
        }

        #region Commands

        internal void OnSendCommandHandler(string commandName)
        {
            switch (commandName)
            {
                case "Del":
                    Del();
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        internal void OnSendCommandWithObjectCommandHandler(object objectValue)
        {
            Console.WriteLine(objectValue);
        }

        private void Del()
        {
            ProfileLang = ProfileLang.Substring(0, ProfileLang.Length - 1);
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
