namespace Injhinuity.Client.Core.Resources
{
    public static class ValidationResources
    {
        public const string LengthTag = "{length}";

        public const string ParseError = "Failed parsing validator resource, check the given type.";

        public const string CommandNameEmpty = "Provided command name is empty.";
        public const string CommandNameTooLong = "Provided command name is above maximum length (" + LengthTag + ").";

        public const string CommandBodyEmpty = "Provided command content is empty.";
        public const string CommandBodyTooLong = "Provided command name is above maximum length (" + LengthTag + ").";
    }
}
