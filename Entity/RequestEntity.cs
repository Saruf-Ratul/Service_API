using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class RequestEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceID { get; set; }
        public string CompanyID { get; set; }
    }
}