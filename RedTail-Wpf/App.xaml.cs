using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using RedTailIDE.Controls;

namespace RedTail_Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                var mainView = new MainWindow();
                mainView.Show();

                var vm = new MainWindowViewModel();
                mainView.DataContext = vm;

                vm.View = mainView;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
