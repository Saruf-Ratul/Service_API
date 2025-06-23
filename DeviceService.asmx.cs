using Newtonsoft.Json;
using ResponseEntity;
using Services.Entity;
using Services.Models;
using Services.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

namespace Services
{
    /// <summary>
    /// Summary description for DeviceService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class DeviceService : System.Web.Services.WebService
    {
        
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void VerifyUser(string UserName,string Password)
        {
            Response response = new Response();
            LoginProcessor loginProcessor = new LoginProcessor();
            response = loginProcessor.VerifyUser(new RequestEntity { UserName= UserName,Password = Password });
               

          //  return new JavaScriptSerializer().Serialize(response);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
          
            Context.Response.Write(js.Serialize(response));


        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAppointmentList(string appointmentDate, string companyId,string userId)
        {

            var response = new List<Appointment>();
            AppointmentProcessor appointmentProcessorProcessor = new AppointmentProcessor();
            response = appointmentProcessorProcessor.GetAllAppointments(appointmentDate, companyId,userId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));


        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetCustomerList(string Date, string CompanyId)
        {
          //  string formattedDate = filterDate.ToString("yyyy/MM/dd");
            var response = new List<Customer>();
            CustomerProcessor customerProcessorProcessor = new CustomerProcessor();
            response = customerProcessorProcessor.GetAllCustomers(Date, CompanyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));


        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetInvoiceList(string Date, string CompanyId)
        {
            var response = new List<InvoiceDetails>();
            InvoiceProcessor invoiceProcessor = new InvoiceProcessor();
            response = invoiceProcessor.GetInvoiceDetailsList(Date, CompanyId);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
      
            Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AddCustomer(CustomerDTO customer)
        {
            try
            {
                CustomerProcessor customerProcessor = new CustomerProcessor();
                string result = customerProcessor.AddCustomer(customer);

                var response = new StringResult
                {
                    Status = result.StartsWith("Error:") ? "error" : "success",
                    Response = result
                };


                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/json";
                HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
                HttpContext.Current.Response.Write(js.Serialize(response));
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();

               // Context.Response.Write(js.Serialize(response));
            }
            catch (Exception ex)
            {
                var response = new StringResult
                {
                    Status = "Error",
                    Response = ex.Message
                };
                JavaScriptSerializer js = new JavaScriptSerializer();
                Context.Response.Clear();
                Context.Response.ContentType = "application/json";

                Context.Response.Write(js.Serialize(response));

            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetStatusList(string companyID)
        {
            var response = new List<Status>();
            AppointmentStatusProcessor AppointmentStatusProcessor = new AppointmentStatusProcessor();
            response = AppointmentStatusProcessor.GetAllAppointmentStatusList(companyID);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetTicketStatusList(string companyID)
        {
            var response = new List<TicketStatus>();
            TicketStatusProcessor TicketStatusProcessor = new TicketStatusProcessor();
            response = TicketStatusProcessor.GetAllTicketStatus(companyID);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetTaxList(string companyID)
        {
            var response = new List<Tax>();

            InvoiceProcessor processor = new InvoiceProcessor();
            response = processor.GetAllTaxes(companyID);

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateAppointment(AppointmentDTO appointment)
        {
            var response = "";
            AppointmentProcessor appointmentProcessor = new AppointmentProcessor();
            response = appointmentProcessor.UpdateAppointment(appointment);


            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();


           // Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAllItemList(string companyId)
        {
            var response =new List<Items>();
            ItemProcessor processor = new ItemProcessor();
            response = processor.GetAllItems(companyId);
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void TEST(string invoice)
        {
            Response response = new Response();
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();
            bool Issuccess = false;
          //  response.Message = invoiceProccessor.CreateInvoice(invoice, ref Issuccess);
            response.IsValid = Issuccess;




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

           

            // Context.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Write("{property: value}");
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CreateInvoice(InvoiceDTO invoice)
        {
            Response response = new Response();
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();
            bool Issuccess = false;
            string id = string.Empty;
            response.Message = invoiceProccessor.CreateInvoice(invoice,
                ref Issuccess,
                ref id);
            response.IsValid = Issuccess;
            response.Id = id;




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

           // Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SendHtmlFormattedEmail(string CompanyID, string CustomerID, string EmailType, string subject, string body,
               string recepientToEmail, string recepientCCEmail, string recepientBCCEmail, List<EmailContent> emailContents, string UserId)
        {
            Response response = new Response();
            EmailProcessor invoiceProccessor = new EmailProcessor();
            bool Issuccess = false;
            response.Message = invoiceProccessor.SendHtmlFormattedEmail(CompanyID, CustomerID, EmailType, subject, body, recepientToEmail, recepientCCEmail, recepientBCCEmail, emailContents, UserId);
            response.IsValid = Issuccess;

            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AddPayment(PaymentDTO payment)
        {
            Response response = new Response();
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();
            bool Issuccess = false;
            response.Message = invoiceProccessor.Addpayment(payment, ref Issuccess);
            response.IsValid = Issuccess;




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

            // Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void EditInvoice(InvoiceEditDTO invoice)
        {
            Response response = new Response();
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();
            bool Issuccess = false;
            response.Message = invoiceProccessor.EditInvoice(invoice, ref Issuccess);
            response.IsValid = Issuccess;




            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.AddHeader("content-length", js.Serialize(response).Length.ToString());
            HttpContext.Current.Response.Write(js.Serialize(response));
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

            // Context.Response.Write(js.Serialize(response));
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetAutoGeneratedInvoice(string companyId, bool IsInvoice)
        {
            InvoiceProcessor invoiceProccessor = new InvoiceProcessor();

            string invoiceNo = invoiceProccessor.AutoGeneratedInvoiceNo(companyId, IsInvoice);
            var response = new
            {
                InvoiceNo = invoiceNo
            };
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetXPayLink(string RMCompanyID, string CustomerID, string InvoiceNo, string CustomerName, string email, string amount)
        {
           
            XPayLinkProcessor xPayLinkProcessor = new XPayLinkProcessor();
            string link = xPayLinkProcessor.GetCSPaymentLink(RMCompanyID,CustomerID,InvoiceNo,CustomerName, email,amount);
            var response = new
            {
                XPayLink = link
            };
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";

            Context.Response.Write(js.Serialize(response));
        }
    }
}
