using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class ServiceType
    {
        public string CompanyID { get; set; } 
        public int ServiceTypeID { get; set; }
        public string Resource { get; set; } 
        public string ServiceName { get; set; } 
        public string CreatedDateTime { get; set; }
        public int? Hour { get; set; } 
        public int? Minute { get; set; }
        public string CalenderColor { get; set; }
        public int? ReminderID { get; set; }
        public bool IsInternalUse { get; set; } = false; 
    }
}