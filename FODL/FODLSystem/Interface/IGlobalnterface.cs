using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FODLSystem.Interface
{
    public interface IGlobalnterface
    {
        Task<string> NextNoSeries(string Module);
    }
}
