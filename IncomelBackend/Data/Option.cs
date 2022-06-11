using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [Table("Option")]
    public class Option
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Relationships

        public IList<UserRoleOption> UserRoleOptions { get; set; }
    }
}
