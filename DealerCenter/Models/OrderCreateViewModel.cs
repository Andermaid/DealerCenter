using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealerCenter.Models
{
    public class OrderCreateViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientPassport { get; set; }
        public string ClientPhoneNumber { get; set; }
        public int SalesManagerId { get; set; }
        public int PurchaseManagerId { get; set; }
        public int MachineryId { get; set; }
    }
}
