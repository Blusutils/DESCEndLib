using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DESCEnd {
    /// <summary>
    /// Interface for objects what can be added to <see cref="Cron"/>
    /// </summary>
    public interface ICronable {
        
    }

    public class CronTimeSpan : IComparable, IComparable<CronTimeSpan>, IEquatable<CronTimeSpan>, ISpanFormattable, IFormattable {
        public int CompareTo(object? obj) => throw new NotImplementedException();
        public int CompareTo(CronTimeSpan other) => throw new NotImplementedException();
        public bool Equals(CronTimeSpan? other) => throw new NotImplementedException();
        public string ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => throw new NotImplementedException();
    }

    /// <summary>
    /// Class that represents task for <see cref="Cron"/>
    /// </summary>
    public class CronTask {
        public CronTask(CronTimeSpan timeSpan) { }

        /// <summary>
        /// Create task with time in *nix crontab format. Search in Google what is this, if you don't know (e. g. "crontab format").
        /// </summary>
        /// <param name="pattern">Cron schedule pattern. In (1 2 3 4 dow) 1 is minute, 2 is hour, 3 is day, 4 is month, dow is day of week.</param>
        /// <returns>Scheduled task</returns>
        public static CronTask FromCrontabPattern(string pattern) {
            return new CronTask(new CronTimeSpan());
        }
        /// <summary>
        /// Create task with time in systemd.timer onCalendar format. Search in Google what is this, if you don't know (e. g. "systemd oncalendar format").
        /// </summary>
        /// <param name="pattern">Cron schedule pattern. In (dow yy-mm-dd HH:MM:SS) dow is day of week, yy is year, mm is month, dd is day, HH is hour, MM is minute, SS is second.</param>
        /// <returns>Scheduled task</returns>
        public static CronTask FromSystemdOnCalendarPattern(string pattern) {
            return new CronTask(new CronTimeSpan());
        }
    }
    /// <summary>
    /// *nix cron-like scheduler.
    /// </summary>
    public class Cron {
        public delegate void CronTaskDelegate(CronTask task);
        public static event CronTaskDelegate OnCronTaskStart;
        public static event CronTaskDelegate OnCronTaskCall;
        public void AddCronTask(CronTask task) {
            ;
        }
    }
}
