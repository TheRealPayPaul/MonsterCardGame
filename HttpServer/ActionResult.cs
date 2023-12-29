using Server.Enums;

namespace Server
{
    public class ActionResult
    {
        public ResponseCode ResponseCode { get; set; } = ResponseCode.Ok;
        public object? Content { get; set; } = null;
    }
}
