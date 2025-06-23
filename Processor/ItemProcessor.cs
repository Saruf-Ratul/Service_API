using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace Services.Processor
{
    public class ItemProcessor
    {
        string connStr = ConfigurationManager.AppSettings["ConnString"].ToString();
        public List<Items> GetAllItems(string companyId)
        {
            try
            {
                string sql = @"
        SELECT Id, Name, Description, Barcode, ItemTypeId, Price, Location, IsTaxable, 
               CompanyId, IsDeleted, QboId
        FROM [msSchedulerV3].[dbo].[Items] 
        WHERE CompanyId = @CompanyID;";

                Database db = new Database();
                DataSet dataSet = db.Get_DataSet(sql, companyId);
                DataTable dt = dataSet.Tables[0];

                List<Items> items = new List<Items>();

                foreach (DataRow dr in dt.Rows)
                {
                    Items item = new Items
                    {
                        Id = dr["Id"].ToString(),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString(),
                        Barcode = dr["Barcode"].ToString(),
                        ItemTypeId = Convert.ToInt32(dr["ItemTypeId"]),
                        Price = Convert.ToDecimal(dr["Price"]),
                        Location = dr["Location"].ToString(),
                        IsTaxable = dr["IsTaxable"] != DBNull.Value && Convert.ToBoolean(dr["IsTaxable"]),
                        CompanyId = dr["CompanyId"].ToString(),
                        IsDeleted = dr["IsDeleted"] != DBNull.Value && Convert.ToBoolean(dr["IsDeleted"]),
                        QboId = dr["QboId"] != DBNull.Value ? Convert.ToInt32(dr["QboId"]) : (int?)null
                    };

                    items.Add(item);
                }

                return items;
            }
            catch
            {
                throw;
            }
        }
    }
    public class Items
    {

        public string Id { get; set; }

       public string Name { get; set; }

        public string Description { get; set; } = string.Empty;


        public string Barcode { get; set; }

 
        public int ItemTypeId { get; set; }


        public decimal Price { get; set; }

        public string Location { get; set; }


        public bool IsTaxable { get; set; } = false;


        public string CompanyId { get; set; }

    
        public bool IsDeleted { get; set; } = false;

     
        public long? QboId { get; set; }
    }
}