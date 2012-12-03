using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using RedTailLib;
using RedTail_Console.Domain;

namespace RedTail_Console.Mapping
{
    public class ConfigurationToDomainObject : Profile
    {
        public override string ProfileName
        {
            get { return GetType().FullName; }
        }

        protected override void Configure()
        {
            CreateMap<BuildConfigurationSection, BuildConfiguration>();
        }
    }
}
