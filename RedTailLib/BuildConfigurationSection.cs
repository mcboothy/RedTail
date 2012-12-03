using System;
using System.Configuration;

namespace RedTailLib
{
    public class BuildConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("tool-path", DefaultValue = "", IsRequired = true)]
        public String ToolPath
        {
            get { return (String) this["tool-path"]; }
            set { this["tool-path"] = value;  }
        }

        [ConfigurationProperty("linker-script", DefaultValue = "", IsRequired = true)]
        public String LinkerScript
        {
            get { return (String)this["linker-script"]; }
            set { this["linker-script"] = value; }
        }

        [ConfigurationProperty("startup", DefaultValue = "", IsRequired = true)]
        public String Startup
        {
            get { return (String) this["startup"]; }
            set { this["startup"] = value; }
        }

        [ConfigurationProperty("assembler-tool", DefaultValue = "", IsRequired = true)]
        public String AssemblerTool
        {
            get { return (String) this["assembler-tool"]; }
            set { this["assembler-tool"] = value; }
        }
        [ConfigurationProperty("linker-tool", DefaultValue = "", IsRequired = true)]
        public String LinkerTool
        {
            get { return (String) this["linker-tool"]; }
            set { this["linker-tool"] = value; }
        }
        [ConfigurationProperty("library-archive-tool", DefaultValue = "", IsRequired = true)]
        public String LibraryArchiveTool
        {
            get { return (String) this["library-archive-tool"]; }
            set { this["library-archive-tool"] = value; }
        }
        [ConfigurationProperty("c-compiler-tool", DefaultValue = "", IsRequired = true)]
        public String CCompilerTool
        {
            get { return (String)this["c-compiler-tool"]; }
            set { this["c-compiler-tool"] = value; }
        }
        [ConfigurationProperty("elf-filename", DefaultValue = "", IsRequired = true)]
        public String ElfFilename
        {
            get { return (String)this["elf-filename"]; }
            set { this["elf-filename"] = value; }
        }

        [ConfigurationProperty("object-type", DefaultValue = "", IsRequired = true)]
        public String ObjectType
        {
            get { return (String) this["object-type"]; }
            set { this["object-type"] = value; }
        }

        [ConfigurationProperty("object-copy-tool", DefaultValue = "", IsRequired = true)]
        public String ObjectCopyTool
        {
            get { return (String) this["object-copy-tool"]; }
            set { this["object-copy-tool"] = value; }
        }

        [ConfigurationProperty("target-name", DefaultValue = "", IsRequired = true)]
        public String TargetName
        {
            get { return (String)this["target-name"]; }
            set { this["target-name"] = value; }
        }


        [ConfigurationProperty("c-flags", DefaultValue = "", IsRequired = true)]
        public String CFlags
        {
            get { return (String)this["c-flags"]; }
            set { this["c-flags"] = value; }
        }

        [ConfigurationProperty("verbose", DefaultValue = "False", IsRequired = false)]
        public Boolean Verbose
        {
            get { return (Boolean)this["verbose"]; }
            set { this["verbose"] = value; }
        }

        [ConfigurationProperty("dump-tool", DefaultValue = "", IsRequired = false)]
        public String DumpTool
        {
            get { return (String)this["dump-tool"]; }
            set { this["dump-tool"] = value; }
        }
    }
}
