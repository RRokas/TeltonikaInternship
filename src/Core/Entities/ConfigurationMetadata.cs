namespace Core.Entities
{
    public class ConfigurationMetadata
    {
        public string Id { get; }
        public string Value { get; }

        public ConfigurationMetadata(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}