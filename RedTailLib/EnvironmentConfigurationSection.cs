using System;
using System.Configuration;

namespace RedTailLib
{
    public class EnvironmentConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("verbose", DefaultValue = "false", IsRequired = true)]
        public Boolean Verbose
        {
            get { return (Boolean)this["verbose"]; }
            set { this["tool-path"] = value; }
        }

    }
}
