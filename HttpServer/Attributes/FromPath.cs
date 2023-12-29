namespace Server.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromPath : Attribute
    {
        public readonly string Key;

        public FromPath(string key)
        {
            Key = key;
        }
    }
}
