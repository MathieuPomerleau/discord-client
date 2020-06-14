namespace Injhinuity.Client.Core.Configuration.Options
{
    public class VersionOptions : INullableOption
    {
        public string? VersionNo { get; set; }

        public bool ContainsNull() =>
            VersionNo is null;
    }
}
