namespace PSK.Server.Misc
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MetadataAttribute : Attribute
    {
        public string Key { get; }
        public string Value { get; }

        public MetadataAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

}
