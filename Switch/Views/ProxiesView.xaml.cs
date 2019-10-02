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

namespace Switch
{
    /// <summary>
    /// Логика взаимодействия для ProxiesView.xaml
    /// </summary>
    public partial class ProxiesView : MetroWindow
    {
        ProxyMod proxyMod;

        public ProxiesView(Lang lang, ProxyMod proxyMod)
        {
            InitializeComponent();
            this.proxyMod = proxyMod;
            DataContext = new
            {
                Lang = lang,
                ProxyMod = proxyMod
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            proxyMod.AddProxyList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            proxyMod.AddProxyFromFile();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            proxyMod.CheckProxy();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            proxyMod.DelAllProxy();
        }

        async internal void onSendMessage(string message)
        {
            await this.ShowMessageAsync("Ошибка", message);
        }
    }
}
