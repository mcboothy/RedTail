using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AvalonEdit.Domain;

namespace AvalonEdit.Controls
{
    public class ProjectViewModel : TreeViewItemViewModel
    {
        private readonly Project _project;
        public ProjectViewModel(Project project) : base(null, false)
        {
            _project = project;

            //Children.Add(this);

            // sort items so directories bubble to top
            foreach (var item in _project.Items.OrderByDescending(p => p.ItemType))
            {
                if(item.ItemType == ItemType.File)
                    Children.Add(new ProjectFileViewModel(item, this));
                else
                    Children.Add(new ProjectDirectoryViewModel(item, this));
            }
        }

        public void RenameProject(string newName)
        {
            _project.Name = newName;
            OnPropertyChanged("ProjectName");
        }

        public string ProjectName { get { return _project.Name;  } }
        public string ProjectLocation { get { return _project.Location;  } }

        protected override void LoadChildren()
        {
            foreach(var item in _project.Items)
                Children.Add(new ProjectItemViewModel(item, this));
        }
    }
}
