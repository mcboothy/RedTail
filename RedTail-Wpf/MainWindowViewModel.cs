using System.Windows.Input;
using Microsoft.Win32;
using RedTail.WpfLib;

namespace RedTail_Wpf
{
    public class MainWindowViewModel
    {
        private RelayCommand _fileOpenCommand;

        public ICommand OpenFileCommand 
        {
            get { return GetFileOpenCommand(); }
        }

        public MainWindow View
        {
            get; set; 
        }

        private ICommand GetFileOpenCommand()
        {
            return _fileOpenCommand ?? (_fileOpenCommand = new RelayCommand((action) => HandleFileOpen()));
        }

        private void HandleFileOpen()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter =
                "C Files (*.c)|*.c|C++ Files (*.cpp,*.cc)|*.cpp;*.cc|Assembler Files (*.s)|*.s|All Files (*.*)|*.*";

            if(dlg.ShowDialog() ?? false)
            {
                // open a file and add it to handler
                EventAggregator.Publish(MainWindow.OpenDocumentRequest, dlg.FileName);
            }

        }
    }
}
