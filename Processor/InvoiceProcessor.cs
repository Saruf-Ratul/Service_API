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
    public class InvoiceProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        public List<InvoiceDetails> GetInvoiceDetailsList(string invoiceDate, string companyId)
        {
            try
            {
               
                Database db = new Database();
                DataTable dt = new DataTable();
                
                string sql = @" SELECT TI.id as InvoiceID,TC.CustomerGuid,TC.FirstName + ' ' +TC.LastName as FullName,        
						 TC.qboid as QBOCustomerId,
						 TI.CustomerId, isnull(TI.DepositAmount,0.00) as DepositAmount,
						 TI.qboid, 
						 TC.City, TI.Number,TI.InvoiceDate as InvoiceDate,
						 TI.Subtotal, TI.[IsConverted],TI.[ConvertedInvocieID],
						 (TI.Total-TI.AmountCollect)as Due,TI.InvoiceDate,TI.AmountCollect,
						 TI.Discount, TI.Total, TI.Tax, TI.Status,TI.Type, TI.Note
						 FROM            
						 [msSchedulerV3].dbo.tbl_Customer as TC JOIN
						 [msSchedulerV3].dbo.tbl_Invoice as TI ON TC.CustomerID = TI.CustomerId
						 AND TC.CompanyID = TI.CompnyID
						 where  TI.[CompnyID] = @CompanyID AND convert(varchar, InvoiceDate, 111) <= '" + invoiceDate + "' ";
                DataSet dataSet = db.Get_DataSet(sql, companyId);
                dt = dataSet.Tables[0];
                List<InvoiceDetails> invoiceDetailsList = new List<InvoiceDetails>();
             
                foreach (DataRow dr in dt.Rows)
                {
                    Invoice invoice = new Invoice();


                    invoice.InvoiceID = dr["InvoiceID"].ToString();
                    invoice.CustomerGuid = Guid.Parse(dr["CustomerGuid"].ToString());
                    invoice.FullName = dr["FullName"].ToString();
                    invoice.QBOCustomerId = dr["QBOCustomerId"]?.ToString();
                    invoice.CustomerId = dr["CustomerId"].ToString();
                    invoice.DepositAmount = Convert.ToDecimal(dr["DepositAmount"]);
                    invoice.QBOId = dr["QBOId"]?.ToString();
                    invoice.City = dr["City"]?.ToString();
                    invoice.Number = dr["Number"].ToString();
                    invoice.InvoiceDate = Convert.ToDateTime(dr["InvoiceDate"]).ToString("yyyy/MM/dd");
                    invoice.Subtotal = Convert.ToDecimal(dr["Subtotal"]);
                    invoice.IsConverted = dr["IsConverted"] != DBNull.Value && Convert.ToBoolean(dr["IsConverted"]);
                    invoice.ConvertedInvoiceID = dr["ConvertedInvocieID"].ToString();
                    invoice.Due = Convert.ToDecimal(dr["Due"]);
                    invoice.AmountCollect = Convert.ToDecimal(dr["AmountCollect"]);
                    invoice.Discount = Convert.ToDecimal(dr["Discount"]);
                    invoice.Total = Convert.ToDecimal(dr["Total"]);
                    invoice.Tax = Convert.ToDecimal(dr["Tax"]);
                    invoice.Surcharge = 0.0;
                    invoice.Note = dr["Note"].ToString();
                    if ((invoice.Total-invoice.AmountCollect) <= 0)
                    {
                        invoice.Status = "Paid";
                    }
                    else
                    {
                        invoice.Status = "Unpaid";
                    }
                    //invoice.Status = dr["Status"].ToString();
                    invoice.Type = dr["Type"].ToString();


                    InvoiceDetails invoiceDetails = new InvoiceDetails
                    {
                        InvoiceID = invoice.InvoiceID,
                        CustomerGuid = invoice.CustomerGuid,
                        FullName = invoice.FullName,
                        InvoiceDate = invoice.InvoiceDate,
                        Number = invoice.Number,
                        Type = invoice.Type,
                        Total = invoice.Total,
                        Status = invoice.Status,
                        Invoice = invoice,
                        Items = GetAllItemList(dr["InvoiceID"].ToString(), dr["CustomerId"].ToString(), companyId),
                    };
                    

                    invoiceDetailsList.Add(invoiceDetails);
                }

                return invoiceDetailsList;
            }
            catch
            {
                throw;
            }
        }
        public List<Item> GetAllItemList(string InvoiceNo, string CustomerID, string companyId)
        {
             var list = new List<Item>();
            Database db = new Database();
            DataTable dt = new DataTable();
            string SQL = "SELECT inv.*, inv.ItemId AS ID, i.* FROM msSchedulerV3.dbo.Items as i INNER JOIN " +
                "             msSchedulerV3.dbo.tbl_InvoiceDetails as inv ON i.Id = inv.ItemId and i.CompanyID = inv.CompanyID Where inv.RefId='" + InvoiceNo + "' and  inv.CompanyID =@CompanyID order by  CAST(NULLIF(inv.LineNum,'') AS INT) asc;";

            DataSet dataSet = db.Get_DataSet(SQL, companyId);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                return list; 
            }

            dt = dataSet.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var item = new Item
                {
                    Id = dr["ID"].ToString(),
                    Name = dr["Name"].ToString(),
                    Description = dr["Description"]?.ToString() ?? "",
                    Barcode = dr["Barcode"]?.ToString(),
                    ItemTypeId = Convert.ToInt32(dr["ItemTypeId"]),
                    Price = Convert.ToDecimal(dr["Price"]),
                    Location = dr["Location"]?.ToString(),
                    IsTaxable = dr["IsTaxable"].ToString(),
                    CompanyId = dr["CompanyId"]?.ToString(),
                    QboId = dr["QboId"] != DBNull.Value ? Convert.ToInt64(dr["QboId"]) : (long?)null
                };

                list.Add(item);
            }

            return list;
        }


        public List<Tax> GetAllTaxes(string companyId)
        {
            var list = new List<Tax>();
            Database db = new Database();
            DataTable dt = new DataTable();

            string SQL = "SELECT * FROM msSchedulerV3.dbo.taxes WHERE CompanyId = @CompanyID AND IsDeleted = 0";

            DataSet dataSet = db.Get_DataSet(SQL, companyId);
            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                return list;
            }

            dt = dataSet.Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                var tax = new Tax
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    Name = dr["Name"].ToString(),
                    Rate = Convert.ToDouble(dr["Rate"])
                };

                list.Add(tax);
            }

            return list;
        }

        public string CreateInvoice(InvoiceDTO invoice,
            ref bool IsSuccess,
            ref string id)
        {
            string response = "";
            string _InvocieID = Guid.NewGuid().ToString().ToUpper();
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    string query = @"
                INSERT INTO  msSchedulerV3.[dbo].[tbl_Invoice] 
                (
                    [ID],
                    [Number],
                    [CompanyID],
                    [CompnyID],
                    [DisplayNumber],
                    [CustomerId],
                    [UserId],
                    [Subtotal],
                    [Discount],
                    [Tax],
                    [Total],
                    [Status],
                    [InvoiceType],
                    [ModifiedDate],
                    [ModifiedBy],
                    [Note],
                    [CreatedDate],
                    [CreatedBy],
                    [InvoiceDate],
                    [AmountCollect],
                    [TaxType],
                    [AppointmentId],
                    [Type],
                    [QboId],
                    [DiscountRate],
                    [DiscountOption],
                    [QboEstimateId],
                    [ExpirationDate],
                    [SyncToken],
                    [QboPaymentID],
                    [DepositAmount],
                    [LoanStatus],
                    [IsConverted],
                    [ConvertedInvocieID],
                    [ConvertedInvocieNumber]
                )
                VALUES 
                (
                    @ID,
                    @Number,
                    @CompanyID,
                    @CompnyID,
                    @DisplayNumber,
                    @CustomerId,
                    @UserId,
                    @Subtotal,
                    @Discount,
                    @Tax,
                    @Total,
                    @Status,
                    @InvoiceType,
                    @ModifiedDate,
                    @ModifiedBy,
                    @Note,
                    @CreatedDate,
                    @CreatedBy,
                    @InvoiceDate,
                    @AmountCollect,
                    @TaxType,
                    @AppointmentId,
                    @Type,
                    @QboId,
                    @DiscountRate,
                    @DiscountOption,
                    @QboEstimateId,
                    @ExpirationDate,
                    @SyncToken,
                    @QboPaymentID,
                    @DepositAmount,
                    @LoanStatus,
                    @IsConverted,
                    @ConvertedInvocieID,
                    @ConvertedInvocieNumber
                )";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@ID", _InvocieID);
                        command.Parameters.AddWithValue("@Number", invoice.Number ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CompanyID", invoice.CompanyID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CompnyID", invoice.CompnyID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DisplayNumber", invoice.DisplayNumber ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CustomerId", invoice.CustomerId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@UserId", invoice.UserId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Subtotal", invoice.Subtotal);
                        command.Parameters.AddWithValue("@Discount", invoice.Discount);
                        command.Parameters.AddWithValue("@Tax", invoice.Tax );
                        command.Parameters.AddWithValue("@Total", invoice.Total );
                        command.Parameters.AddWithValue("@Status", invoice.Status );
                        command.Parameters.AddWithValue("@InvoiceType", invoice.InvoiceType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ModifiedDate", invoice.ModifiedDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ModifiedBy", invoice.ModifiedBy ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Note", invoice.Note ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CreatedDate", invoice.CreatedDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CreatedBy", invoice.CreatedBy ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@AmountCollect", invoice.AmountCollect );
                        command.Parameters.AddWithValue("@TaxType", invoice.TaxType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@AppointmentId", invoice.AppointmentId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Type", invoice.Type ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@QboId", invoice.QboId );
                        command.Parameters.AddWithValue("@DiscountRate", invoice.DiscountRate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DiscountOption", invoice.DiscountOption ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@QboEstimateId", invoice.QboEstimateId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ExpirationDate", invoice.ExpirationDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@SyncToken", invoice.SyncToken ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@QboPaymentID", invoice.QboPaymentID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DepositAmount", invoice.DepositAmount );
                        command.Parameters.AddWithValue("@LoanStatus", invoice.LoanStatus ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsConverted", invoice.IsConverted);
                        command.Parameters.AddWithValue("@ConvertedInvocieID", invoice.ConvertedInvocieID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ConvertedInvocieNumber", invoice.ConvertedInvocieNumber ?? (object)DBNull.Value);

                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            response = "Invoice created successfully.";
                        }
                        else
                        {
                            response = "Failed to create invoice.";
                        }

                        int lineNumber = 0;
                        foreach (InvoiceItem invoiceItem in invoice.items)
                        {
                            query = @" insert into msSchedulerV3.[dbo].[tbl_InvoiceDetails]
                                (companyid,RefId, ItemId,LineNum, ItemName, Description,ServiceDate, Quantity, uPrice , TotalPrice,  IsTaxable, ItemTyId,CreatedDate, CreatedBy)
                                    values (@companyid,@RefId, @ItemId,@LineNum, @ItemName, @Description,@ServiceDate, @Quantity, @uPrice , @TotalPrice,  @IsTaxable, @ItemTyId,@CreatedDate, @CreatedBy)";
                            command.CommandText = query;
                            command.Parameters.Clear();
                            lineNumber += 1;
                            command.Parameters.AddWithValue("@CompanyID", invoice.CompnyID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RefId", _InvocieID);
                            command.Parameters.AddWithValue("@ItemId", invoiceItem.ItemId ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@LineNum", lineNumber);
                            command.Parameters.AddWithValue("@ItemName", invoiceItem.Name);
                            command.Parameters.AddWithValue("@Description", invoiceItem.Description);
                            command.Parameters.AddWithValue("@ServiceDate", DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));
                            command.Parameters.AddWithValue("@Quantity", invoiceItem.Quantity);
                            command.Parameters.AddWithValue("@uPrice", invoiceItem.UnitPrice);
                            command.Parameters.AddWithValue("@TotalPrice", invoiceItem.TotalPrice);
                            command.Parameters.AddWithValue("@IsTaxable", invoiceItem.IsTaxable );
                            command.Parameters.AddWithValue("@ItemTyId", invoiceItem.ItemTyId ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));
                            command.Parameters.AddWithValue("@CreatedBy", "Xinator BMS");
                          

                          
                           command.ExecuteNonQuery();

                        }
                        


                       
                        connection.Close();
                        id = _InvocieID;

                        IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
            }
            return response;
        }




        public string Addpayment(PaymentDTO paymentDTO, ref bool IsSuccess)
        {
            string response = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {

                    connection.Open();

                    string UpdateInvocie = @"Update  msSchedulerV3.[dbo].[tbl_Invoice] set DepositAmount = isnull(DepositAmount, 0) + " + paymentDTO.Amount + ", AmountCollect = isnull(AmountCollect, 0) + " + paymentDTO.Amount +
                            " where CompnyID='" + paymentDTO.CompanyID + "' And ID='" + paymentDTO.InvocieId + "';";
                    using (SqlCommand checkCommand = new SqlCommand(UpdateInvocie, connection))
                    {
                        // checkCommand.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                        int count = (int)checkCommand.ExecuteNonQuery();

                        if (count == 0)
                        {
                            return "Invoice not found.";
                        }
                    }


                    string updateQuery = @"
                INSERT INTO msSchedulerV3.[dbo].[tbl_Payment] 
                            ([Companyid]
                           ,[InvocieId]
                           ,[Amount]
                           ,[Type]
                           ,[Source]
                           ,CheckName
                           ,IsDeposit
                           ,CheckNumber
                           ,[QboId])
                            values  (@Companyid
                           ,@InvocieId
                           ,@Amount
                           ,@Type
                           ,@Source
                           ,@CheckName
                           ,@IsDeposit
                           ,@CheckNumber
                           ,@QboId);";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {

                        command.Parameters.AddWithValue("@Companyid", paymentDTO.CompanyID);
                        command.Parameters.AddWithValue("@InvocieId", paymentDTO.InvocieId);
                        command.Parameters.AddWithValue("@Amount", paymentDTO.Amount);
                        command.Parameters.AddWithValue("@Type", paymentDTO.Type);
                        command.Parameters.AddWithValue("@Source", paymentDTO.Source);
                        command.Parameters.AddWithValue("@CheckName", paymentDTO.CheckName);
                        command.Parameters.AddWithValue("@CheckNumber", paymentDTO.CheckNumber);
                        
                        command.Parameters.AddWithValue("@IsDeposit", "0");
                        command.Parameters.AddWithValue("@QboId", 0);


                        int result = command.ExecuteNonQuery();
                        response = result > 0 ? "Payment Added successfully." : "Failed to update invoice.";
                        IsSuccess = true;
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
            }
            return response;
        }
        public string EditInvoice(InvoiceEditDTO invoice, ref bool IsSuccess)
        {
            string response = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connStr))
                {
                    connection.Open();

     
                    string checkQuery = "SELECT COUNT(1) FROM msSchedulerV3.[dbo].[tbl_Invoice] WHERE  id = @InvoiceID and compnyId = '" + invoice.CompnyID+"'";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count == 0)
                        {
                            return "Invoice not found.";
                        }
                    }

           
                    string updateQuery = @"
                UPDATE msSchedulerV3.[dbo].[tbl_Invoice] 
                SET 
                    [Number]= @Number,
                    [DisplayNumber] = @DisplayNumber,
                    [Subtotal] = @Subtotal,
                    [Discount] = @Discount,
                    [Tax] = @Tax,
                    [Total] = @Total,
                    [Status] = @Status,
                    [InvoiceType] = @InvoiceType,
                    [ModifiedDate] = @ModifiedDate,
                    [ModifiedBy] = @ModifiedBy,
                    [Note] = @Note,
                    [InvoiceDate] = @InvoiceDate,
                    [AmountCollect] = @AmountCollect,
                    [TaxType] = @TaxType,
                    [Type] = @Type,
                    [DiscountRate] = @DiscountRate,
                    [DiscountOption] = @DiscountOption,
                    [ExpirationDate] = @ExpirationDate,
                    [DepositAmount] = @DepositAmount,
                    [LoanStatus] = @LoanStatus,
                    [IsConverted] = @IsConverted,
                    [ConvertedInvocieID] = @ConvertedInvocieID,
                    [ConvertedInvocieNumber] = @ConvertedInvocieNumber
                WHERE id = @InvoiceID;
