using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class TicketStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string CompanyId { get; set; }
    }
}