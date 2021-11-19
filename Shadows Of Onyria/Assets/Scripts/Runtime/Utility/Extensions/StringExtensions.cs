public static class StringExtensions
{
    public static bool FastEndsWith(this string a, string b)
    {
        var ap = a.Length - 1;
        var bp = b.Length - 1;
    
        while (ap >= 0 && bp >= 0 && a [ap] == b [bp])
        {
            ap--;
            bp--;
        }
    
        return (bp < 0);
    }

    public static bool FastStartsWith(this string a, string b)
    {
        var aLen = a.Length;
        var bLen = b.Length;
    
        var ap = 0; 
        var bp = 0;
    
        while (ap < aLen && bp < bLen && a [ap] == b [bp])
        {
            ap++;
            bp++;
        }
    
        return (bp == bLen);
    }

    public static bool FirstCharacterIs(this string a, char b)
    {
        return a[0] == b;
    }
    
    public static bool LastCharacterIs(this string a, char b)
    {
        return a[a.Length - 1] == b;
    }
}
