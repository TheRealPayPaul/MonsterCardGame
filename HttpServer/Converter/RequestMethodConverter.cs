using Server.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Converter
{
    public static class RequestMethodConverter
    {
        public static RequestMethod StringToEnum(string requestMethod)
        {
            switch (requestMethod)
            {
                case "GET":
                    return RequestMethod.GET;
                case "POST":
                    return RequestMethod.POST;
                case "PUT":
                    return RequestMethod.PUT;
                case "DELETE":
                    return RequestMethod.DELETE;
                default:
                    throw new ArgumentException($"RequestMethodString {requestMethod} is not valid and thus can not be converted!");
            }
        }
    }
}
