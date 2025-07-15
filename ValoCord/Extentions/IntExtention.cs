namespace ValoCord.Extentions;

public static class IntExtensions
{
    public static string ToOrdinal(this int number)
    {
        switch (number)
        {
            case 1: return $"{number}st";
            case 2: return $"{number}nd";
            case 3: return $"{number}rd";
            default: return $"{number}th";
        }
    }
}