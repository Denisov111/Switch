﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Switch.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow :MetroWindow
    {
        public MainWindow(ViewModels.GlobalViewModel context)
        {
            InitializeComponent();
            DataContext = new
            {
                Context = context,
                Lang = context.Lang,
            };
        }

        
        async internal void onSendMessage(string message)
        {
            //await this.ShowMessageAsync("Ошибка", message);
            await this.ShowMessageAsync("", message);
        }
    }
}
