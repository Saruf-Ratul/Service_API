using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class Customer
    {
        public string CompanyID { get; set; } = string.Empty;
        public string CreatedCompanyID { get; set; } = string.Empty;
        public int TagID { get; set; } = 0;
        public string CustomerID { get; set; }
        public string AMCustomerID { get; set; } = "0";
        public string CustomerGuid { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Title2 { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string FirstName2 { get; set; }
        public string  LastName { get; set; }
        public string  LastName2 { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string  JobTitle2 { get; set; }
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Notes { get; set; }
        public string CreatedDateTime { get; set; }
        public bool CallPopUploaded { get; set; } = false;
        public int QboId { get; set; } = 0;
        public string CallPopAppId { get; set; }
        public bool IsPrimaryContact { get; set; } = false;
        public int BusinessID { get; set; } = 0;
        public int SyncToken { get; set; } = 0;
        public string BusinessName { get; set; } = string.Empty;
        public bool IsBusinessContact { get; set; } = false;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyName2 { get; set; }
        public string DealerID { get; set; } = string.Empty;
        public bool IsDealer { get; set; } = false;
        public string CustomerCode { get; set; }
        public string UpdateDate { get; set; }
    }
}