Delete From msSchedulerV3.[dbo].[tbl_InvoiceDetails] where companyid=@Companyid and RefId=@RefId;";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Number", invoice.Number ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DisplayNumber", invoice.DisplayNumber ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Subtotal", invoice.Subtotal);
                        command.Parameters.AddWithValue("@Discount", invoice.Discount);
                        command.Parameters.AddWithValue("@Tax", invoice.Tax);
                        command.Parameters.AddWithValue("@Total", invoice.Total);
                        command.Parameters.AddWithValue("@Status", invoice.Status);
                        command.Parameters.AddWithValue("@InvoiceType", invoice.InvoiceType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ModifiedDate", invoice.ModifiedDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ModifiedBy", invoice.ModifiedBy ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Note", invoice.Note ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@AmountCollect", invoice.AmountCollect);
                        command.Parameters.AddWithValue("@TaxType", invoice.TaxType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Type", invoice.Type ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DiscountRate", invoice.DiscountRate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DiscountOption", invoice.DiscountOption ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ExpirationDate", invoice.ExpirationDate ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@DepositAmount", invoice.DepositAmount);
                        command.Parameters.AddWithValue("@LoanStatus", invoice.LoanStatus ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IsConverted", invoice.IsConverted);
                        command.Parameters.AddWithValue("@ConvertedInvocieID", invoice.ConvertedInvocieID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ConvertedInvocieNumber", invoice.ConvertedInvocieNumber ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
                        command.Parameters.AddWithValue("@CompanyID", invoice.CompnyID ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@RefId", invoice.InvoiceID );

                        int result = command.ExecuteNonQuery();
                        response = result > 0 ? "Invoice updated successfully." : "Failed to update invoice.";
                        int lineNumber = 0;
                        foreach (InvoiceItem invoiceItem in invoice.items)
                        {
                            updateQuery = @" insert into msSchedulerV3.[dbo].[tbl_InvoiceDetails]
                                (companyid,RefId, ItemId,LineNum, ItemName, Description,ServiceDate, Quantity, uPrice , TotalPrice,  IsTaxable, ItemTyId,CreatedDate, CreatedBy)
                                    values (@companyid,@RefId, @ItemId,@LineNum, @ItemName, @Description,@ServiceDate, @Quantity, @uPrice , @TotalPrice,  @IsTaxable, @ItemTyId,@CreatedDate, @CreatedBy)";
                            command.CommandText = updateQuery;
                            command.Parameters.Clear();
                            lineNumber += 1;
                            command.Parameters.AddWithValue("@CompanyID", invoice.CompnyID ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@RefId", invoice.InvoiceID);
                            command.Parameters.AddWithValue("@ItemId", invoiceItem.ItemId ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@LineNum", lineNumber);
                            command.Parameters.AddWithValue("@ItemName", invoiceItem.Name);
                            command.Parameters.AddWithValue("@Description", invoiceItem.Description);
                            command.Parameters.AddWithValue("@ServiceDate", DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));
                            command.Parameters.AddWithValue("@Quantity", invoiceItem.Quantity);
                            command.Parameters.AddWithValue("@uPrice", invoiceItem.UnitPrice);
                            command.Parameters.AddWithValue("@TotalPrice", invoiceItem.TotalPrice);
                            command.Parameters.AddWithValue("@IsTaxable", invoiceItem.IsTaxable);
                            command.Parameters.AddWithValue("@ItemTyId", invoiceItem.ItemTyId ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));
                            command.Parameters.AddWithValue("@CreatedBy", "Xinator BMS");



                            command.ExecuteNonQuery();

                        }




                        connection.Close();

                        IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response = "Error: " + ex.Message;
            }
            return response;
        }

        public string AutoGeneratedInvoiceNo(string companyId, bool isInvoice)
        {
            string newNumber = string.Empty;
            Database db = new Database();
            DataTable table = new DataTable();

            // Query to get current invoice/estimate number and prefix
            string sql = @"
        SELECT [InvoicePrefix], [InvoiceNumberSeed], [EstimatePrefix], [EstimateNumberSeed]
        FROM [msSchedulerV3].[dbo].[tbl_Company]
        WHERE CompanyID = @CompanyID";

            DataSet dataSet = db.Get_DataSet(sql, companyId);

            if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                return newNumber; // Return empty if no data found
            }

            table = dataSet.Tables[0];

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                if (isInvoice)
                {
                    long invoiceNumberSeed = Convert.ToInt64(row["InvoiceNumberSeed"]) + 1;
                    string invoicePrefix = row["InvoicePrefix"].ToString();

                    if (invoiceNumberSeed < 10001)
                    {
                        invoiceNumberSeed = 10001; // Start from 10001 if it's lower
                    }

                    newNumber = $"{invoicePrefix}-{companyId}-{invoiceNumberSeed}";

                    // Update the new InvoiceNumberSeed back to the database
                    string updateSql = "UPDATE [msSchedulerV3].[dbo].[tbl_Company] SET InvoiceNumberSeed = '" + invoiceNumberSeed + "' WHERE CompanyID = '" + companyId + "'";

                    db.Execute(updateSql);
                }
                else
                {
                    long estimateNumberSeed = Convert.ToInt64(row["EstimateNumberSeed"]) + 1;
                    string estimatePrefix = row["EstimatePrefix"].ToString();

                    if (estimateNumberSeed < 10001)
                    {
                        estimateNumberSeed = 10001; // Start from 10001 if it's lower
                    }

                    newNumber = $"{estimatePrefix}-{companyId}-{estimateNumberSeed}";

                    string updateSql = " UPDATE [msSchedulerV3].[dbo].[tbl_Company] SET EstimateNumberSeed = '" + estimateNumberSeed + "' WHERE CompanyID = '" + companyId + "'";

                    db.Execute(updateSql);
                }
            }

            return newNumber;
        }
    }
}

    
