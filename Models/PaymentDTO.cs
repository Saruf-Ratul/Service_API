using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Models
{
    public class PaymentDTO
    {
        public string CompanyID { get; set; }
        public string InvocieId { get; set; }
        public string Amount { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public string CheckName { get; set; }
        public string CheckNumber { get; set; }
    }
}