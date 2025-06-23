using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Models
{
    public class AppointmentDTO
    {
        public string CompanyID { get; set; }
        public int ApptID { get; set; }
        public string AppoinmentUId { get; set; }
        public int CustomerID { get; set; }
        public string ServiceTypeId { get; set; }
        public int? ResourceID { get; set; }
        public int? TimeSlotId { get; set; }
        public DateTime ApptDateTime { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string TimeSlot { get; set; }
        public string Note { get; set; } = string.Empty;
        public string StatusId { get; set; }
        public string TicketStatusId { get; set; }
        public string PromoCode { get; set; }
        public string UserID { get; set; }
    }
}