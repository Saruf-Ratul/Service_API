using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class Tax
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rate { get; set; }
    }
}