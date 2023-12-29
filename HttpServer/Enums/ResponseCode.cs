namespace Server.Enums
{
    public enum ResponseCode
    {
        Ok = 200,
        Created = 201,
        NoContent = 204,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        TimeOut = 408,
        Conflict = 409,
        ImATeapot = 418,
        InternalServerError = 500,
    }
}
