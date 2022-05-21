namespace Rewards.Helpers
{
    public static class DateTimeHelpers
    {
        public static DateTime GetStartOfWeek(this DateTime dateTime)
        {
            return dateTime.AddDays(-DaysToSubtractToSunday(dateTime)).Date;
        }

        private static int DaysToSubtractToSunday(DateTime dateTime)
        {
            return dateTime.DayOfWeek switch
            {
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                DayOfWeek.Saturday => 6,
                _ => 0
            };
        }
    }
}
