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

    public struct CronTimeContainer {
        public int Years { get; set; } = 0;
        public int Months { get; set; } = 0;
        public int Days { get; set; } = 0;
        public int Hours { get; set; } = 0;
        public int Minutes { get; set; } = 0;
        public int Seconds { get; set; } = 0;
        public int DayOfWeek { get; set; } = 0;
        public CronTimeContainer() { }
    }

    /// <summary>
    /// Class that represents task for <see cref="Cron"/>
    /// </summary>
    public class CronTask {
        public CronTask(CronTimeContainer container) { }

        /// <summary>
        /// Create task with time in *nix crontab format. Search in Google what is this, if you don't know (e. g. "crontab format").
        /// </summary>
        /// <param name="pattern">Cron schedule pattern. In (1 2 3 4 dow) 1 is minute, 2 is hour, 3 is day, 4 is month, dow is day of week.</param>
        /// <returns>Scheduled task</returns>
        public static CronTask FromCrontabPattern(string pattern) {
            string[] elems = pattern.Split(' ');
            // TODO: ranges
            var pat = new CronTimeContainer { };
            if (elems.Length > 5) throw new ArgumentOutOfRangeException("crontab pattern contains more than 5 elements");

            int GetSymbolValue(string el) {
                int output;
                if (!int.TryParse(el, out output)) {
                    if (el.StartsWith("*")) {
                        output = -1;
                    }
                    if (el.Contains("/")) {
                        var repeats = el.Split('/');
                        output /= Convert.ToByte(repeats[2]);
                    }
                }
                return output;
            }

            for (int i = 0; i < elems.Length; i++) {
                switch (i) {
                    case 0: // minutes
                        pat.Minutes = GetSymbolValue(elems[i]);
                        break;
                    case 1: // hours
                        pat.Hours = GetSymbolValue(elems[i]);
                        break;
                    case 2: // days
                        pat.Days = GetSymbolValue(elems[i]);
                        break;
                    case 3: // months
                        pat.Months = GetSymbolValue(elems[i]);
                        break;
                    case 4: // day of week
                        break;
                }
            }
            return new CronTask(pat);
        }
        /// <summary>
        /// Create task with time in systemd.timer onCalendar format. Search in Google what is this, if you don't know (e. g. "systemd oncalendar format").
        /// </summary>
        /// <param name="pattern">Cron schedule pattern. In (dow yy-mm-dd HH:MM:SS) dow is day of week, yy is year, mm is month, dd is day, HH is hour, MM is minute, SS is second.</param>
        /// <returns>Scheduled task</returns>
        public static CronTask FromSystemdOnCalendarPattern(string pattern) {
            return new CronTask(new CronTimeContainer { });
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
