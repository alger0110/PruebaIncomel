using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [Table("Employee")]
    public class Employee
    {
        public int Id { get; set; }
        public string DPI { get; set; }
        public string FullName { get; set; }
        public decimal BaseSalary { get; set; }
        public int DecreeBond { get; set; }
        public int Sons { get; set; }
        [Column("CreatedBy")]
        public int UserId { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }

        //Relationships
        public User User { get; set; }
        public IList<Wages> Wages { get; set; }
    }
}
