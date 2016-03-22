namespace Localization_Tool
{
    public class Translation
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Translation(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
