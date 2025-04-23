namespace Mocella.AsbHost
{
    public static class ASBConstants
    {
        //Regular Expression Patterns

        public const string RegexPattern_Email = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
        public const string RegexPattern_Phone = @"^(\+\d{1,4}[\s\-]*)?(\(\d{2,3}\)[\s\-]*)?(\d{2,4}[\s\-]*)*\d{3,4}[\s\-]*\d{3,4}$";

        //Common Field Lengths
        public const int EmailMaxLength = 256;
        public const int NameMinLength = 1;
        public const int NameMaxLength = 256;
        public const int NameExtendedMaxLength = 4000;
        public const int ShortNameMaxLength = 64;
        public const int JobTitleMaxLength = 64;
        public const int GuidStringLength = 36;
        public const int MaxImportSize = 1000;
        public const int DetailsMaxLength = 50;
        public const int PostalCodeMaxLength = 25;
        public const int PhoneNumberMaxLength = 25;
        public const int EntityIdMaxLength = 10;

        //Common Properties
        public static bool BeValidGuidOrEmpty(Guid? guid) => !guid.HasValue || guid.Value != Guid.Empty;
    }
}
