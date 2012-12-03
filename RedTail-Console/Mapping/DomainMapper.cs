using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;

namespace RedTail_Console.Mapping
{
    public class DomainMapper : IMapper
    {
        static DomainMapper()
        {
            try
            {
                Mapper.Initialize(x =>
                {
                    var profileTypes = from a in AppDomain.CurrentDomain.GetAssemblies()
                                       from t in a.GetTypes()
                                       where
                                           t != typeof(Profile) && !t.IsAbstract &&
                                           typeof(Profile).IsAssignableFrom(t)
                                       select t;

                    foreach (var profileType in profileTypes)
                    {
                        try
                        {
                            var profile = (Profile)Activator.CreateInstance(profileType);

                            Debug.Print("Found AutoMapper configuration profile: " +
                                          profile.ProfileName);

                            x.AddProfile(profile);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(string.Format("Failed to load AutoMapper profile {0}: {1}",
                                profileType.Name,
                                ex), ex);

                            throw;
                        }
                    }
                });


                Mapper.AssertConfigurationIsValid();
            }
            catch (ReflectionTypeLoadException loadException)
            {
                var sb = new StringBuilder();

                foreach (var exSub in loadException.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);

                    if (exSub is FileNotFoundException)
                    {
                        var exFileNotFound = exSub as FileNotFoundException;

                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }

                    sb.AppendLine();
                }

                Debug.Print(sb + loadException.ToString());
                throw;

            }
            catch (Exception ex)
            {
                Debug.Print("Failed to validate AutoMapper configuration: " + ex);
                //throw;
            }                
        }

        public TDest Map<TDest>(object source)
        {
            return Mapper.Map<TDest>(source);
        }

        public TDest Map<TSrc, TDest>(TSrc source, TDest dest)
        {
            return Mapper.Map(source, dest);
        }
    }


}
