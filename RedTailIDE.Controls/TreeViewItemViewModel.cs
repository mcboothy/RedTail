using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MyWpfLibrary;

namespace AvalonEdit.Controls
{
    public class TreeViewItemViewModel : ObservableObject, ITreeViewItemViewModel
    {
        protected bool _isExpanded;

        protected readonly TreeViewItemViewModel _parent;
        protected ObservableCollection<TreeViewItemViewModel> _children;

        private static readonly DummyNode DummyChild = new DummyNode();
        private bool _isSelected;

        public TreeViewItemViewModel() : this(null, false)
        {
        }

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;

            _children = new ObservableCollection<TreeViewItemViewModel>();

            if (lazyLoadChildren)
                _children.Add(DummyChild);
        }

        protected virtual void LoadChildren()
        {
        }

        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        public bool HasDummyChild
        {
            get { return Children.Count == 1 && Children[0] == DummyChild; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded;  }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (!HasDummyChild) return;

                Children.Remove(DummyChild);
                LoadChildren();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }
    }
}
