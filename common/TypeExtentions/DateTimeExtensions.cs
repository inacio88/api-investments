namespace common.TypeExtentions
{
    public static class DateTimeExtensions
    {
        public static bool IsDateGreaterThan(this DateTime leftSide, DateTime rightSide)
        {
            return leftSide.Date > rightSide.Date;
        }

        public static bool IsDateLessThan(this DateTime leftSide, DateTime rightSide)
        {
            return leftSide.Date < rightSide.Date;
        }
    }
}