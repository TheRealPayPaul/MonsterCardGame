namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromRequest : Attribute
    {
        public readonly string Key;

        public FromRequest(string key)
        {
            Key = key;
        }
    }
}
