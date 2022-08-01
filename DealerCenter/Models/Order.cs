using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DealerCenter.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public double PurchasePrice { get; set; }
        public double SellingPrice { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public int? ClientId { get; set; }
        public Client Client { get; set; }
        public int? SalesManagerId { get; set; }
        public Employee SalesManager { get; set; }
        public int? PurchaseManagerId { get; set; }
        public Employee PurchaseManager { get; set; }
        public int? MachineryId { get; set; }
        public Machinery Machinery { get; set; }
        public int? OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int? PaymentStatusId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
