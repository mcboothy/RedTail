using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AvalonDock.Layout;
using AvalonEdit.Controls;
using AvalonEdit.Domain;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var project = ProjectLoader.LoadProject("project path here");
            var vm = new ProjectViewModel(project);
            projectView.DataContext = vm;

            // TODO: Load from resource or from web even, save cached copy so if user doesn't connect you have something to display
            startPage.webBrowser.NavigateToString("<HTML><H2><B>This page comes using String</B><P></P></H2>");
        }

        private void dockManager_DocumentClosing(object sender, AvalonDock.DocumentClosingEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                var doc = new LayoutDocument {Title = "Test1"};
                doc.Content = new EditView();

                firstDocumentPane.Children.Add(doc);
            }

        }
    }
}
