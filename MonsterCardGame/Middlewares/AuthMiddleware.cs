using System.Text.RegularExpressions;
using MonsterCardGame.Models.PJWT;
using MonsterCardGame.Utilities;
using Server;
using Server.Attributes;
using Server.Enums;
using Server.Interfaces;

namespace MonsterCardGame.Middlewares
{
    [ApiMiddleware]
    internal class AuthMiddleware : IMiddleware
    {
        private readonly PJWToken _pjwtoken;

        public AuthMiddleware()
        {
            _pjwtoken = new PJWToken();
        }

        public AuthMiddleware(PJWToken pjwtoken)
        {
            _pjwtoken = pjwtoken;
        }

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

            string unicodeEscapedAuthValue = Regex.Unescape(authorizationValueFragments[1]);
            try
            {
                if (!_pjwtoken.IsValid(unicodeEscapedAuthValue, Program.PJWT_SECRET))
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

            reqObj.SessionContent = _pjwtoken.GetContent<TokenContent>(unicodeEscapedAuthValue);

            return null;
        }
    }
}
