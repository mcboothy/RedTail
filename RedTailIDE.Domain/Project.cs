using System.Collections.Generic;

namespace RedTailIDE.Domain
{
    public class Project : IProjectItem
    {
        public IEnumerable<IProjectItem> Items { get; set; }

        private readonly List<IProjectItem> _items = new List<IProjectItem>();

        public void AddItem(IProjectItem item)
        {
            _items.Add(item);    
        }

        public ItemType ItemType
        {
            get { return ItemType.Project; }
        }

        public string Name
        {
            get; set;
        }

        public string Location
        {
            get;
            set;
        }

        public IEnumerable<IProjectItem> Children
        {
            get { return _items; }
        }
    }
}
