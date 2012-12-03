using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace RedTailIDE
{
    /// <summary>
    /// Interaction logic for EditControl.xaml
    /// </summary>
    public partial class EditView : UserControl, INotifyPropertyChanged
    {
        public EditView()
        {
            InitializeComponent();

            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (var s = typeof(EditView).Assembly.GetManifestResourceStream("RedTailIDE.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");

                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }

                textEditor.TextArea.TextEntering += TextEditorTextAreaTextEntering;
                textEditor.TextArea.TextEntered += TextEditorTextAreaTextEntered;

                var foldingUpdateTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
                foldingUpdateTimer.Tick += FoldingUpdateTimerTick;
                foldingUpdateTimer.Start();

            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new [] { ".cool" }, customHighlighting);
        }

        public string CurrentFileName
        {
            get { return _currentFileName;  }
            set { _currentFileName = value; OnPropertyChanged("CurrentFileName"); }
        }

        private string _currentFileName;

        void OpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {CheckFileExists = true};
            if (!(dlg.ShowDialog() ?? false)) return;

            CurrentFileName = dlg.FileName;
            textEditor.Load(CurrentFileName);
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(CurrentFileName));
        }

        void SaveFileButtonClick(object sender, EventArgs e)
        {
            if (CurrentFileName == null)
            {
                var dlg = new SaveFileDialog {DefaultExt = ".c"};
                if (dlg.ShowDialog() ?? false)
                {
                    CurrentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }

            textEditor.Save(CurrentFileName);
        }

        CompletionWindow completionWindow;

        void TextEditorTextAreaTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != ".") return;
            // open code completion after the user has pressed dot:
            completionWindow = new CompletionWindow(textEditor.TextArea);
            // provide AvalonEdit with the data:
            var data = completionWindow.CompletionList.CompletionData;
            data.Add(new MyCompletionData("Item1"));
            data.Add(new MyCompletionData("Item2"));
            data.Add(new MyCompletionData("Item3"));
            data.Add(new MyCompletionData("Another item"));
            completionWindow.Show();
            completionWindow.Closed += delegate
                                           {
                                               completionWindow = null;
                                           };
        }

        void TextEditorTextAreaTextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length <= 0 || completionWindow == null) return;
            if (!char.IsLetterOrDigit(e.Text[0]))
            {
                // Whenever a non-letter is typed while the completion window is open,
                // insert the currently selected element.
                completionWindow.CompletionList.RequestInsertion(e);
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        #region Folding
        FoldingManager _foldingManager;
        AbstractFoldingStrategy _foldingStrategy;

        void HighlightingComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                _foldingStrategy = null;
            }
            else
            {
                switch (textEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        _foldingStrategy = new XmlFoldingStrategy();
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        _foldingStrategy = new RegionFoldingStrategy();
                        break;
                    case "C++":
                    case "PHP":
                    case "Java":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        _foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        _foldingStrategy = null;
                        break;
                }
            }
            if (_foldingStrategy != null)
            {
                if (_foldingManager == null)
                    _foldingManager = FoldingManager.Install(textEditor.TextArea);
                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
            else
            {
                if (_foldingManager != null)
                {
                    FoldingManager.Uninstall(_foldingManager);
                    _foldingManager = null;
                }
            }
        }

        void FoldingUpdateTimerTick(object sender, EventArgs e)
        {
            if (_foldingStrategy != null)
            {
                _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            var func = PropertyChanged;
            if (func != null)
            {
                func(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
