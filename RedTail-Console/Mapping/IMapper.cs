using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedTail_Console.Mapping
{
    public interface IMapper
    {
        TDest Map<TDest>(object source);
        TDest Map<TSrc, TDest>(TSrc source, TDest dest);
    }
}
