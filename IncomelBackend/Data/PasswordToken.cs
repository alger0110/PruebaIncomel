using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [Table("PasswordToken")]
    public class PasswordToken
    {
        public int? Id { get; set; }
        public string Value { get; set; }
        public int UserId { get; set; }
        public bool Valid { get; set; }

        //Relationships
        public User User { get; set; }
    }
}
