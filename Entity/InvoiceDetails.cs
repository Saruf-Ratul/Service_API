using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Entity
{
    public class InvoiceDetails
    {
        public string InvoiceID { get; set; }
        public Guid CustomerGuid { get; set; }
        public string FullName { get; set; }
        public string InvoiceDate { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public Invoice Invoice { get; set; }
        public List<Item> Items { get; set; }
        public List<InvoiceDetailsItemWise> InvoiceDetailsList {get; set;}
    }
    public class Invoice {
        public string InvoiceID { get; set; }
        public Guid CustomerGuid { get; set; }
        public string FullName { get; set; }
        public string QBOCustomerId { get; set; }
        public string CustomerId { get; set; }
        public decimal DepositAmount { get; set; }
        public string QBOId { get; set; }
        public string City { get; set; }
        public string Number { get; set; }
        public string InvoiceDate { get; set; }
        public decimal Subtotal { get; set; }
        public bool IsConverted { get; set; }
        public string ConvertedInvoiceID { get; set; }
        public decimal Due { get; set; }
        public decimal AmountCollect { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public double Surcharge { get; set; }
        public string Note { get; set; }
    } 
    public class Item
    {

        public string Id { get; set; }

      
        public string Name { get; set; }

        public string Description { get; set; } = "";

     
        public string Barcode { get; set; }

    
        public int ItemTypeId { get; set; }

  
        public decimal Price { get; set; }

        public string Location { get; set; }


        public string IsTaxable { get; set; }


        public string CompanyId { get; set; }

  

        public long? QboId { get; set; }
    }
    public class InvoiceDetailsItemWise
    {
        public Guid ID { get; set; } = Guid.NewGuid();

 
        public int SortId { get; set; }

        public string RefId { get; set; }

        public string CompanyId { get; set; }

        public string InvoiceId { get; set; }

        public string InvoiceNumber { get; set; }

        public string ItemId { get; set; }

        public string LineNum { get; set; } = "0";

        public decimal? Quantity { get; set; }

        public decimal? UPrice { get; set; }

        public string ItemName { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public decimal? TotalPrice { get; set; }

        public string CreatedDate { get; set; }

        public string ItemTyId { get; set; }

        public string IsTaxable { get; set; }

        public string ServiceDate { get; set; }
    }
}