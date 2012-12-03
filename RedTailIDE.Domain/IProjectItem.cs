using System.Collections.Generic;

namespace AvalonEdit.Domain
{
    public enum ItemType
    {
        Project,
        File,
        Directory
    }

    public interface IProjectItem
    {
        ItemType ItemType { get; }
        string Name { get; set; }
        string Location { get; set; }

        IEnumerable<IProjectItem> Children { get; }
    }
}