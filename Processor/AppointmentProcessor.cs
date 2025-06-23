using Services.Entity;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Services.Processor
{
    public class AppointmentProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        public List<Appointment> GetAllAppointments(string appointmentDate, string companyId, string userId)
        {
            List<Appointment> appointments = new List<Appointment>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string query = @"
            WITH ResourceCTE AS (
                SELECT id 
                FROM [msSchedulerV3].[dbo].tbl_Resources 
                WHERE UserID = @UserID
            )
            SELECT DISTINCT
                a.CompanyID, a.ApptID, a.AppoinmentUId, a.CustomerID, a.ServiceType, a.ResourceID, 
                a.TimeSlotId, a.ApptDateTime, a.StartDateTime, a.EndDateTime, a.TimeSlot, a.Note, 
                a.Status, a.TicketStatus, a.CreatedDateTime AS AppointmentCreatedDateTime, 
                a.MarkDownloaded, a.PromoCode, a.CreatedBy, a.UserID, 
                c.CreatedCompanyID, c.TagID, c.AMCustomerID, c.CustomerGuid, c.Title, c.Title2, 
                c.FirstName, c.FirstName2, c.LastName, c.LastName2, c.JobTitle, c.JobTitle2, 
                c.Address1, c.Address2, c.City, c.State, c.ZipCode, c.Phone, c.Mobile, c.Email, 
                c.Notes AS CustomerNotes, c.CreatedDateTime AS CustomerCreatedDateTime, 
                c.CallPopUploaded, c.CallPopAppId, c.IsPrimaryContact, c.BusinessID, 
                c.SyncToken, c.BusinessName, c.IsBusinessContact, c.CompanyName, c.CompanyName2,
                r.Name AS ResourceName, s.StatusName AS StatusName, st.ServiceName ,  s.StatusID , 
				st.ServiceTypeID , r.Id AS ResourceId,ts.StatusID as TicketStatusId, ts.StatusName as TicketStatusName
            FROM [msSchedulerV3].[dbo].tbl_Appointment AS a   
            INNER JOIN [msSchedulerV3].[dbo].tbl_Customer AS c 
                ON a.CustomerID = c.CustomerID AND a.CompanyID = c.CompanyID 
            INNER JOIN [msSchedulerV3].[dbo].tbl_Resources AS r 
                ON a.ResourceID = r.Id AND a.CompanyID = r.CompanyID  
            INNER JOIN [msSchedulerV3].[dbo].tbl_Status AS s 
                ON a.Status = s.StatusID AND a.CompanyID = s.CompanyID  
            INNER JOIN [msSchedulerV3].[dbo].tbl_ServiceType AS st 
                ON a.ServiceType = st.ServiceTypeID AND a.CompanyID = st.CompanyID
		LEFT JOIN [msSchedulerV3].[dbo].tbl_TicketStatus AS ts 
    ON a.TicketStatus = ts.StatusID and a.CompanyID = ts.CompanyID
            WHERE a.CompanyID = @CompanyID 
                AND a.Status != 'Deleted'  
                AND s.StatusName NOT IN ('Cancelled', 'Closed','Pending')  
                AND a.ResourceID IS NOT NULL 
                AND a.ResourceID IN (SELECT id FROM ResourceCTE)
                AND CONVERT(VARCHAR, a.CreatedDateTime, 111) <= @AppointmentDate";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.Parameters.AddWithValue("@CompanyID", companyId);
                        command.Parameters.AddWithValue("@AppointmentDate", appointmentDate);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                appointments.Add(new Appointment
                                {
                                    CompanyID = reader["CompanyID"].ToString(),
                                    ApptID = Convert.ToInt32(reader["ApptID"]),
                                    AppoinmentUId = reader["AppoinmentUId"].ToString(),
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    ServiceTypeId = reader["ServiceType"].ToString(),
                                    ResourceID = reader["ResourceID"] != DBNull.Value ? Convert.ToInt32(reader["ResourceID"]) : (int?)null,
                                    TimeSlotId = reader["TimeSlotId"] != DBNull.Value ? Convert.ToInt32(reader["TimeSlotId"]) : (int?)null,
                                    ApptDateTime = reader["ApptDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["ApptDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    StartDateTime = reader["StartDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["StartDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    EndDateTime = reader["EndDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["EndDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    TimeSlot = reader["TimeSlot"].ToString(),
                                    Note = reader["Note"].ToString(),
                                    StatusId = reader["Status"].ToString(),
                                    TicketStatusId = reader["TicketStatus"].ToString(),
                                    CreatedDateTime = reader["AppointmentCreatedDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["AppointmentCreatedDateTime"]).ToString("yyyy/MM/dd hh:mm tt") : "",
                                    MarkDownloaded = reader["MarkDownloaded"] != DBNull.Value && Convert.ToBoolean(reader["MarkDownloaded"]),
                                    PromoCode = reader["PromoCode"].ToString(),
                                    CreatedBy = reader["CreatedBy"].ToString(),
                                    UserID = reader["UserID"].ToString(),

                                    Customer = new Customer
                                    {
                                        CompanyID = reader["CompanyID"].ToString(),
                                        CreatedCompanyID = reader["CreatedCompanyID"].ToString(),
                                        TagID = Convert.ToInt32(reader["TagID"]),
                                        CustomerID = reader["CustomerID"].ToString(),
                                        AMCustomerID = reader["AMCustomerID"].ToString(),
                                        CustomerGuid = reader["CustomerGuid"].ToString(),
                                        Title = reader["Title"].ToString(),
                                        FirstName = reader["FirstName"].ToString(),
                                        LastName = reader["LastName"]?.ToString(),
                                        JobTitle = reader["JobTitle"].ToString(),
                                        Address1 = reader["Address1"].ToString(),
                                        Address2 = reader["Address2"]?.ToString(),
                                        City = reader["City"]?.ToString(),
                                        State = reader["State"]?.ToString(),
                                        ZipCode = reader["ZipCode"]?.ToString(),
                                        Phone = reader["Phone"].ToString(),
                                        Mobile = reader["Mobile"].ToString(),
                                        Email = reader["Email"].ToString(),
                                        Notes = reader["CustomerNotes"]?.ToString(),
                                        CreatedDateTime = reader["CustomerCreatedDateTime"]?.ToString(),
                                        CallPopUploaded = reader["CallPopUploaded"] != DBNull.Value && Convert.ToBoolean(reader["CallPopUploaded"]),
                                        BusinessName = reader["BusinessName"].ToString(),
                                        IsBusinessContact = reader["IsBusinessContact"] != DBNull.Value && Convert.ToBoolean(reader["IsBusinessContact"]),
                                        CompanyName = reader["CompanyName"].ToString(),
                                    },

                                    Resource = new Services.Entity.Resource
                                    {
                                        Id = Convert.ToInt32(reader["ResourceId"]),
                                        Name = reader["ResourceName"].ToString()
                                    },

                                    Status = new Status
                                    {
                                        StatusId = Convert.ToInt32(reader["StatusID"]),
                                        StatusName = reader["StatusName"].ToString()
                                    },
                                    TicketStatus = new TicketStatus
                                    {
                                        StatusId = reader["TicketStatusId"] != DBNull.Value ? Convert.ToInt32(reader["TicketStatusId"]) : 0, 
                                        StatusName = reader["TicketStatusName"] != DBNull.Value ? reader["TicketStatusName"].ToString() : string.Empty,
                                        CompanyId = reader["CompanyID"].ToString()
                                    },
                                ServiceType = new ServiceType
                                    {
                                        ServiceTypeID = Convert.ToInt32(reader["ServiceTypeID"]),
                                        ServiceName = reader["ServiceName"].ToString()
                                    },
                                    Invoices = GetInvoicesByAppointment(Convert.ToInt32(reader["ApptID"]), Convert.ToInt32(reader["CustomerID"]), reader["CompanyID"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching appointments: " + ex.Message);
            }
            return appointments;
        }

        public List<AppointmentInvoice> GetInvoicesByAppointment(int appointmentrId, int customerId, string companyId)
        {
            try
            {
                string sql = "";
                Database db = new Database();
                DataTable dt = new DataTable();
                List<AppointmentInvoice> invoices = new List<AppointmentInvoice>();
                sql = @"SELECT *,c.CustomerGuid,c.FirstName+ c.LastName as FullName,c.CustomerID,c.City
            FROM [msSchedulerV3].[dbo].tbl_invoice as i
			join [msSchedulerV3].[dbo].tbl_Customer as c on c.customerID = i.customerId and c.CompanyID =  @CompanyID
            WHERE AppointmentId != 0 AND AppointmentId = " + appointmentrId + " AND CompnyID = @CompanyID AND i.CustomerId = " + customerId + "";

                DataSet dataSet = db.Get_DataSet(sql, companyId);
                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                {
                    return invoices;
                }
                dt = dataSet.Tables[0];


                foreach (DataRow dr in dt.Rows)
                {

                    // FOR ITEMS
                    sql = @"select ItemId,ItemName,Description,Quantity,TotalPrice,uPrice,ServiceDate,ItemTyId,IsTaxable   from [msSchedulerV3].[dbo].tbl_InvoiceDetails where RefId = '" + dr["ID"].ToString() + "' and companyid =@CompanyID order by CAST(NULLIF(LineNum,'') AS INT) asc ;";

                    DataSet dataSet_Items = db.Get_DataSet(sql, companyId);
                   

                    List<InvoiceItem> items = new List<InvoiceItem>();
                    DataTable dt_InvoiceItems = dataSet_Items.Tables[0];

                    foreach (DataRow dr_Items in dt_InvoiceItems.Rows)
                    {
                        InvoiceItem item = new InvoiceItem
                        {
                            ItemId = dr_Items["ItemId"].ToString(),
                            Name = dr_Items["ItemName"].ToString(),
                            Description = dr_Items["Description"].ToString(),
                            Quantity = dr_Items["Quantity"].ToString(),
                            UnitPrice = dr_Items["uPrice"].ToString(),
                            IsTaxable = dr_Items["IsTaxable"].ToString(),
                            ItemTyId = dr_Items["ItemTyId"].ToString(),
                            
                            TotalPrice = dr_Items["TotalPrice"].ToString()

                        };
                        items.Add(item);
                    }



                    AppointmentInvoice invoice = new AppointmentInvoice
                    {

                        InvoiceID = dr["ID"].ToString(),
                        DepositAmount = dr["DepositAmount"] != DBNull.Value ? Convert.ToDecimal(dr["DepositAmount"]) : 0,

                        Number = dr["Number"] != DBNull.Value ? dr["Number"].ToString() : string.Empty,

                        InvoiceDate = dr["InvoiceDate"] != DBNull.Value
                            ? Convert.ToDateTime(dr["InvoiceDate"]).ToString("yyyy/MM/dd hh:mm tt")
                            : string.Empty,

                        Subtotal = dr["Subtotal"] != DBNull.Value ? Convert.ToDecimal(dr["Subtotal"]) : 0,
                        AmountCollect = dr["AmountCollect"] != DBNull.Value ? Convert.ToDecimal(dr["AmountCollect"]) : 0,
                        Discount = dr["Discount"] != DBNull.Value ? Convert.ToDecimal(dr["Discount"]) : 0,
                        Total = dr["Total"] != DBNull.Value ? Convert.ToDecimal(dr["Total"]) : 0,
                        Tax = dr["Tax"] != DBNull.Value ? Convert.ToDecimal(dr["Tax"]) : 0,

                        Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString() : string.Empty,
                        Type = dr["Type"] != DBNull.Value ? dr["Type"].ToString() : string.Empty,
                        Note = dr["Note"] != DBNull.Value ? dr["Note"].ToString() : string.Empty,
                        CustomerGuid = dr["CustomerGuid"].ToString(),
                        FullName = dr["FullName"].ToString(),
                        QBOCustomerId = "0",
                        CustomerId = dr["CustomerID"].ToString(),
                        QBOId ="0",
                        City =dr["City"].ToString(),
                        Due = "0",
                        IsConverted = Convert.ToBoolean(dr["isConverted"]),
                        ConvertedInvoiceID ="",
                        Surcharge = 0,
                        items = items




                    };

                    invoices.Add(invoice);
                }

                return invoices;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching invoices", ex);
            }


        }
        public string UpdateAppointment(AppointmentDTO appointment)
        {
            string response = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string query = @"UPDATE [msSchedulerV3].[dbo].[tbl_Appointment]
                            SET [CompanyID] = @CompanyID,
                                [ApptID] = @ApptID,
                                [AppoinmentUId] = @AppoinmentUId,
                                [CustomerID] = @CustomerID,
                                [ServiceType] = @ServiceType,
                                [ResourceID] = @ResourceID,
                                [TimeSlotId] = @TimeSlotId,
                                [ApptDateTime] = @ApptDateTime,
                                [StartDateTime] = @StartDateTime,
                                [EndDateTime] = @EndDateTime,
                                [TimeSlot] = @TimeSlot,
                                [Note] = @Note,
                                [Status] = @Status,
                                [TicketStatus] = @TicketStatus,
                                [PromoCode] = @PromoCode,
                                [UserID] = @UserID
                            WHERE ApptID = @ApptID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompanyID", appointment.CompanyID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ApptID", appointment.ApptID);
                        command.Parameters.AddWithValue("@AppoinmentUId", appointment.AppoinmentUId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CustomerID", appointment.CustomerID);
                        command.Parameters.AddWithValue("@ServiceType", appointment.ServiceTypeId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ResourceID", appointment.ResourceID);
                        command.Parameters.AddWithValue("@TimeSlotId", appointment.TimeSlotId.HasValue ? (object)appointment.TimeSlotId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@ApptDateTime", appointment.ApptDateTime);
                        command.Parameters.AddWithValue("@StartDateTime", appointment.StartDateTime);
                        command.Parameters.AddWithValue("@EndDateTime", appointment.EndDateTime);
                        command.Parameters.AddWithValue("@TimeSlot", appointment.TimeSlot ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Note", appointment.Note ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Status", appointment.StatusId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@TicketStatus", appointment.TicketStatusId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@PromoCode", appointment.PromoCode ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UserID", appointment.UserID ?? (object)DBNull.Value);

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        if (result != 0)
                        {
                            response = "Appointment updated successfully.";
                        }
                        else
                        {
                            response = "Error updating appointment.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
            }
            return response;
        }
    }
    

    public class AppointmentInvoice
    {
        public string InvoiceID { get; set; }
        public string CustomerGuid { get; set; }
        public string FullName { get; set; }
        public string QBOCustomerId { get; set; }
        public string CustomerId { get; set; }
        public decimal DepositAmount { get; set; }
        public string City { get; set; }
        public string QBOId { get; set; }
  
        public string Number { get; set; }
        public string InvoiceDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal AmountCollect { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public string Due { get; set; }
        public bool  IsConverted { get; set; }
        public string  ConvertedInvoiceID { get; set; }
        public decimal Surcharge { get; set; }
        public List<InvoiceItem> items { get; set; }


    }
}