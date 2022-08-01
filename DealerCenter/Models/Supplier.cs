using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DealerCenter.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool isDeleted { get; set; }
        //=====================================
        public List<Machinery> Machineries { get; set; }

        public Supplier()
        {
            Machineries = new List<Machinery>();
        }
    }
}
