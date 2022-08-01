using System;
using System.ComponentModel.DataAnnotations;

namespace DealerCenter.Models
{
    public class OrderSetDateViewModel
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
