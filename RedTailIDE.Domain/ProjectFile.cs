using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonEdit.Domain
{
    public class ProjectFile : IProjectItem
    {
        public ItemType ItemType
        {
            get { return ItemType.File; }
        }

        public string Name
        {
            get; set; 
        }

        public string Location
        {
            get; set;
        }

        public IEnumerable<IProjectItem> Children
        {
            get { return new List<IProjectItem>(); }
        }
    }
}
