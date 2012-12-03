﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AvalonEdit.Domain
{
    public class ProjectDirectory : IProjectItem
    {
        protected readonly List<IProjectItem> _items;

        public ProjectDirectory(List<IProjectItem> items)
        {
            _items = items;
        }

        public ItemType ItemType
        {
            get { return ItemType.Directory; }
        }

        public string Name { get; set; }
        public string Location { get; set; }

        public IEnumerable<IProjectItem> Children { get { return _items; }}
    }
}
