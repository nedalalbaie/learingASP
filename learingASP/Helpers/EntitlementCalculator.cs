namespace learingASP.Calculators
{
    public class EntitlementCalculator
    {
        private const decimal AnnualDays = 30m;

        public static decimal ForYear(DateOnly employeeStartDate, int year)
        {
            if (employeeStartDate.Year > year) return 0m;         
            if (employeeStartDate.Year < year) return AnnualDays;  

            var monthsRemaining = 13 - employeeStartDate.Month;
            return AnnualDays * monthsRemaining / 12;
        }
    }
}
