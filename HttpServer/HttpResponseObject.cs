using Server.Enums;

namespace Server
{
    public class HttpResponseObject
    {
        public ResponseCode ResponseCode { get; set; } = ResponseCode.Ok;
        public string Server { get; set; } = string.Empty;
        public DateTime Date {  get; set; }
        public string Connection { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public object? Content { get; set; } = string.Empty;
    }
}
