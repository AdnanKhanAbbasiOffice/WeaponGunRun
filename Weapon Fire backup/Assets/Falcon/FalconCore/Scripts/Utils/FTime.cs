using System;
using System.Diagnostics;
using System.Globalization;

namespace Falcon.FalconCore.Scripts.Utils {
    public static class FTime {

        public static long CurrentTimeMillis()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        public static long CurrentTimeSec()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        
        public static long NanoTime() {
            long nano = 10000L * Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }

        public static String DateToString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static DateTime StringToDate(String dateStr)
        {
            return DateTime.Parse(dateStr, CultureInfo.InvariantCulture);
        }
    }
}