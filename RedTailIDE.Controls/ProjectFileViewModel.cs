using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AvalonEdit.Domain;

namespace AvalonEdit.Controls
{
    public class ProjectFileViewModel : ProjectItemViewModel
    {
        public ProjectFileViewModel(IProjectItem item, TreeViewItemViewModel parent) : base(item, parent)
        {
        }
    }
}
