using System;

namespace Extensions
{
    public class DateTimeHelpers
    {
        // Return the number of years, months, days, hours,
        // minutes, seconds, and milliseconds you need to add to
        // from_date to get to_date.
        public static void GetElapsedTime(DateTime fromDate, DateTime toDate, 
            out int years, out int months, out int days, out int hours,
            out int minutes, out int seconds, out int milliseconds)
        {
            // If from_date > to_date, switch them around.
            if (fromDate > toDate)
            {
                GetElapsedTime(toDate, fromDate,
                    out years, out months, out days, out hours,
                    out minutes, out seconds, out milliseconds);
                years = -years;
                months = -months;
                days = -days;
                hours = -hours;
                minutes = -minutes;
                seconds = -seconds;
                milliseconds = -milliseconds;
            }
            else
            {
                // Handle the years.
                years = toDate.Year - fromDate.Year;

                // See if we went too far.
                DateTime test_date = fromDate.AddMonths(12 * years);
                if (test_date > toDate)
                {
                    years--;
                    test_date = fromDate.AddMonths(12 * years);
                }

                // Add months until we go too far.
                months = 0;
                while (test_date <= toDate)
                {
                    months++;
                    test_date = fromDate.AddMonths(12 * years + months);
                }
                months--;

                // Subtract to see how many more days,
                // hours, minutes, etc. we need.
                fromDate = fromDate.AddMonths(12 * years + months);
                TimeSpan remainder = toDate - fromDate;
                days = remainder.Days;
                hours = remainder.Hours;
                minutes = remainder.Minutes;
                seconds = remainder.Seconds;
                milliseconds = remainder.Milliseconds;
            }
        }
    }
}