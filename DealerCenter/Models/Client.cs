using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DealerCenter.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Passport { get; set; }
        public string PhoneNumber { get; set; }
        public List<Order> Orders { get; set; }

        public Client()
        {
            Orders = new List<Order>();
        }
    }
}
