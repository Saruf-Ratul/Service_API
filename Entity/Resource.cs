using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class Resource
    {
        public int Id { get; set; } 
        public string CompanyID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public int WorkingHour { get; set; } 
        public bool SaterDay { get; set; } 
        public bool Sunday { get; set; }
        public bool Monday { get; set; } 
        public bool Tuesday { get; set; } 
        public bool Wednesday { get; set; } 
        public bool Thursday { get; set; } 
        public bool Friday { get; set; } 
        public string Mobile { get; set; } 
        public string Email { get; set; } 
    }
}