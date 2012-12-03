using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedTailLib;

namespace RedTail_Console
{
    public class FileElement : Element
    {
        public String Path;

        public String Filename
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(Path) ?? string.Empty; }
        }

        public override void Accept(Visitor visitor)
        {
            visitor.VisitCompileObjectFile(this);
        }
    }
}
