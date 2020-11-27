namespace Injhinuity.Client.Core.Resources
{
    public static class ValidationResources
    {
        public const string LengthTag = "{length}";

        public const string ParseError = "Failed parsing validator resource, check the given type.";

        public const string NameEmpty = "Provided name field is empty.";
        public const string NameTooLong = "Provided name field is above maximum length (" + LengthTag + ").";

        public const string BodyEmpty = "Provided body field is empty.";
        public const string BodyTooLong = "Provided body field is above maximum length (" + LengthTag + ").";

        public const string EmoteEmpty = "Provided emote field is empty.";
        public const string EmoteInvalidFormat = "Provided emote field cannot be parsed.";
    }
}
