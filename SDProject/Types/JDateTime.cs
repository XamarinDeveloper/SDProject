using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using SDProject.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SDProject.Types {
    [JsonObject(MemberSerialization.OptIn)]
    public struct JDateTime : IFormattable {
        [JsonProperty]
        private readonly DateTime dateTime;
        private DateTime UtcDateTime => dateTime.ToUniversalTime();
        public int Year => new PersianCalendar().GetYear(dateTime);
        public int Month => new PersianCalendar().GetMonth(dateTime);
        public int Day => new PersianCalendar().GetDayOfMonth(dateTime);
        public DayOfWeek DayOfWeek => new PersianCalendar().GetDayOfWeek(dateTime);
        public int Hour => new PersianCalendar().GetHour(dateTime);
        public int Minute => new PersianCalendar().GetMinute(dateTime);
        public int Second => new PersianCalendar().GetSecond(dateTime);
        public double Millisecond => new PersianCalendar().GetMilliseconds(dateTime);

        public bool IsLeapYear => new PersianCalendar().IsLeapYear(Year);

        public JDateTime Date => dateTime.Date;
        public static JDateTime Now => DateTime.Now;
        public static JDateTime Today => DateTime.Today;

        private static readonly Dictionary<DayOfWeek, string> dayName = new Dictionary<DayOfWeek, string> {
            { DayOfWeek.Saturday, "شنبه" },
            { DayOfWeek.Sunday, "یکشنبه" },
            { DayOfWeek.Monday, "دوشنبه" },
            { DayOfWeek.Tuesday, "سه‌شنبه" },
            { DayOfWeek.Wednesday, "چهارشنبه" },
            { DayOfWeek.Thursday, "پنجشنبه" },
            { DayOfWeek.Friday, "جمعه" }
        };

        private static readonly List<string> monthName = new List<string> {
            "فروردین",
            "اردیبهشت",
            "خرداد",
            "تیر",
            "مرداد",
            "شهریور",
            "مهر",
            "آبان",
            "آذر",
            "دی",
            "بهمن",
            "اسفند",
        };

        public JDateTime(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int millisecond = 0) {
            dateTime = new PersianCalendar().ToDateTime(year, month, day, hour, minute, second, millisecond);
        }
        public JDateTime(DateTime dateTime) {
            this.dateTime = dateTime;
        }

        public JDateTime AddYears(int year) => dateTime.AddYears(year);
        public JDateTime AddMonths(int month) => dateTime.AddMonths(month);
        public JDateTime AddDays(int day) => dateTime.AddDays(day);
        public JDateTime AddHours(int hour) => dateTime.AddHours(hour);
        public JDateTime AddMinutes(int minute) => dateTime.AddMinutes(minute);
        public JDateTime AddSeconds(int second) => dateTime.AddSeconds(second);
        public JDateTime AddMilliseconds(double milliseconds) => dateTime.AddMilliseconds(milliseconds);

        public static JDateTime Parse(string date) {
            var dateRegex = new Regex(@"^(\d{4})/(\d{1,2})/(\d{1,2})$");
            var dateTimeRegex = new Regex(@"^(\d{4})/(\d{1,2})/(\d{1,2}) (\d{1,2}):(\d{1,2})(:(\d{1,2}))?$");
            if (dateRegex.IsMatch(date)) {
                var (year, month, day) = dateRegex.Match(date).Groups.Skip(1).Select(item => item.Value.ToInt32()).ToArray();
                return new JDateTime(year, month, day);
            }
            else if (dateTimeRegex.IsMatch(date)) {
                var (year, month, day, hour, minute) = dateTimeRegex.Match(date).Groups.Skip(1).Take(5).Select(item => item.Value.ToInt32()).ToArray();
                int second = 0;
                if(!dateTimeRegex.Match(date).Groups.Last().Value.IsEmpty()) {
                    second = dateTimeRegex.Match(date).Groups.Last().Value.ToInt32();
                }
                return new JDateTime(year, month, day, hour, minute, second);
            }
            else {
                throw new ArgumentException("Input format should be yyyy/MM/dd (HH:mm:ss)");
            }
        }

        public static bool TryParse(string date, out JDateTime result) {
            try {
                result = Parse(date);
                return true;
            }
            catch {
                result = Now;
                return false;
            }
        }

        public override string ToString() {
            return ToString("yyyy/MM/dd HH:mm:ss");
        }
        public string ToString(string format, IFormatProvider formatProvider = null) {
            format = format.Replace("yyyyy", $"{Year:00000}").Replace("yyyy", $"{Year:0000}").Replace("yyy", $"{Year:000}").Replace("yy", $"{Year % 100:00}").Replace("y", $"{Year % 100:0}");
            format = format.Replace("MMM", $"{monthName[Month - 1]}").Replace("MM", $"{Month:00}").Replace("M", $"{Month:0}");
            format = format.Replace("ddd", $"{dayName[DayOfWeek]}").Replace("dd", $"{Day:00}").Replace("d", $"{Day:0}");
            format = format.Replace("HH", $"{Hour:00}").Replace("H", $"{Hour:0}");
            format = format.Replace("hh", $"{(Hour + 11) % 12 + 1:00}").Replace("h", $"{(Hour + 11) % 12 + 1:0}");
            format = format.Replace("mm", $"{Minute:00}").Replace("m", $"{Minute:0}");
            format = format.Replace("ss", $"{Second:00}").Replace("s", $"{Second:0}");
            format = format.Replace("ttt", $"{(Hour < 12 ? "قبل‌از‌ظهر" : "بعد‌از‌ظهر")}").Replace("tt", $"{(Hour < 12 ? "ق.ظ" : "ب.ظ")}").Replace("t", $"{(Hour < 12 ? "ق" : "ب")}");
            return format;
        }

        public static implicit operator JDateTime(DateTime dateTime) {
            return new JDateTime(dateTime);
        }
        public static implicit operator JDateTime?(DateTime? dateTime) {
            return dateTime == null ? (JDateTime?)null : new JDateTime((DateTime)dateTime);
        }
        public static implicit operator DateTime(JDateTime persianDateTime) {
            return persianDateTime.dateTime;
        }
        public static implicit operator DateTime?(JDateTime? persianDateTime) {
            return persianDateTime == null ? (DateTime?)null : ((JDateTime)persianDateTime).dateTime;
        }
        public static implicit operator JDateTime(Timestamp timestamp) {
            return timestamp.ToDateTime().ToLocalTime();
        }
        public static implicit operator Timestamp(JDateTime? persianDateTime) {
            return persianDateTime == null ? null : Timestamp.FromDateTime(((JDateTime)persianDateTime).UtcDateTime);
        }

        public static bool operator ==(JDateTime jDateTime1, JDateTime jDateTime2) {
            return jDateTime1.dateTime == jDateTime2.dateTime;
        }
        public static bool operator !=(JDateTime jDateTime1, JDateTime jDateTime2) {
            return jDateTime1.dateTime != jDateTime2.dateTime;
        }
        public static bool operator >(JDateTime jDateTime1, JDateTime jDateTime2) {
            return jDateTime1.dateTime > jDateTime2.dateTime;
        }
        public static bool operator <(JDateTime jDateTime1, JDateTime jDateTime2) {
            return jDateTime1.dateTime < jDateTime2.dateTime;
        }
        public static bool operator >=(JDateTime jDateTime1, JDateTime jDateTime2) {
            return jDateTime1.dateTime >= jDateTime2.dateTime;
        }
        public static bool operator <=(JDateTime jDateTime1, JDateTime jDateTime2) {
            return jDateTime1.dateTime <= jDateTime2.dateTime;
        }


        public static JDateTime operator +(JDateTime jDateTime, TimeSpan timeSpan) {
            return new JDateTime(jDateTime.dateTime + timeSpan);
        }

        public static JDateTime operator -(JDateTime jDateTime, TimeSpan timeSpan) {
            return new JDateTime(jDateTime.dateTime - timeSpan);
        }

        public static TimeSpan operator -(JDateTime jDateTime1, JDateTime jDateTime12) {
            return jDateTime1.dateTime - jDateTime12.dateTime;
        }


        public override bool Equals(object obj) {
            return obj is JDateTime time &&
                   dateTime == time.dateTime;
        }
        public override int GetHashCode() {
            return HashCode.Combine(dateTime);
        }

    }
}