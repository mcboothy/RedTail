using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AvalonEdit.Controls
{
    interface ITreeViewItemViewModel : INotifyPropertyChanged
    {
        ObservableCollection<TreeViewItemViewModel> Children { get; }
        bool HasDummyChild { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        TreeViewItemViewModel Parent { get; }
    }
}
