using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [Table("UserRoleOption")]
    public class UserRoleOption
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int OptionId { get; set; }

        //Relationships
        public User User { get; set; }
        public Role Role { get; set; }
        public Option Option { get; set; }
    }
}
