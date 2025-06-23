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
    public class CustomerProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();

        public List<Customer> GetAllCustomers(string filterDate,string companyId)
        {
            try
            {
                string sql = @"
            SELECT 
                CompanyID, CreatedCompanyID, CustomerID, AMCustomerID, CustomerGuid, Title, 
                FirstName, LastName,  JobTitle,  Address1,  
                City, State, ZipCode, Phone, Mobile, Email, Notes,  
                QboId, IsPrimaryContact, BusinessID, SyncToken, BusinessName, 
                IsBusinessContact, CompanyName, DealerID, IsDealer, CustomerCode, UpdateDate 
            FROM [msSchedulerV3].[dbo].[tbl_Customer] 
            WHERE CompanyID = @CompanyID AND convert(varchar, CreatedDateTime, 111)  <='" + filterDate + "';";

                Database db = new Database();
                DataSet dataSet = db.Get_DataSet(sql, companyId);
                DataTable dt = dataSet.Tables[0];

                List<Customer> customers = new List<Customer>();

                foreach (DataRow dr in dt.Rows)
                {
                    
                    Customer customer = new Customer
                    {
                        CompanyID = dr["CompanyID"].ToString(),
                        CustomerID = dr["CustomerID"].ToString(),
                        AMCustomerID = dr["AMCustomerID"].ToString(),
                        CustomerGuid = dr["CustomerGuid"].ToString(),
                        Title = dr["Title"].ToString(),
                         FirstName = dr["FirstName"].ToString(),
                       
                        LastName = dr["LastName"].ToString(),
                      
                        JobTitle = dr["JobTitle"].ToString(),
                      
                        Address1 = dr["Address1"].ToString(),
                      
                        City = dr["City"].ToString(),
                        State = dr["State"].ToString(),
                        ZipCode = dr["ZipCode"].ToString(),
                        Phone = dr["Phone"].ToString(),
                        Mobile = dr["Mobile"].ToString(),
                        Email = dr["Email"].ToString(),
                        Notes = dr["Notes"].ToString(),
                        QboId = Convert.ToInt32(dr["QboId"]),
                        IsPrimaryContact = dr["IsPrimaryContact"] != DBNull.Value && Convert.ToBoolean(dr["IsPrimaryContact"]),
                        BusinessID = Convert.ToInt32(dr["BusinessID"]),
                        SyncToken = Convert.ToInt32(dr["SyncToken"]),
                        BusinessName = dr["BusinessName"].ToString(),
                        IsBusinessContact = dr["IsBusinessContact"] != DBNull.Value && Convert.ToBoolean(dr["IsBusinessContact"]),
                        CompanyName = dr["CompanyName"].ToString(),
                        DealerID = dr["DealerID"].ToString(),
                        IsDealer = dr["IsDealer"] != DBNull.Value && Convert.ToBoolean(dr["IsDealer"]),
                        CustomerCode = dr["CustomerCode"].ToString(),
                        UpdateDate = dr["UpdateDate"] != DBNull.Value ? Convert.ToDateTime(dr["UpdateDate"].ToString()).ToString("yyyy/MM/dd hh:mm tt") : ""

                    };

                    customers.Add(customer);
                }

                return customers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string AddCustomer(CustomerDTO customer)
        {
            string response = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                string customerGuid = Guid.NewGuid().ToString().ToUpper();
                string CustomerID = @"(Select IsNull(MAX(CustomerID),0) +1 as NewCustomerID from msSchedulerV3.dbo.tbl_Customer where CompanyID = @CompanyID)";
                string query = @"
                INSERT INTO [msSchedulerV3].[dbo].[tbl_Customer] 
                (   
                    
                    [CustomerGuid],
                    [CustomerID],
                    [CreatedCompanyID],
                    [BusinessName],
                    [IsBusinessContact],
                    [Title],
                    [FirstName],
                    [LastName],
                    [JobTitle],
                    [CompanyName],
                    [Address1],
                    [Address2],
                    [City],
                    [State],
                    [ZipCode],
                    [Phone],
                    [Mobile],
                    [Email],
                    [CreatedDateTime],
                    [Notes],
                    [CustomerCode],
                    [CompanyID]
                )
                VALUES
                (
                    @CustomerGuid," +
                    CustomerID +
                    @",
                    @CreatedCompanyID,
                    @BusinessName,
                    @IsBusinessContact,
                    @Title,
                    @FirstName,
                    @LastName,
                    @JobTitle,
                    @CompanyName,
                    @Address1,
                    @Address2,
                    @City,
                    @State,
                    @ZipCode,
                    @Phone,
                    @Mobile,
                    @Email,
                    GETDATE(),
                    @Notes,
                    @CustomerCode,
                    @CompanyID
                )";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@CustomerGuid", customerGuid);
                        command.Parameters.AddWithValue("@CreatedCompanyID", customer.CompanyID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@BusinessName", customer.BussinessName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsBusinessContact", customer.IsBussinessContact);
                        command.Parameters.AddWithValue("@Title", customer.Title ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@FirstName", customer.FirstName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@LastName", customer.LastName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@JobTitle", customer.JobTitle ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CompanyName", customer.CompanyName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Address1", customer.Address1 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Address2", customer.Address2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@City", customer.City ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@State", customer.State ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ZipCode", customer.ZipCode ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Phone", customer.Phone ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Mobile", customer.Mobile ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Email", customer.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Notes", customer.Notes ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CustomerCode", customer.CustomerCode ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CompanyID", customer.CompanyID ?? (object)DBNull.Value);

                        connection.Open();
                        int insertionStatus  = command.ExecuteNonQuery();
                        if(insertionStatus == 1)
                        {
                            response = customerGuid + " Added Successfully";
                        }
                        else
                        {
                            response = "There is an error adding customer.";
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
}