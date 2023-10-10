using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IMiddleware
    {
        public HttpResponseObject? Invoke(HttpRequestObject reqObj);
    }
}
