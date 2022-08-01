using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DealerCenter.Models
{
    public enum PaymentStatusEnum : int
    {
        ОжиданиеПредоплаты = 1,
        ОжиданиеПолнойОплаты,
        ПолностьюОплачен,
        ЗаказОтменён
    }

    public class PaymentStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        //==================================
        public List<Order> Orders { get; set; }

        public PaymentStatus()
        {
            Orders = new List<Order>();
        }
    }
}
