namespace Core.Entities
{
    public class ConfigurationMetadata
    {
        public string Id { get; init; }
        public string Value { get; init; }

        public ConfigurationMetadata(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}