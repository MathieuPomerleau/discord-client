namespace Injhinuity.Client.Discord.Embeds
{
    public class InjhinuityEmbedField
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool Inline { get; set; }

        public InjhinuityEmbedField(string name, object value, bool inline = false)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }
    }
}
