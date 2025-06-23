using ResponseEntity;
using Services.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace Services.Processor
{
    public class LoginProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        public Response VerifyUser(RequestEntity requestEntity)
        {
            Response _loginObject = new Response();

            string sql = "";
            Database db = new Database();
            DataTable dt = new DataTable();
          
                sql = "Select c.CompanyIdInt,c.TimeZone,c.CompanyID,c.CompanyGUID,c.CompanyName,c.CompanyTag, u.UserID,u.Password,u.email,u.FirstName,u.LastName,u.id " +
                     "From [XinatorCentral].[dbo].[tbl_User] u inner join msSchedulerV3.dbo.tbl_Company c " +
                     " on u.CompanyID= c.CompanyID " +
                     " where u.Password='" + requestEntity.Password + "'" +
                     " and u.UserID='" + requestEntity.UserName + "'";




                //sql += @"SELECT [ProductID] FROM [XinatorCentral].[dbo].[tbl_ProductsByCompany] where ProductID = '11' and ProductAccess=1 and CompanyID='" + CompanyID + "'";

                //sql += @"SELECT [CompanyType] FROM [XinatorCentral].dbo.tbl_Company where  CompanyID='" + CompanyID + "'";
                DataSet dataSet = db.Get_DataSet(sql, "");
                dt = dataSet.Tables[0];




            if (dt.Rows.Count > 0)
            {
                _loginObject.IsValid = true;
                DataRow dr = dt.Rows[0];
                _loginObject.UserName = dr["FirstName"].ToString() + " " + dr["LastName"].ToString();
                _loginObject.CompanyID = dr["CompanyID"].ToString();
                _loginObject.CompanyTag = dr["CompanyTag"].ToString();
                _loginObject.CompanyName = dr["CompanyName"].ToString();
                _loginObject.UserName = dr["UserID"].ToString();
            }

            return _loginObject;



        }
    }
}