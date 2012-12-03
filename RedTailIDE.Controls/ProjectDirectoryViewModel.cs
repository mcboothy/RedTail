using RedTailIDE.Domain;

namespace RedTailIDE.Controls
{
    public class ProjectDirectoryViewModel : ProjectItemViewModel
    {
        public ProjectDirectoryViewModel(IProjectItem item, TreeViewItemViewModel parent) : base(item, parent)
        {
        }
    }
}
