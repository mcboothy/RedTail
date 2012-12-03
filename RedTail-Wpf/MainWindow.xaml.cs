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
using RedTail.WpfLib;

namespace RedTail_Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int OpenDocumentRequest = 1;

        public MainWindow()
        {
            InitializeComponent();

            EventAggregator.Subscribe(OpenDocumentRequest, OpenDocumentRequestHandler);
        }

        private void OpenDocumentRequestHandler(object data)
        {
            var filename = data.ToString();

            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane == null) return;

            var doc = new LayoutDocument { Title = filename };

            var ev = new EditView();
            ev.OpenFile(filename);
            doc.Content = ev;

            firstDocumentPane.Children.Add(doc);
        }
    }
}
