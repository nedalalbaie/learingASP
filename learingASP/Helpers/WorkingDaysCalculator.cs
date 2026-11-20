namespace learingASP.Calculators
{
    public class WorkingDaysCalculator
    {
        public static bool IsWorkingDay(DateOnly date)
         => date.DayOfWeek is not (DayOfWeek.Friday or DayOfWeek.Saturday);

        public static int Count(DateOnly start, DateOnly end)
        {
            if (end < start)
                throw new ArgumentException("End date cannot be before start date.", nameof(end));

            var count = 0;

            for (var day = start; day <= end; day = day.AddDays(1))
            {
                if (IsWorkingDay(day))
                    count++;
            }

            return count;
        }
    }
}
