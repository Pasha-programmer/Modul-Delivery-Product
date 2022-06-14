using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModulDelivery.Infrastructure
{
    public class WorkSchedule
    {
        public WorkSchedule(Day[] days)
        {
            if (days.Length != 7)
                throw new ArgumentOutOfRangeException("Попытка записать в недельное расписание не 7 дней.");

            schedule.Add(DayOfWeek.Monday, days[0]);
            schedule.Add(DayOfWeek.Tuesday, days[1]);
            schedule.Add(DayOfWeek.Wednesday, days[2]);
            schedule.Add(DayOfWeek.Thursday, days[3]);
            schedule.Add(DayOfWeek.Friday, days[4]);
            schedule.Add(DayOfWeek.Saturday, days[5]);
            schedule.Add(DayOfWeek.Sunday, days[6]);
        }

        public WorkSchedule(string[] days)
        {
            if (days.Length != 7)
                throw new ArgumentOutOfRangeException("Попытка записать в недельное расписание не 7 дней.");

            var parseDays = days
                .Select(day => {
                    var d = day.Split(' ');
                    return new Day(DateTime.Parse(d.First()), DateTime.Parse(d.Last()));
                })
                .ToArray();

            schedule.Add(DayOfWeek.Monday, parseDays[0]);
            schedule.Add(DayOfWeek.Tuesday, parseDays[1]);
            schedule.Add(DayOfWeek.Wednesday, parseDays[2]);
            schedule.Add(DayOfWeek.Thursday, parseDays[3]);
            schedule.Add(DayOfWeek.Friday, parseDays[4]);
            schedule.Add(DayOfWeek.Saturday, parseDays[5]);
            schedule.Add(DayOfWeek.Sunday, parseDays[6]);
        }

        public Dictionary<DayOfWeek, Day> schedule { get; private set; }
    }

    public class Day
    {
        public Day(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        public DateTime From;
        public DateTime To;
    }

}
