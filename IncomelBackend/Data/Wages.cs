using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [Table("Wages")]
    public class Wages
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal IGSS { get; set; }
        public decimal IRTRA { get; set; }
        public decimal PaternityBonus { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal LiquidSalary { get; set; }

        //Relationships
        public Employee Employee { get; set; }
    }
}
