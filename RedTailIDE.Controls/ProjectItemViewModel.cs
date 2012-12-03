using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AvalonEdit.Domain;

namespace AvalonEdit.Controls
{
    public class ProjectItemViewModel : TreeViewItemViewModel
    {
        protected readonly IProjectItem _item;
        public ProjectItemViewModel(IProjectItem item, TreeViewItemViewModel parent)
            : base(parent, true)
        {
            _item = item;
        }

        public ItemType ItemType
        {
            get { return _item.ItemType; }
        }

        public string Name
        {
            get { return _item.Name; }
        }

        protected override void LoadChildren()
        {
            foreach (var item in _item.Children.OrderByDescending(p => p.ItemType))
            {
                if (item.ItemType == ItemType.File)
                    Children.Add(new ProjectFileViewModel(item, this));
                else
                    Children.Add(new ProjectDirectoryViewModel(item, this));
            }
        }
    }
}
