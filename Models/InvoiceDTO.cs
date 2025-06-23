using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.Models
{
    public class InvoiceDTO
    {
        public string id { get; set; }
        public string Number { get; set; }
        public string CompanyID { get; set; }
        public string CompnyID { get; set; }  // Note the typo
        public string DisplayNumber { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string InvoiceType { get; set; }
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Note { get; set; }
        public string  CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string InvoiceDate { get; set; }
        public decimal AmountCollect { get; set; }
        public string TaxType { get; set; }
        public string AppointmentId { get; set; }
        public string Type { get; set; }
        public int QboId { get; set; }
        public string DiscountRate { get; set; }
        public string DiscountOption { get; set; }
        public int? QboEstimateId { get; set; }
        public string ExpirationDate { get; set; }
        public string SyncToken { get; set; }
        public string QboPaymentID { get; set; }
        public decimal DepositAmount { get; set; }
        public string LoanStatus { get; set; }
        public bool IsConverted { get; set; }
        public string ConvertedInvocieID { get; set; }
        public string ConvertedInvocieNumber { get; set; }
        public List<InvoiceItem> items { get; set; }

    }
    public class InvoiceItem
    {
        public string ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string TotalPrice { get; set; }
        public string IsTaxable { get; set; }
        public string ItemTyId { get; set; }
       
    }

    public class InvoiceEditDTO
    {
        public string InvoiceID { get; set; }
        public string CompnyID { get; set; } 
        public string Number { get; set; }
        public string DisplayNumber { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string InvoiceType { get; set; }
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string Note { get; set; }
        public string InvoiceDate { get; set; }
        public decimal AmountCollect { get; set; }
        public string Type { get; set; }
        public string DiscountRate { get; set; }
        public string DiscountOption { get; set; }
        public string ExpirationDate { get; set; }
        public decimal DepositAmount { get; set; }
        public string LoanStatus { get; set; }
        public bool IsConverted { get; set; }
        public string ConvertedInvocieID { get; set; }
        public string ConvertedInvocieNumber { get; set; }
        public string TaxType { get; set; }
        public List<InvoiceItem> items { get; set; }
    }

}