using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedTail_Console.Domain
{
    public class BuildConfiguration
    {
        public String ToolPath
        {
            get; set;
        }

        public String LinkerScript
        {
            get;
            set;
        }

        public String Startup
        {
            get;
            set;
        }

        public String AssemblerTool
        {
            get;
            set;
        }

        public String LinkerTool
        {
            get;
            set;
        }

        public String LibraryArchiveTool
        {
            get;
            set;
        }

        public String CCompilerTool
        {
            get;
            set;
        }

        public String ElfFilename
        {
            get;
            set;
        }

        public String ObjectType
        {
            get;
            set;
        }

        public String ObjectCopyTool
        {
            get;
            set;
        }

        public String TargetName
        {
            get;
            set;
        }

        public String CFlags
        {
            get;
            set;
        }

        public Boolean Verbose
        {
            get; set;
        }
    }
}
