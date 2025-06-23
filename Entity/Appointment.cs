using Services.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class Appointment
    {
      
            public string CompanyID { get; set; }
            public int ApptID { get; set; }
            public string AppoinmentUId { get; set; }
            public int CustomerID { get; set; }
            public string ServiceTypeId { get; set; }
            public int? ResourceID { get; set; }
            public int? TimeSlotId { get; set; }
            public string ApptDateTime { get; set; }
            public string StartDateTime { get; set; }
            public string EndDateTime { get; set; }
            public string TimeSlot { get; set; }
            public string Note { get; set; } = string.Empty;
            public string StatusId { get; set; }
            public string TicketStatusId { get; set; }
            public string CreatedDateTime { get; set; }
            public bool? MarkDownloaded { get; set; } = false;
            public string PromoCode { get; set; }
            public string CreatedBy { get; set; } = string.Empty;
            public string UserID { get; set; }
            public Resource Resource { get; set; }
            public Customer Customer { get; set; }
            public Status Status { get; set; }
            public TicketStatus TicketStatus { get; set; }
            public ServiceType ServiceType { get; set; }
            public List<AppointmentInvoice> Invoices { get; set; } 
     
    }
}