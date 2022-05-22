using System;
using System.Collections.Generic;
using Xunit;

namespace Rewards.Helpers.Test
{
    public class DateTimeHelpersTests
    {
        [Theory, MemberData(nameof(TestGetStartOfWeekData))]
        public void TestGetStartOfWeek(DateTime dateTime, DateTime startOfWeek)
        {
            var result = DateTimeHelpers.GetStartOfWeek(dateTime);
            Assert.Equal(startOfWeek, result);
        }

        public static IEnumerable<object[]> TestGetStartOfWeekData => new List<object[]>
        {
            new object[] { new DateTime(2022, 5, 22), new DateTime(2022, 5, 22) }, // Sunday
            new object[] { new DateTime(2022, 5, 23), new DateTime(2022, 5, 22) }, // Monday
            new object[] { new DateTime(2022, 5, 24), new DateTime(2022, 5, 22) }, // Tuesday
            new object[] { new DateTime(2022, 5, 25), new DateTime(2022, 5, 22) }, // Wednesday
            new object[] { new DateTime(2022, 5, 26), new DateTime(2022, 5, 22) }, // Thursday
            new object[] { new DateTime(2022, 5, 27), new DateTime(2022, 5, 22) }, // Friday
            new object[] { new DateTime(2022, 5, 28), new DateTime(2022, 5, 22) }  // Saturday
        };
    }
}
