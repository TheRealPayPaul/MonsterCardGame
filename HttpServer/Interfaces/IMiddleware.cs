using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Interfaces
{
    public interface IMiddleware
    {
        // TODO should return a response or null
        public void Invoke();
    }
}
