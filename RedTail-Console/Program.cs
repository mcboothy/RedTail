using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RedTailLib;

namespace RedTail_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run(args);
        }

        private BuildConfigurationSection _configSection;
        private EnvironmentConfigurationSection _environmentSection;
        //private readonly DomainMapper _domainMapper = new DomainMapper();

        private static void HandleError(string error)
        {
            // don't output null or empty strings
            if(!string.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine(error);
            }
        }

        private void HandleOutput(string output)
        {
            // check for verbose settings
            // if verbose then output, otherwise silent
            // don't output null or empty strings
            if(!string.IsNullOrWhiteSpace(output) && _environmentSection.Verbose)
            {
                Console.WriteLine(output);
            }
        }

        /// <summary>
        /// 0.  compile the startup file
        ///  a. create temp directory
        ///  b. compile startup file into temp directory
        /// 1. compile all the libraries
        ///  a. create temp directory in each library path
        ///  b. iterate each file and build with output being temp directory above
        ///  c. create library using library tool, output name should be library name and should go into library path
        ///
        /// 2. compile all the source files
        ///  a. create temp directory for build
        ///  b. iterate each file and build with output being temp directory created in previous step
        /// 3. link
        ///  a. include each file in directory created in 2a (compiled object files for each source file)
        ///  b. create library path using all library paths in 1c
        ///  c. add library switch with path created from 3b and each library file from step 1
        /// 4. final clean up
        ///  a. create elf file
        ///  b. create kernel.img file from elf file
        ///  c. delete each temp directory from 1a and 2a
        ///  d. delete elf file
        ///
        /// </summary>
        /// <param name="args"></param>
        public void Run(string[] args)
        {
            if( args.Length != 1 )
            {
                PrintUsage();
                return;
            }

            _configSection =
                   (BuildConfigurationSection)System.Configuration.ConfigurationManager.GetSection(
                   "buildPropertiesGroup/buildProperties");

            _environmentSection = (EnvironmentConfigurationSection)System.Configuration.ConfigurationManager.GetSection(
                   "environmentPropertiesGroup/environmentProperties");

            var projectFile = args[0];
            var project = ReadProjectFromFile(projectFile);
            if (project == null)
            {
                Console.WriteLine("Could not open  project file {0}", projectFile);
                return;
            }

            var startup = _configSection.Startup;
            var toolpath = _configSection.ToolPath + @"\bin";

            // set environment
            SetPath(Path.GetFullPath(toolpath));

            var outputPaths = new Dictionary<string, string>();
            var outputObjects = new List<CompiledObject>();
            var libraryPaths = new Dictionary<string, string>();

            Console.WriteLine("Building bootstrap files");
            if (!CompileProjectFile(libraryPaths, outputObjects, outputPaths, Path.GetFullPath(startup) )) return;

            // compile the libraries
            Console.WriteLine("Building libraries");
            if (project.Libraries.Any(lib => !BuildLibrary(outputPaths, libraryPaths, lib)))
            {
                return;
            }

            // compile each object
            Console.WriteLine("Compiling project files");
            if (project.Files.Any(file => !CompileProjectFile(libraryPaths, outputObjects, outputPaths, Path.Combine(project.Location, file.Path))))
            {
                return;
            }

            Console.WriteLine("Linking output files");
            LinkObjects(_configSection.LinkerTool, _configSection.LinkerScript, _configSection.ElfFilename,
                string.IsNullOrWhiteSpace(project.Options.MapFile) ? "" : Path.Combine(project.Location, project.Options.MapFile),
                outputObjects, 
                project.Libraries, 
                project.Location);

            Console.WriteLine("Creating target");
            CreateTarget(project.Location, project.Options.ListFile);

            Console.WriteLine("Cleaning up");
            CleanUp(outputPaths, project);
        }

        private void CleanUp(Dictionary<string, string> outputPaths, Project project)
        {
            File.Delete(Path.Combine(project.Location, _configSection.ElfFilename));

            foreach (var path in outputPaths.Keys)
            {
                Directory.Delete(path, true);
            }
        }

        private bool CompileProjectFile(IDictionary<string, string> libraryPaths, ICollection<CompiledObject> outputObjects, IDictionary<string, string> outputPaths, string file)
        {
            //var filePath = Path.Combine(project.Location, file.Path);
            var buildPath = Path.Combine(Path.GetDirectoryName(file) ?? string.Empty, "_build");

            if (!outputPaths.ContainsKey(buildPath))
            {
                outputPaths.Add(buildPath, buildPath);
            }

            Directory.CreateDirectory(buildPath);

            CompiledObject compiled;
            if (TryCompileFile(new InputFile {Path = file}, buildPath,
                               libraryPaths.Keys.Select(t => t + @"\include").ToList(), out compiled))
            {
                outputObjects.Add(compiled);
            }
            else
            {
                // abort
                Console.WriteLine("Failed to compile {0}", file);
                return false;
            }
            return true;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: RedTail <projectfile>");
        }

        private bool BuildLibrary(IDictionary<string, string> outputPaths, IDictionary<string, string> libraryPaths, Library lib)
        {
            Console.WriteLine("Building library {0}", lib.Name);

            libraryPaths.Add(Path.GetFullPath(lib.Location), Path.GetFullPath(lib.Location));

            var libraryObjects = new List<CompiledObject>();

            foreach (var file in lib.Files)
            {
                var filePath = Path.Combine(Path.GetFullPath(lib.Location), file.Path);
                var buildPath = Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, "build");

                if (!outputPaths.ContainsKey(buildPath))
                {
                    outputPaths.Add(buildPath, buildPath);
                }

                Directory.CreateDirectory(buildPath);

                CompiledObject compiled;
                if (TryCompileFile(new InputFile {Path = filePath}, buildPath, new List<string>(), out compiled))
                {
                    libraryObjects.Add(compiled);
                }
                else
                {
                    // abort
                    Console.WriteLine("Failed to compile library file {0}", file.Path);
                    return false;
                }
            }

            var switches = new List<ArgumentSwitch>();

            var libtarget = Path.Combine(Path.GetFullPath(lib.Location), lib.Name) + ".a";
            switches.Add(new ArgumentSwitch
                             {
                                 Identifier = "rv",
                                 SwitchType = "-",
                                 Value = libtarget
                             });

            switches.Add(new ArgumentSwitch {Identifier = "", SwitchType = "", Value = BuildFileList(libraryObjects)});

            // now build library with objects from above
            return ExecuteTool(_configSection.LibraryArchiveTool, BuildArgumentString(switches));
        }

        private void CreateTarget(string location, string listfile)
        {
            var switches = new List<ArgumentSwitch>
                               {
                                   new ArgumentSwitch
                                       {
                                           Identifier = "",
                                           SwitchType = "",
                                           Value = Path.Combine(location, _configSection.ElfFilename)
                                       },
                                   new ArgumentSwitch
                                       {Identifier = "O", SwitchType = "-", Value = _configSection.ObjectType},
                                   new ArgumentSwitch
                                       {
                                           Identifier = "",
                                           SwitchType = "",
                                           Value = Path.Combine(location, _configSection.TargetName)
                                       }
                               };

            ExecuteTool(_configSection.ObjectCopyTool, BuildArgumentString(switches));

            if(!string.IsNullOrWhiteSpace(listfile))
            {
                switches.Clear();
                switches.Add(new ArgumentSwitch
                                 {Identifier = "d", SwitchType = "-", Value = Path.Combine(location, _configSection.ElfFilename)});
                //switches.Add(new ArgumentSwitch
                //                 {Identifier = "", SwitchType = ">", Value = Path.Combine(location, listfile)});

                // here we have to do a little different because we need to capture the output stream
                // and put it into the listfile
                var startInfo = new ProcessStartInfo(_configSection.DumpTool, BuildArgumentString(switches))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };


                var p = Process.Start(startInfo);
                p.ErrorDataReceived += (sender, args) => HandleError(args.Data);

                var dataOut = new List<string>();
                p.OutputDataReceived += (sender, args) => dataOut.Add(args.Data);

                p.BeginErrorReadLine();
                p.BeginOutputReadLine();

                p.WaitForExit();

                //return p.ExitCode == 0;

                //ExecuteTool(_configSection.DumpTool, BuildArgumentString(switches));
                using (var fs = File.Open(Path.Combine(location, listfile), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (var streamWriter = new StreamWriter(fs))
                    {
                        foreach (var line in dataOut)
                        {
                            streamWriter.WriteLine(line);
                        }
                    }
                }
            }
    
        }

        private void LinkObjects(String linkerTool, String linkerScript, String outputFileName, String mapFileName, IEnumerable<CompiledObject> outputObjects,
                                    IEnumerable<Library> libraries, string outputPath)
        {
            var libraryString = string.Join(" ", libraries.Select(t => string.Format("{0}.a", Path.Combine(t.Location, t.Name))));

            var switches = new List<ArgumentSwitch>
                               {
                                   new ArgumentSwitch {Identifier = "no-undefined", SwitchType = "--", Value = ""},
                                   new ArgumentSwitch
                                       {Identifier = "", SwitchType = "", Value = BuildFileList(outputObjects)},
                                   new ArgumentSwitch
                                       {Identifier = "", SwitchType = "", Value = libraryString},
                                   new ArgumentSwitch
                                       {
                                           Identifier = "o",
                                           SwitchType = "-",
                                           Value = string.Format(@"{0}\{1}", outputPath, outputFileName)
                                       },
                                   new ArgumentSwitch
                                       {Identifier = "T", SwitchType = "-", Value = linkerScript}
                               };

            if(!string.IsNullOrWhiteSpace(mapFileName))
            {
                switches.Add(new ArgumentSwitch {Identifier = "Map", SwitchType = "-", Value = mapFileName});
            }

            var tool = linkerTool;
            var argumentString = BuildArgumentString(switches);

            ExecuteTool(tool, argumentString);
        }

        private static string BuildFileList(IEnumerable<CompiledObject> objects)
        {
            var fileListBuilder = new StringBuilder();
            foreach (var obj in objects)
            {
                fileListBuilder.Append(Path.Combine(obj.Path, obj.Name) + ".o ");
            }
            var fileList = fileListBuilder.ToString();
            return fileList;
        }

        private bool ExecuteTool(string tool, string argumentString)
        {
            var startInfo = new ProcessStartInfo(tool, argumentString)
                                {
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true
                                };


            var p = Process.Start(startInfo);
            p.ErrorDataReceived += (sender, args) => HandleError(args.Data);

            p.OutputDataReceived += (sender, args) => HandleOutput(args.Data);

            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            p.WaitForExit();

            return p.ExitCode == 0;
        }

        private static string BuildArgumentString(IEnumerable<ArgumentSwitch> switches)
        {
            var builder = new StringBuilder();
            foreach(var switch_ in switches)
            {
                builder.AppendFormat("{0}{1} {2} ", switch_.SwitchType, switch_.Identifier, switch_.Value);
            }

            return builder.ToString();
        }

        private static void SetPath(string toolpath)
        {
            var oldPath = Environment.GetEnvironmentVariable("PATH");

            var newPath = oldPath + (string.IsNullOrEmpty(oldPath)
                                         ? ""
                                         : ";" + toolpath);

            Environment.SetEnvironmentVariable("PATH", newPath);
        }

        private bool TryCompileFile(InputFile inputFile, string outputDirectory, IEnumerable<string> includeDirectories, out CompiledObject compiledObject)
        {
            Console.WriteLine("Compiling {0}", inputFile.Path);

            var arguments = "";

            var tool = "";
            switch (inputFile.FileType)
            {
                case InputFileType.CSource:
                    tool = _configSection.CCompilerTool;
                    arguments += _configSection.CFlags + " -c "; // "-Wall -O2 -nostdlib -nostartfiles -ffreestanding -c ";
                    if( _environmentSection.Verbose )
                    {
                        arguments += "-v ";
                    }

                    arguments += string.Join(" ", includeDirectories.Select(t => string.Format("-I {0} ", t)).ToArray());
                    break;
                case InputFileType.AsmSource:
                    if (_environmentSection.Verbose)
                    {
                        arguments += "--verbose ";
                    }
                    tool = _configSection.AssemblerTool;
                    break;
            }


            arguments += string.Format(@"{0} -o {1}.o", inputFile.Path, 
                Path.Combine(outputDirectory, inputFile.Filename));

            compiledObject = null;
            if(ExecuteTool(tool, arguments))
            {
                compiledObject = new CompiledObject {Name = inputFile.Filename, Path = outputDirectory};
                return true;
            }

            return false;
        }


        private Project ReadProjectFromFile(String fileName)
        {
            try
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return ReadProjectFromStream(stream);
                }
            }
            catch (Exception ex)
            {
                HandleError(ex.Message);
                Debug.Print(ex.ToString());
            }

            return null;
        }

        private static Project ReadProjectFromStream(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var reader = new XmlSerializer(typeof (Project));
                return (Project) reader.Deserialize(streamReader);
            }
        }
    }
}
