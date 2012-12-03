using RedTailIDE.Domain;

namespace RedTailIDE.Controls
{
    public class ProjectFileViewModel : ProjectItemViewModel
    {
        public ProjectFileViewModel(IProjectItem item, TreeViewItemViewModel parent) : base(item, parent)
        {
        }
    }
}
