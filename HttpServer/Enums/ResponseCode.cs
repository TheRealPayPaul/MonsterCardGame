using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Enums
{
    public enum ResponseCode
    {
        Ok = 200,
        NotFound = 404,
        ImATeapot = 418,
        InternalServerError = 500,
    }
}
