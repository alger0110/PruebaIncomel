using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [Table("User")]
    public class User
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [NotMapped]
        public string Token { get; set; }

        //Relationships
        public IList<UserRole> UserRole { get; set; }
        public IList<UserRoleOption> UserRoleOptions { get; set; }
    }
}
