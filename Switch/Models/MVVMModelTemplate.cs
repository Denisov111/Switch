﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Switch
{
    public class MVVMModelTemplate : INotifyPropertyChanged
    {
        #region MVVM


        #region Fields

        public string templateVar;

        #endregion


        #region Properties

        public string TemplateVar
        {
            get { return templateVar; }
            set
            {
                templateVar = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region fields

        public ViewModels.MVVMModelTemplateViewModel mVVMModelTemplateViewModel;
        public Views.MVVMModelTemplateView view;

        #endregion

        public void Run()
        {
            mVVMModelTemplateViewModel = new ViewModels.MVVMModelTemplateViewModel(this);
            view = new Views.MVVMModelTemplateView(mVVMModelTemplateViewModel);
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
                default:
                    throw new NotImplementedException();
            }
        }

        private void AddProfile()
        {
            throw new NotImplementedException();
        }

        internal void OnSendCommandWithObjectCommandHandler(object objectValue)
        {
            Console.WriteLine(objectValue);
        }

        private void Del()
        {
            TemplateVar = TemplateVar.Substring(0, TemplateVar.Length - 1);
        }

        #endregion

        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            mVVMModelTemplateViewModel.OnPropertyChanged(prop);
        }

        #endregion
    }
}
