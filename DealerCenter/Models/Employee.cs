using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DealerCenter.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool isDeleted { get; set; }
        public int? RoleId { get; set; }
        public Role Role { get; set; }
    }
}
