using MonsterCardGame.Models.DB;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Utilities;
using Server;
using Server.Attributes;
using Server.Enums;
using Server.Factories;
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
        public object? Invoke([RawHttpRequest] HttpRequestObject reqObj)
        {
            string? authorizationValue;
            if (!reqObj.Headers.TryGetValue("Authorization", out authorizationValue))
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Unauthorized,
                    Content = "No 'Authorization' header provided",
                };

            string[] authorizationValueFragments = authorizationValue.Split(' ');
            if (authorizationValueFragments.Length < 2)
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Unauthorized,
                    Content = "Wrong 'Authorization' header format. Wanted: <schema> <token>",
                };

            if (authorizationValueFragments[0] != "Bearer")
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Unauthorized,
                    Content = "Wrong 'Authorization' header schema. Wanted: 'Bearer'",
                };

            try
            {
                if (!PJWToken.IsValid(authorizationValueFragments[1], Program.PJWT_SECRET))
                    return new ActionResult()
                    {
                        ResponseCode = ResponseCode.Unauthorized,
                        Content = "Token expired or manipulated",
                    };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{nameof(AuthMiddleware)}] Caught exception with validation: {ex.Message}");
                return new ActionResult()
                {
                    ResponseCode = ResponseCode.Unauthorized,
                    Content = "Token is formatted wrong",
                };
            }

            reqObj.SessionContent = PJWToken.GetContent<TokenContent>(authorizationValueFragments[1]);

            return null;
        }
    }
}
