namespace Injhinuity.Client.Core.Configuration.Options
{
    public interface INullableOption
    {
        static string OptionName { get; } = "Option";
        void ContainsNull(NullableOptionsResult result);
    }
}
