using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Switch
{
    public class PersonaEditor : INotifyPropertyChanged
    {
        #region MVVM


        #region Fields

        public Persona persona;

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

        #endregion

        #endregion

        #region fields

        public ViewModels.PersonaEditorViewModel personaEditorViewModel;
        public Views.PersonaEditorView view;

        #endregion

        public void Run(Persona persona)
        {
            personaEditorViewModel = new ViewModels.PersonaEditorViewModel(this);
            view = new Views.PersonaEditorView(personaEditorViewModel);
            this.Persona = persona;
            view.ShowDialog();
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
            
        }

        #endregion

        #region INotifyPropertyChanged code

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            personaEditorViewModel.OnPropertyChanged(prop);
        }

        #endregion

    }
}
