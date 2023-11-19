namespace Services.Helpers
{
    public static class Extensions
    {
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

                switch (lastSeen.Value.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return $"Last seen sunday at {time}";
                    case DayOfWeek.Monday:
                        return $"Last seen monday at {time}";
                    case DayOfWeek.Tuesday:
                        return $"Last seen tuesday at {time}";
                    case DayOfWeek.Wednesday:
                        return $"Last seen wednesday at {time}";
                    case DayOfWeek.Thursday:
                        return $"Last seen thursday at {time}";
                    case DayOfWeek.Friday:
                        return $"Last seen friday at {time}";
                    case DayOfWeek.Saturday:
                        return $"Last seen saturday at {time}";
                }
            }

            return $"Last seen {date} {time}";
        }
    }
}
