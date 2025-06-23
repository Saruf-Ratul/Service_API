using Microsoft.VisualStudio.DebuggerVisualizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResponseEntity
{
    public class Response
    {
        public Boolean IsValid { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string CompanyID { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyTag { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Id { get; set; } 

    }
}
