using Server.Attributes;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterCardGame.Middlewares
{
    [ApiMiddleware]
    internal class AuthMiddleware : IMiddleware
    {
        public void Invoke()
        {
            throw new NotImplementedException();
        }
    }
}
