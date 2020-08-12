namespace Injhinuity.Client.Core.Configuration.Options
{
    public interface INullableOption
    {
        static string OptionName { get; }
        void ContainsNull(NullableOptionsResult result);
    }
}
