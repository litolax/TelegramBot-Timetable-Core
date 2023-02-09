using System.Timers;
using Timer = System.Timers.Timer;

namespace TelegramBot_Timetable_Core.Services
{
    public interface ISpamService
    {
        void AddToSpam(long userId);
        bool IsSpammer(long userId);
    }

    public class SpamService : ISpamService
    {
        private Dictionary<long, DateTime> Spammers { get; set; } = new();
        private Timer _timer = new(10000) { AutoReset = true, Enabled = true };

        public SpamService()
        {
            this._timer.Elapsed += this.ValidationSpammersTimeout;
        }

        private void ValidationSpammersTimeout(object? sender, ElapsedEventArgs e)
        {
            foreach (var (spammerId, timeoutTime) in this.Spammers)
            {
                if (DateTime.UtcNow <= timeoutTime.AddMinutes(2)) continue;
                this.Spammers.Remove(spammerId);
            }
        }

        public void AddToSpam(long userId)
        {
            if (this.Spammers.ContainsKey(userId)) return;
            this.Spammers.Add(userId, DateTime.UtcNow);
        }

        public bool IsSpammer(long userId) => this.Spammers.ContainsKey(userId);
    }
}