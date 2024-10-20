namespace Auditable.Infrastructure
{
    using System;

    public static class SystemDateTime
    {
        private static Func<DateTime> _getDateTime = () => DateTime.UtcNow;

        public static DateTime UtcNow => _getDateTime();

        public static void SetDateTime(Func<DateTime> getDateTime)
        {
            Code.Require(()=> getDateTime != null, nameof(getDateTime));
            _getDateTime = getDateTime;
        }

        public static void Reset()
        {
            _getDateTime = () => DateTime.UtcNow;
        }
    }
}