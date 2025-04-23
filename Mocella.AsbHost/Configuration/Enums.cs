namespace Mocella.AsbHost.Configuration;

public enum RequestAction
{
    Add,
    Update,
    Delete
}

public static class EnumConverter
{
    public static bool TryConvertStringToEnum<TEnum>(string value, out TEnum result) where TEnum : struct, Enum
    {
        return Enum.TryParse(value, out result);
    }
}