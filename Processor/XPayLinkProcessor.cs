using CECPro.Openedge;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Services.Processor
{
    public class XPayLinkProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        //    public  string GetCSPaymentLink(string CompanyID, string CustomerID, string InvoiceNo, string CustomerName, string email, string amount)
        //    {
        //        string firstName = "";
        //        string lastName = "";
        //        string UserID = HttpContext.Current.Session["LoginUser"].ToString();
        //        if (!string.IsNullOrEmpty(CustomerName))
        //        {
        //            string[] parts = CustomerName.Split(' ');
        //            firstName = string.Join(" ", parts.Take(parts.Length - 1));
        //            lastName = parts.Last();

        //        }
        //        string paymentLink = "";
        //        PaymentServiceSoapClient ps = new PaymentServiceSoapClient();
        //        try
        //        {
        //            paymentLink = ps.GetCSLink(CompanyID, firstName, lastName, email, InvoiceNo, amount, UserID);
        //        }
        //        catch (Exception ex)
        //        {
        //            return "";
        //        }

        //        if (paymentLink.Contains("Error"))
        //        {
        //            return "";
        //        }

        //        return paymentLink;
        //    }
        //}

        public string GetCSPaymentLink(string RMCompanyID, string CustomerID, string InvoiceNo, string CustomerName, string email, string amount)
        {
            string firstName = "";
            string lastName = "";

            if (!string.IsNullOrEmpty(CustomerName))
            {
                string[] parts = CustomerName.Split(' ');
                firstName = string.Join(" ", parts.Take(parts.Length - 1));
                lastName = parts.Last();
            }

            string paymentLink = "";
            string CompanyID = GetCompanyID(RMCompanyID);

            if (string.IsNullOrEmpty(CompanyID))
            {
                return ""; 
            }

            string UserID = GetUserID(CompanyID); 

            if (string.IsNullOrEmpty(UserID))
            {
                return ""; 
            }
            //string invoiceId =GetInvoiceID(CompanyID, InvoiceNo);
            //string invoiceId = "4107527";

            //if (string.IsNullOrEmpty(invoiceId))
            //{
            //    return "";
            //}
            PaymentServiceSoapClient ps = new PaymentServiceSoapClient();
            try
            {
                paymentLink = ps.GetCSLink(CompanyID, firstName, lastName, email, InvoiceNo, amount, UserID);
            }
            catch (Exception ex)
            {
                return "";
            }

            if (paymentLink.Contains("Error"))
            {
                return "";
            }

            return paymentLink;
        }
        private string GetCompanyID(string RMCompanyID)
        {
            string companyID = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT [CompanyID] FROM [msSchedulerV3].[dbo].[tbl_AMCompanyMapping] WHERE AM_CompanyID = @RMCompanyID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RMCompanyID", RMCompanyID);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            companyID = result.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return companyID;
        }

        private string GetUserID(string CompanyID)
        {
            string userID = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT TOP 1 [UserID] FROM [XinatorCentral].[dbo].[tbl_User] WHERE CompanyID = @CompanyID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyID", CompanyID);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            userID = result.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return userID;
        }
        private string GetInvoiceID(string CompanyID, string invoiceNo)
        {
            string  invoiceID = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT TOP 1 invoiceid FROM [msSchedulerV3].[dbo].[tbl_Invoice] WHERE CompnyID = @CompanyID and number = '"+invoiceNo+"'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyID", CompanyID);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            invoiceID = result.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return invoiceID;
        }
    }
}