using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public interface IEmployeeService
    {
        Tuple<string, bool> Create(Employee employee);
        Tuple<string, bool> Update(Employee employee);
        Tuple<Employee,string, bool> GetEmpleoye(Func< Employee,bool> filter);
        Tuple<List< Employee>,string, bool> GetEmpleoyes(Func<Employee, bool> filter = null);
        Tuple<string, bool> Delete(int employeeId);
    }
}
