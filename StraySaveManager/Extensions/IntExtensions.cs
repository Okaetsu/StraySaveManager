namespace ExtensionMethods
{
    public static class IntExtensions
    {
        public static bool IsWithinRange(this int num, int min, int max)
        {
            return num >= min && num <= max;
        }
    }
}