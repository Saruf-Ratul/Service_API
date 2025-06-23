using Services.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace Services.Processor
{
    public class AppointmentStatusProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        public List<Status> GetAllAppointmentStatusList(string companyID)
        {
            var list = new List<Status>();
            try
            {
                

                Database db = new Database(connStr);

                string Sql = @"SELECT  [StatusID]
                              ,[StatusName]
                              ,[CompanyID]
                          FROM [msSchedulerV3].[dbo].[tbl_Status] where  CompanyID='" + companyID + "' order by StatusID";


                DataTable dt = new DataTable();
                db.Execute(Sql, out dt);



                foreach (DataRow _dr in dt.Rows)
                {
                    Status _status = new Status();
                    _status.StatusId = Convert.ToInt32(_dr["StatusID"]);
                    _status.StatusName = _dr["StatusName"].ToString();
                    list.Add(_status);

                }
                return list;
            }
            catch (Exception ex){
                throw ex;

            }
        }
    }
}