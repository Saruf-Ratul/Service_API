using Services.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace Services.Processor
{
    public class TicketStatusProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        public List<TicketStatus> GetAllTicketStatus(string companyID)
        {
            var list = new List<TicketStatus>();
            try
            {
      

                Database db = new Database(connStr);

                string Sql = @"SELECT  [StatusID]
                              ,[StatusName]
                              ,[CompanyID]
                          FROM [msSchedulerV3].[dbo].[tbl_TicketStatus] where  CompanyID='" + companyID + "' order by StatusID";


                DataTable dt = new DataTable();
                db.Execute(Sql, out dt);

                foreach (DataRow _dr in dt.Rows)
                {
                    TicketStatus _status = new TicketStatus();
                    _status.StatusId = Convert.ToInt32(_dr["StatusID"]);
                    _status.StatusName = _dr["StatusName"].ToString();
                    list.Add(_status);

                }
            }
            catch(Exception ex) {
                throw ex;
            }

            return list;
        }

    }
}