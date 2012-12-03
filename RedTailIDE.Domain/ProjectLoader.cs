using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AvalonEdit.Domain
{
    public static class ProjectLoader
    {
        public static Project LoadProject(string path)
        {
            return new Project
                       {
                           Name = "Test Project", Items = new List<IProjectItem>()
                                                              {
                                                                  new ProjectFile()
                                                                      {
                                                                          Name = "Testfile.c",
                                                                          Location = @"c:\dev\projects\testproject\TestFile.c"
                                                                      },
                                                                  new ProjectDirectory(new List<IProjectItem>( new List<IProjectItem>()
                                                                                         {
                                                                                             
                                                                                          new ProjectFile()
                                                                                              {
                                                                                                  Name = "Testfile.c",
                                                                                                  Location = @"c:\dev\projects\testproject\TestFile.c"
                                                                                              },
                                                                                          new ProjectDirectory(new List<IProjectItem>())
                                                                                              {
                                                                                                  Name = "TestDir",
                                                                                                  Location = @"c:\dev\projects\testproject\TestDir\",
                                                                                              }
                                                                                         }))
                                                                      {
                                                                          Name = "TestDir",
                                                                          Location = @"c:\dev\projects\testproject\TestDir\",
                                                                      }

                                                              }
                           
                           //Files = new ObservableCollection<ProjectFile>()
                           //            {
                           //                new ProjectFile { Name = "TestFile.c", Path = @"c:\dev\projects\testproject\TestFile.c" }
                           //            },
                           //Directories = new ObservableCollection<ProjectDirectory>()
                           //                  {
                           //                      new ProjectDirectory { Name = "TestDir", Path = @"c:\dev\projects\testproject\TestDir\"}
                           //                  }
                           
                       };
            //throw new NotImplementedException();

            // load the xml project
            // top level entries only
            // add each top level file and directory to respective lists
            // make sure to set project property to this
            // now the item can properly lazy load
        }

    }
}
