using System;

namespace RedTailLib
{
    public enum InputFileType
    {
        CSource,
        CppSource,
        AsmSource
    }

    public class InputFile
    {
        public String Path;

        public String Filename
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(Path) ?? string.Empty; }
        }

        public InputFileType FileType
        {
            get
            {
                switch(System.IO.Path.GetExtension(Path))
                {
                    case ".s":
                        return InputFileType.AsmSource;

                    case ".c":
                        return InputFileType.CSource;

                    case ".cpp":
                        return InputFileType.CppSource;
                          
                    case ".cc":
                        return InputFileType.CppSource;

                    default:
                        return InputFileType.AsmSource;
                }
            }
        }
    }
}
