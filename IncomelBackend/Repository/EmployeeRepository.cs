using Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class EmployeeRepository : IEmployeeService
    {
        private readonly IncomelDBContext _dbContext;
        public EmployeeRepository(IncomelDBContext incomelDBContext)
        {
            _dbContext = incomelDBContext;
        }
        public Tuple<string, bool> Create(Employee employee)
        {
            try
            {
                employee.CreatedAt = DateTime.Now;
                _dbContext.Employees.Add(employee);
                _dbContext.SaveChanges();

                return Tuple.Create("Empleado agregado.", true);
            }
            catch (Exception ex)
            {
                Log.Error("EmployeeRepository - Create: " + ex.Message, ex);
                return Tuple.Create(ex.Message, false);
            }
        }

        public Tuple<string, bool> Delete(int employeeId)
        {
            try
            {
                var employee = this.GetEmpleoye(x => x.Id == employeeId);
                if (employee == null)
                {
                    return Tuple.Create("No se encontro ningun empleado con este filtro.", false);
                }

                _dbContext.Employees.Remove(employee.Item1);
                _dbContext.SaveChanges();

                return Tuple.Create("Empleado eliminado con éxito", true);
            }
            catch (Exception ex)
            {
                Log.Error("EmployeeRepository - Delete: " + ex.Message, ex);
                return Tuple.Create(ex.Message, false);
            }
        }

        public Tuple<Employee, string, bool> GetEmpleoye(Func<Employee, bool> filter)
        {
            try
            {
                var employee = _dbContext.Employees.Include(x => x.Wages).FirstOrDefault(filter);
                if (employee == null)
                {
                    return Tuple.Create(new Employee(), "No se encontro ningun empleado con este filtro.", false);
                }

                return Tuple.Create(employee, string.Empty, true);
            }
            catch (Exception ex)
            {
                Log.Error("EmployeeRepository - GetEmpleoye: " + ex.Message, ex);
                return Tuple.Create(new Employee(),ex.Message, false);
            }
        }

        public Tuple<List<Employee>, string, bool> GetEmpleoyes(Func<Employee, bool> filter = null)
        {
            try
            {
                var employees = new List<Employee>();
                if (filter == null)
                {
                    employees = _dbContext.Employees.Include(x => x.User).Include(x => x.Wages).ToList();
                    foreach (var item in employees)
                    {
                        item.User.PasswordHash = string.Empty;
                    }
                    if (employees == null)
                    {

                        return Tuple.Create(new List<Employee>(), "No se encontraron empleados.", false);
                    }
                }
                else
                {
                    employees = _dbContext.Employees.Include(x => x.User).Include(x => x.Wages).Where(filter).ToList();
                    foreach (var item in employees)
                    {
                        item.User.PasswordHash = string.Empty;
                    }
                    if (employees == null)
                    {

                        return Tuple.Create(new List<Employee>(), "No se encontraron empleados con ese filtro.", false);
                    }
                }


                return Tuple.Create(employees, string.Empty, true);
            }
            catch (Exception ex)
            {
                Log.Error("EmployeeRepository - GetEmpleoye: " + ex.Message, ex);
                return Tuple.Create(new List<Employee>(), ex.Message, false);
            }
        }

        public Tuple<string, bool> Update(Employee employee)
        {
            try
            {
                employee.CreatedAt = DateTime.Now;
                _dbContext.Entry(employee).State = EntityState.Modified;
                _dbContext.SaveChanges();

                return Tuple.Create("Empleado actualizado.", true);
            }
            catch (Exception ex)
            {
                Log.Error("EmployeeRepository - Update: " + ex.Message, ex);
                return Tuple.Create(ex.Message, false);
            }
        }
    }
}
