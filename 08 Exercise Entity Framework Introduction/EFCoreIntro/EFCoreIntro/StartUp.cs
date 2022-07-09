using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            //03 Employees Full Information
            SoftUniContext dbContext = new SoftUniContext();

            //Console.WriteLine(GetEmployeesFullInformation(dbContext));

            //04 Employees with Salary Over 50 000
            // Console.WriteLine(GetEmployeesWithSalaryOver50000(dbContext));

            //05 Employees from Research and Development
            //Console.WriteLine(GetEmployeesFromResearchAndDevelopment(dbContext));

            //06 Adding a New Address and Updating Employee
            //Console.WriteLine(AddNewAddressToEmployee(dbContext));

            //07 Employees and Projects
            //Console.WriteLine(GetEmployeesInPeriod(dbContext));

            //08 Addresses by Town
            //Console.WriteLine(GetAddressesByTown(dbContext));

            //09 Employee 147
            //Console.WriteLine(GetEmployee147(dbContext));

            //10 Departments with More Than 5 Employees
            //Console.WriteLine(GetDepartmentsWithMoreThan5Employees(dbContext));

            //11 Find Latest 10 Projects
            //Console.WriteLine(GetLatestProjects(dbContext));

            //12 Increase Salaries
            //Console.WriteLine(IncreaseSalaries(dbContext));

            //13 Find Employees by First Name Starting With Sa
            //Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(dbContext));

            //14 Delete Project by Id
            //Console.WriteLine(DeleteProjectById(dbContext));

            //15 Remove Town
            Console.WriteLine(RemoveTown(dbContext));

        }

        // 03 Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var allEmployees = context
                .Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToArray();

            foreach (var e in allEmployees)
            {
                output
                    .AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");

            }

            return output.ToString().TrimEnd();
        }

        //04 Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var salaryEmployees = context
                .Employees
                .Where(e => e.Salary > 50000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ToArray();

            foreach (var e in salaryEmployees)
            {
                output
                    .AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return output.ToString().TrimEnd();
        }

        //05 Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var rndEmployees = context
                .Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();

            foreach (var rndE in rndEmployees)
            {
                output
                    .AppendLine($"{rndE.FirstName} {rndE.LastName} from {rndE.DepartmentName} - ${rndE.Salary:f2}");

            }

            return output.ToString().TrimEnd();
        }

        //06 Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(newAddress);

            Employee nakov = context
                .Employees
                .FirstOrDefault(e => e.LastName == "Nakov");
            nakov.Address = newAddress;
            context.SaveChanges();

            string[] addressTexts = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                .ToArray();

            foreach (string address in addressTexts)
            {
                output.AppendLine(address);
            }

            return output.ToString().TrimEnd();
        }

        //07 Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var a = context.Employees;

            var employeesWithProjects = context
                .Employees
                .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 &&
                                                          ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    AllProjects = e.EmployeesProjects
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            StartDate = ep.Project.StartDate
                                .ToString("M/d/yyyy h:mm:ss tt"),
                            EndDate = ep.Project.EndDate.HasValue
                                ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                                : "not finished"
                        })
                        .ToArray()
                })
                .ToArray();

            foreach (var e in employeesWithProjects)
            {
                output
                    .AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.AllProjects)
                {
                    output
                        .AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return output.ToString().TrimEnd();
        }

        //08 Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var addresses = context
                .Addresses
                .Include(x => x.Town)
                .Include(x => x.Employees)
                .OrderByDescending(x => x.Employees.Count())
                .ThenBy(x => x.Town.Name)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .ToList();

            foreach (var address in addresses)
            {
                output
                    .AppendLine($"{address.AddressText}, {address.Town.Name} - {address.Employees.Count()} employees");
            }

            return output.ToString().TrimEnd();

        }

        //09 Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employee147 = context
                .Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                        .Where(ei => e.EmployeeId == 147)
                        .Select(ep => new
                        {
                            ep.Project.Name
                        })
                        .OrderBy(ep => ep.Name)
                        .ToArray()
                })
                .ToArray();

            foreach (var e in employee147)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

                foreach (var p in e.Projects)
                {
                    output.AppendLine($"{p.Name}");
                }
            }

            return output.ToString().TrimEnd();
        }

        //10 Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var departments = context
                .Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(de => new
                {
                    de.Name,
                    ManagerFirstName = de.Manager.FirstName,
                    ManagerLastName = de.Manager.LastName,
                    Employees = de.Employees
                        .Select(e => new
                        {
                            e.FirstName,
                            e.LastName,
                            e.JobTitle,

                        })
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .ToArray()
                })
                .ToArray();

            foreach (var department in departments)
            {
                output
                    .AppendLine($"{department.Name} - {department.ManagerFirstName} {department.ManagerLastName}");
                foreach (var employee in department.Employees)
                {
                    output
                        .AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }

            return output.ToString().TrimEnd();
        }

        //11 Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var projects = context
                .Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate
                        .ToString("M/d/yyyy h:mm:ss tt")
                })
                .OrderBy(p => p.Name)
                .ToArray();



            foreach (var project in projects)
            {
                output
                    .AppendLine($"{project.Name}")
                    .AppendLine($"{project.Description}")
                    .AppendLine($"{project.StartDate}");
            }

            return output.ToString().TrimEnd();
        }

        //12 Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.Department.Name == "Engineering" ||
                            e.Department.Name == "Tool Design" ||
                            e.Department.Name == "Marketing" ||
                            e.Department.Name == "Information Services")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    newSalary = e.Salary * (decimal)1.12
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var employee in employees)
            {
                output
                    .AppendLine($"{employee.FirstName} {employee.LastName} (${employee.newSalary:f2})");
            }

            return output.ToString().TrimEnd();
        }

        //13 Find Employees by First Name Starting With Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var saEmployees = context
                .Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var employee in saEmployees)
            {
                output
                    .AppendLine(
                        $"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:f2})");
            }

            return output.ToString().TrimEnd();
        }

        //14 Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Project projToDelete = context
                .Projects
                .Find(2);

            EmployeeProject[] referredEmployees = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == projToDelete.ProjectId)
                .ToArray();

            context.EmployeesProjects.RemoveRange(referredEmployees);
            context.Projects.Remove(projToDelete);
            context.SaveChanges();

            string[] projectNames = context
                .Projects
                .Take(10)
                .Select(p => p.Name)
                .ToArray();

            foreach (string pName in projectNames)
            {
                output.AppendLine(pName);
            }

            return output.ToString().TrimEnd();
        }

        //15 Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            Town townToDelete = context
                .Towns
                .FirstOrDefault(t => t.Name == "Seattle");

            Address[] adressesToBeDeleted = context
                .Addresses
                .Where(a => a.Town.TownId == townToDelete.TownId)
                .ToArray();

            int deletedAddresses = adressesToBeDeleted.Count();

            var employeeIdAddresses = context
                .Employees
                .Where(e => e.Address.TownId == townToDelete.TownId)
                .ToArray();

            foreach (var addressId in employeeIdAddresses)
            {
                addressId.AddressId = null;
            }

            context.Addresses.RemoveRange(adressesToBeDeleted);
            context.Towns.Remove(townToDelete);
            context.SaveChanges();

            return $"{deletedAddresses} addresses in Seattle were deleted";

        }
    }
}
