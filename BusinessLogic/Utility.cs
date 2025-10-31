namespace BusinessLogic;

public static class Utility
{
    public static string Normalize(string? value) => (value ?? string.Empty).Trim();
}