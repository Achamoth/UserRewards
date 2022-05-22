namespace Rewards.Services
{
    public interface IDateTimeService
    {
        public DateTime Now { get; }
    }

    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}
