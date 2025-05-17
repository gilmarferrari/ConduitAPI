using System.Security.Cryptography;

namespace Conduit.Application.Helpers
{
    public static class Extensions
    {
        private static readonly Random _random = new Random();

        public static string GetLastSeen(DateTime? lastSeen, bool hasActiveClients)
        {
            if (hasActiveClients)
            {
                return "Online";
            }

            if (lastSeen == null || !lastSeen.HasValue)
            {
                return "Offline";
            }

            var date = lastSeen.Value.ToString("MM/dd/yyyy");
            var time = lastSeen.Value.ToString("HH:mm");

            if (lastSeen.Value.AddDays(7).Date > DateTime.UtcNow.Date)
            {
                if (lastSeen.Value.Date == DateTime.UtcNow.Date)
                {
                    return $"Last seen today at {time}";
                }

                if (lastSeen.Value.Date == DateTime.Today.AddDays(-1).Date)
                {
                    return $"Last seen yesterday at {time}";
                }

                return $"Last seen {lastSeen.Value.DayOfWeek.ToString().ToLower()} at {time}";
            }
            return $"Last seen {date} {time}";
        }

        public static (string, DateTime) GenerateRandomCode()
        {
            var randomToken = _random.Next(00014423, 99949031).ToString("00000000");

            return (randomToken, DateTime.UtcNow.AddHours(4));
        }

        public static string GenerateRandomToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(40);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }

    public enum TokenType
    {
        ActivationToken = 0,
        ResetToken = 1,
    }
}
