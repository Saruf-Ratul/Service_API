using CECPro.Wisetack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace Services.Processor
{
    public class EmailProcessor
    {

        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        DataSet dataSet = null;
        public string SendHtmlFormattedEmail(string CompanyID, string CustomerID, string EmailType, string subject, string body,
                string recepientToEmail, string recepientCCEmail, string recepientBCCEmail, List<EmailContent> emailContents, string UserId)
        {

            string SendBy = UserId;
            string CompanyAddress = "";
            string CompanyCity = "";
            string CompanyState = "";
            string CompanyZipCode = "";
            string CompanyPhone = "";
            string CompanyEmail = "";
            string CompanyWebsite = "";
            string CompanyFacebook = "";
            string CompanyTwitter = "";
            string CompanyLogoFile = "";
            string CompanyFullName = "";
            string CompanyGUID = "";
            try
            {
                string wisetackFooter = "";

                Database db = new Database(connStr);

                string historyid = string.Empty;
                string EmailFrom = string.Empty;
                bool isSendPrequal = false;

                string prequalLink = string.Empty;


                string sql = "select * from msSchedulerV3.dbo.tbl_Company where CompanyID=@CompanyID;";

                sql += "Select [WisetackFooterMsg] from msSchedulerV3.dbo.tbl_CustCommunication where  CompanyID=@CompanyID;";

                sql += "Select ISNULL(Max(id), 0)+1 as newid from msSchedulerV3.dbo.tbl_EmailHistory;";

                sql += "Select EmailFrom from msSchedulerV3.dbo.tbl_CustCommunication where CompanyID = @CompanyID;";

                sql += "Select ISNULL(IsSendPrequal, 0) AS IsSendPrequal from msSchedulerV3.dbo.tbl_CustCommunication where  CompanyID=@CompanyID;";


                sql += "select ISNULL(PrequalLink, '') AS PrequalLink  From Wiseteck.dbo.tbl_MerchantSettings Where CompanyID =@CompanyID;";




                DataTable dt_Company = new DataTable();


                if (dataSet == null)
                {
                    dataSet = db.Get_DataSet(sql, CompanyID);
                }

                dt_Company = dataSet.Tables[0];

                wisetackFooter = dataSet.Tables[1].Rows.Count > 0 ? dataSet.Tables[1].Rows[0]["WisetackFooterMsg"].ToString() : "";
                historyid = dataSet.Tables[2].Rows.Count > 0 ? dataSet.Tables[2].Rows[0]["newid"].ToString() : "";
                EmailFrom = dataSet.Tables[3].Rows.Count > 0 ? dataSet.Tables[3].Rows[0]["EmailFrom"].ToString() : "";
                isSendPrequal = dataSet.Tables[4].Rows.Count > 0 ? Convert.ToBoolean(dataSet.Tables[4].Rows[0]["IsSendPrequal"].ToString()) : false;

                prequalLink = dataSet.Tables[5].Rows.Count > 0 ? dataSet.Tables[5].Rows[0]["PrequalLink"].ToString() : "";

                //db.Execute(sql, out dt_Company);

                if (dt_Company.Rows.Count > 0)
                {
                    DataRow Rs = dt_Company.Rows[0];
                    CompanyAddress = Rs["Address"].ToString();
                    CompanyCity = Rs["City"].ToString();
                    CompanyState = Rs["State"].ToString();
                    CompanyZipCode = Rs["ZipCode"].ToString();
                    CompanyPhone = Rs["Phone"].ToString();
                    CompanyEmail = Rs["Email"].ToString();
                    CompanyWebsite = Rs["Website"].ToString();
                    CompanyFacebook = Rs["Facebook"].ToString();
                    CompanyTwitter = Rs["Twitter"].ToString();
                    CompanyLogoFile = Rs["LogoFile"].ToString();
                    CompanyFullName = Rs["CompanyName"].ToString();
                    CompanyGUID = Rs["CompanyGUID"].ToString();

                }
                string Emailbody = body;

                if (!string.IsNullOrEmpty(body))
                {

                    StringBuilder builder = new StringBuilder(Emailbody);
                    builder.Replace("\r\n", "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
                    Emailbody = builder.ToString();

                }

                string LogoPath = HttpContext.Current.Server.MapPath("~/CompanyLogo/" + CompanyLogoFile);
                string header = "";

                string BodyText = "";
                BodyText = "<body><table style='max-width:800px; font-family:Arial;font-size:13px;'>" +
                 "<tr><td align='left'><table width='100%'><tr><td>" +
                 "<img src=\"cid:photo\"  id='img' alt='' style='max-height:60px;' height='60' /></td>" +
                 "<td align='center' style='color:#2980b9; vertical-align:bottom;'>" +
                 "</td></tr></table></td></tr>" +
                 "<tr><td align='left' style='padding-left:5px;'>" +
                 Emailbody +
                        "</td></tr>";


                BodyText = BodyText + "</table></td></tr></table></body>";
                string attachedfileNames = "";
                using (MailMessage mailMessage = new MailMessage())
                {


                    if (!string.IsNullOrEmpty(recepientToEmail))
                    {
                        string[] recepientToList = recepientToEmail.Split(',');
                        foreach (string multiple_email in recepientToList)
                        {
                            mailMessage.To.Add(new MailAddress(multiple_email));

                        }
                    }
                    if (!string.IsNullOrEmpty(recepientCCEmail))
                    {
                        string[] recepientCCList = recepientCCEmail.Split(',');
                        foreach (string multiple_email in recepientCCList)
                        {
                            mailMessage.CC.Add(new MailAddress(multiple_email));

                        }
                    }

                    if (!string.IsNullOrEmpty(recepientBCCEmail))
                    {
                        string[] recepientBCCList = recepientBCCEmail.Split(',');
                        foreach (string multiple_email in recepientBCCList)
                        {
                            mailMessage.Bcc.Add(multiple_email);

                        }
                    }

                    if (string.IsNullOrEmpty(EmailFrom) || EmailFrom == "0")
                    {
                        EmailFrom = "noreply@" + CompanyID + ".com";

                    }



                    if (!string.IsNullOrEmpty(prequalLink))
                    {
                        BodyText = BodyText.Replace("[Prequal]", prequalLink);
                    }
                    else
                    {
                        BodyText = BodyText.Replace("[Prequal]", "");
                    }
                    if (!string.IsNullOrEmpty(prequalLink))
                    {
                        BodyText = BodyText.Replace("[Prequal]", prequalLink);
                    }
                    if (isSendPrequal)
                    {
                        BodyText += "<br/><br/>Prequal Link: " + prequalLink;
                    }


                    mailMessage.From = new MailAddress(EmailFrom);
                    mailMessage.Subject = subject;
                    mailMessage.Body = BodyText;

                    if (!string.IsNullOrEmpty(CompanyLogoFile))
                    {

                        if (File.Exists(LogoPath))
                        {
                            AlternateView htmlview = default(AlternateView);
                            htmlview = AlternateView.CreateAlternateViewFromString(BodyText, null, "text/html");
                            LinkedResource imageResourceEs = new LinkedResource(LogoPath, MediaTypeNames.Image.Jpeg)
                            {
                                ContentId = "photo"
                            };
                
                            htmlview.LinkedResources.Add(imageResourceEs);
                            mailMessage.AlternateViews.Add(htmlview);
                        }
                        else
                        {
                            mailMessage.Body = BodyText;
                        }

                    }
                    else
                    {
                        mailMessage.Body = BodyText;
                    }
                    if (emailContents != null)
                    {
                        if (emailContents.Count > 0)
                        {
                            foreach (EmailContent f in emailContents)
                            {
                                Attachment attachment = new Attachment(HttpContext.Current.Server.MapPath(f.FileUrl)); //create the attachment
                                mailMessage.Attachments.Add(attachment); //add the attachment

                            }

                        }

                    }





          

                    mailMessage.IsBodyHtml = true;
                    string emailSentError = "";

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["SMTP"];
                    smtp.EnableSsl = true;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = ConfigurationManager.AppSettings["SmtpUser"];
                    NetworkCred.Password = ConfigurationManager.AppSettings["SmtpPassword"];
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                    smtp.Send(mailMessage);

                    string emailhistory = "INSERT INTO msSchedulerV3.dbo.tbl_EmailHistory(id,CompanyID, CustomerID, Subject, EmailBody, EmailTo, EmailCC, EmailBCC, EmailType, SendBy)VALUES" +
                         "(" +
                         "'" + historyid + "'," +
                         "'" + CompanyID + "'," +
                         "'" + CustomerID + "'," +
                         "'" + subject + "'," +
                         "'" + body.Replace("'", string.Empty) + "'," +
                         "'" + recepientToEmail + "'," +
                         "'" + recepientCCEmail + "'," +
                         "'" + recepientBCCEmail + "'," +
                         "'" + EmailType + "'," +
                         "'" + SendBy + "'" +

                         ")";

                    db.Execute(emailhistory);
                    if (emailContents != null)
                    {
                        if (emailContents.Count > 0)
                        {
                            foreach (EmailContent e in emailContents)
                            {
                                SaveFileContent(historyid, CompanyID, e.FileName, e.FileUrl);
                            }
                        }
                    }

                }

                return "Sent";



            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public void SaveFileContent(string historyid, string CompanyID, string filename, string FileUrl)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "insert into msSchedulerV3.dbo.tbl_EmailHistoryContent(HistoryID,CompanyID,FileName,FileUrl) values (@HistoryID,@CompanyID,@FileName,  @FileUrl)";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@HistoryID", historyid);
                    cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                    cmd.Parameters.AddWithValue("@FileName", filename);
                    cmd.Parameters.AddWithValue("@FileUrl", FileUrl);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
    }
    public class EmailCommunication
    {
        public string EmailTo { get; set; }
        public string StandardMailSubject { get; set; }
        public string StandardMailBody { get; set; }
        public string EmailBCC { get; set; }
        public string EmailCC { get; set; }
        public string ProposalMailSubject { get; set; }
        public string ProposalMailBody { get; set; }
        public string EmailConfirmText { get; set; }
        public string SMSConfirmText { get; set; }
        public string EmailAckText { get; set; }
        public string SMSAckText { get; set; }

        public string InvoiceMailSubject { get; set; }
        public string InvoiceMailBody { get; set; }
        public string AttachmentsName { get; set; }
        public string EmailType { get; set; }

        public List<EmailContent> EmailContents { get; set; }

    }
    public class EmailContent
    {
        public byte[] FileContent { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileUrl { get; set; }
    }
}