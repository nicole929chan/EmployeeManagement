using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{    
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee() { Id = 1, Name = "Nick", Department = Dept.HR, Email = "nick@example.com"},
                new Employee() { Id = 2, Name = "Simon", Department = Dept.IT, Email = "simon@example.com"},
                new Employee() { Id = 3, Name = "John", Department = Dept.IT, Email = "john@example.com"}
            };
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == Id);
        }

        Employee IEmployeeRepository.Add(Employee employee)
        {
            employee.Id = _employeeList.Max(e => e.Id) + 1;
            _employeeList.Add(employee);

            return employee;
        }

        Employee IEmployeeRepository.Delete(int id)
        {
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                _employeeList.Remove(employee);
            }

            return employee;
        }

        Employee IEmployeeRepository.Update(Employee employeeChanges)
        {
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == employeeChanges.Id);
            if (employee != null)
            {
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }

            return employee;
        }
    }
}
