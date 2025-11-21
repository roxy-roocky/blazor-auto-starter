using System;

namespace utilities;

public static class StringExtensions
{
    public static string ToCapitalized(this string value) => char.ToUpper(value[0]) + value[1..];
}
