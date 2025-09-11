namespace UniversityEvents.Application.Extensions;

public static class NullableExtensions
{
    public static T GetValueOrDefault<T>(this T value)
    {
        return value == null ? default : value;
    }
}
