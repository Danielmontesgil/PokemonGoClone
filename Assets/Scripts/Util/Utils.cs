
public static class Utils
{
    public static string FirstCharToUpper(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}
