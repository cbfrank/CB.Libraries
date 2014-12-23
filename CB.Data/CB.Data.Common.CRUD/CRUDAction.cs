using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CB.Data.Common.CRUD
{
    [Flags]
    public enum CRUDAction
    {
        Query = 0x1,
        Create = 0x2,
        Update = 0x4,
        Delete = 0x8
    }
}
