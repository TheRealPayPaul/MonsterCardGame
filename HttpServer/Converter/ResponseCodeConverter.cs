using Server.Enums;


namespace Server.Converter
{
    internal static class ResponseCodeConverter
    {

        public static string EnumToString(ResponseCode code)
        {
            switch (code)
            {
                case ResponseCode.Ok:
                    return "OK";
                case ResponseCode.BadRequest:
                    return "BAD REQUEST";
                case ResponseCode.NotFound:
                    return "NOT FOUND";
                case ResponseCode.ImATeapot:
                    return "I'M A TEAPOT";
                case ResponseCode.InternalServerError:
                    return "INTERNAL SERVER ERROR";
                default:
                    return "Undefined";
            }
        }
    }
}
