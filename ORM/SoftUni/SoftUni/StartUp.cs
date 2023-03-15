using SoftUni.Data;
using SoftUni.Models;
using System.Text;

namespace SoftUni;


public class StartUp
{
  static void Main(string[] args)
    {
        SoftUniContext dbcontext = new SoftUniContext();
        string result = GetEmployeesInPeriod(dbcontext);
        Console.WriteLine(result);
    }


    //problem 03
    public static string GetEmployeesFullInformation(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employees = context.Employees.OrderBy(e => e.EmployeeId).Select(e => new{
            e.FirstName,
            e.LastName,
            e.MiddleName,
            e.Salary,
            e.JobTitle
        }).ToArray();

        foreach(var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    //problem 04
    public static string GetEmployeesWithSalaryOver50000(SoftUniContext context) 
    {
        StringBuilder sb = new StringBuilder();

        var employees = context.Employees
            .Where(e => e.Salary > 50000)
            .OrderBy(e => e.FirstName)
            .Select(e => new{e.FirstName,e.Salary})
            .ToArray();

        foreach(var e in employees)
        {
            sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
        }


        return sb.ToString().TrimEnd();
    }

    //problem 05
    public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employees = context.Employees
            .Where(e => e.Department.Name == "Research and Development")
            .OrderBy(e => e.Salary)
            .ThenByDescending(e => e.FirstName)
            .Select(e => new {e.FirstName,e.LastName,e.Salary,DepartmentName = e.Department.Name })
            .ToArray();

        foreach(var e in employees)
        {
            sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
        }

        return sb.ToString().TrimEnd();
    }

    //problem 06
    public static string AddNewAddressToEmployee(SoftUniContext context)
    {
        Address address = new Address();
        address.AddressText = "Vitoshka 15";
        address.TownId = 4;

        var searchedEmployee = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");
        if(searchedEmployee != null)
        {
            context.Employees.FirstOrDefault(e => e.LastName == "Nakov").Address = address;
        }
        context.SaveChanges();

        StringBuilder sb = new StringBuilder();
        var addresses = context.Employees
            .OrderByDescending(e => e.AddressId)
            .Take(10)
            .Select(e => e.Address.AddressText);

        foreach(string a in addresses)
        {
            sb.AppendLine(a);
        }

        return sb.ToString().TrimEnd();
    }

    //problem 07
    public static string GetEmployeesInPeriod(SoftUniContext context) 
    {
        StringBuilder sb = new StringBuilder();

        DateTime startDate = new DateTime(2001, 1, 1);
        DateTime endDate = new DateTime(2003, 12, 31);

        var employeesPr = context.EmployeesProjects
            .Where(ep => ep.Project.StartDate >= startDate || ep.Project.StartDate <= endDate).Take(10)
            .ToArray();
        int[] employeesIds = employeesPr.Select(ep => ep.EmployeeId).ToArray();
        int[] projectIds = employeesPr.Select(ep => ep.ProjectId).ToArray();

        List<Employee> employees = new List<Employee>();
        List<Project> projects = new List<Project>();
        foreach(int employeeId in employeesIds)
        {
            employees.Add(context.Employees.Find(employeeId));
        }

        foreach(int projectId in projectIds)
        {
            projects.Add(context.Projects.Find(projectId));
        }


        foreach(var e in employees)
        {
            int count = 0;
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.Manager?.FirstName} {e.Manager?.LastName}");
                foreach (var p in projects)
                {
                if(employeesPr.Where(ep => ep.ProjectId == p.ProjectId 
                 && e.EmployeeId == ep.EmployeeId).Skip(count).Count<EmployeeProject>() >= count)
                    count++;
                    sb.AppendLine($"--{p.Name} - {p.StartDate.ToString("M/d/yyyy h:mm:ss tt")} - {p.EndDate?.ToString("M/d/yyyy h:mm:ss tt")}");
                }
        }

        return sb.ToString().TrimEnd();
    }


}
