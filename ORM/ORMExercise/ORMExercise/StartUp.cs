
using Microsoft.EntityFrameworkCore;
using ORMExercise.Data;

namespace ORMExercise
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            using (SoftUniContext context = new SoftUniContext())
            {
                DateTime oldDate = new DateTime(2000, 1, 1);
                var employees = context.Employees.Where(e => e.HireDate < oldDate).ToList();

                foreach (var e in employees)
                {
                    Console.WriteLine(e.FirstName + ' ' + e.LastName);
                }
            }
        }
    }
}