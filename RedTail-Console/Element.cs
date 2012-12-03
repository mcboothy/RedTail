using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedTail_Console
{
    public abstract class Element
    {
        public abstract void Accept(Visitor visitor);
    }

}
