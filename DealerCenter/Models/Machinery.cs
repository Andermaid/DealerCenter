using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DealerCenter.Models
{
    public class Machinery
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int YearOfManufacture { get; set; }
        public double PurchasePrice { get; set; }
        public double SellingPrice { get; set; }
        public bool isDeleted { get; set; }
        //========================================
        public int? MachineryClassId { get; set; }
        public MachineryClass MachineryClass { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public List<Order> Orders { get; set; }

        public Machinery()
        {
            Orders = new List<Order>();
        }
    }
}
