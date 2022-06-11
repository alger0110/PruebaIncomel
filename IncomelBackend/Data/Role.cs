using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [Table("Role")]
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //Relationships
        //public AppRoleMenu AppRoleMenu { get; set; }
        public IList<UserRole> UserRole { get; set; }
        public IList<UserRoleOption> UserRoleOptions { get; set; }
    }
}
