namespace NRustLightning.Server.Tests.Api
{
    public class Data
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public Data(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public dynamic DynamicValue => Value;
    }
}