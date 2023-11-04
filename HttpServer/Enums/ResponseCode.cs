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
        NoContent = 204,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        ImATeapot = 418,
        InternalServerError = 500,
    }
}
