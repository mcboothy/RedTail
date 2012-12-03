using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AvalonEdit.Domain;

namespace AvalonEdit.Controls
{
    public class ProjectDirectoryViewModel : ProjectItemViewModel
    {
        public ProjectDirectoryViewModel(IProjectItem item, TreeViewItemViewModel parent) : base(item, parent)
        {
        }
    }
}
