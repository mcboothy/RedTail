using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RedTailLib;
using RedTail_Console.Domain;

namespace RedTail_Console
{
    public class RedTailBuildVisitor : Visitor
    {
        private readonly BuildConfiguration _configuration;

        public RedTailBuildVisitor(BuildConfiguration configuration)
        {
            _configuration = configuration;
        }

        public enum Status
        {
            Success,
            Failure
        }

        public Status LastStatus { get; set; }

        public bool ErrorOccured { get; set; }

        public override void VisitCompileObjectFile(FileElement element)
        {

            // TODO: compile the object file here
            // the object file element will have the filepath
            // and the output path

            var fileType = InputFileType.AsmSource;
            switch (System.IO.Path.GetExtension(element.Path))
            {
                case ".s":
                    fileType = InputFileType.AsmSource;
                    break;

                case ".c":
                    fileType = InputFileType.CSource;
                    break;

                case ".cpp":
                    fileType = InputFileType.CppSource;
                    break;

                case ".cc":
                    fileType = InputFileType.CppSource;
                    break;
            }


            //throw new NotImplementedException();

            // on error should we throw an exception?

            // I think so, a custom exception called CompileFileException
        }

        private bool TryCompileFile(string filename, string path, InputFileType fileType, string outputDirectory, IEnumerable<string> includeDirectories, out CompiledObject compiledObject)
        {
            Console.WriteLine("Compiling {0}", path);

            var arguments = "";

            //InputFile file = new InputFile {Path = filePath};

            // figure out tool based on extension
            //var extension = (Path.GetExtension(in) ?? string.Empty).ToLower();
            var tool = "";
            switch (fileType)
            {
                case InputFileType.CSource:
                    tool = _configuration.CCompilerTool;
                    arguments += _configuration.CFlags + " -c "; // "-Wall -O2 -nostdlib -nostartfiles -ffreestanding -c ";
                    if (_configuration.Verbose)
                    {
                        arguments += "-v ";
                    }

                    arguments += string.Join(" ", includeDirectories.Select(t => string.Format("-I {0} ", t)).ToArray());
                    break;
                case InputFileType.AsmSource:
                    if (_configuration.Verbose)
                    {
                        arguments += "--verbose ";
                    }
                    tool = _configuration.AssemblerTool;
                    break;
            }


            arguments += string.Format(@"{0} -o {1}.o", path,
                Path.Combine(outputDirectory, filename));

            compiledObject = null;
            //if (ExecuteTool(tool, arguments))
            //{
            //    compiledObject = new CompiledObject { Name = filename, Path = outputDirectory };
            //    return true;
            //}

            return false;
        }
    }
}
