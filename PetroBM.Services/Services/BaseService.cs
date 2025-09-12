using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Services.Services
{

    public interface IBaseService
    {
        bool CheckPermission(string userName, int permissonCode);

        bool CheckPermissionWithWareHouse(string userName, int permissonCode, byte wareHouseCode);
    }

    public class BaseService : IBaseService
    {
        public bool CheckPermission(string userName, int permissonCode)
        {
            bool activeFlag = false;
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select_Permission_By_ManagerReportChart";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("UserName", userName);
                        SqlParameter param2 = new SqlParameter("PermissionCode", permissonCode);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            activeFlag = reader.GetBoolean(0);
                        }
                    }
                    conn.Close();
                }
            }
            return activeFlag;
        }

        public bool CheckPermissionWithWareHouse(string userName, int permissonCode,byte wareHouseCode)
        {
            bool activeFlag = false;
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Select_Permission_By_ManagerReportChart";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("UserName", userName);
                        SqlParameter param2 = new SqlParameter("PermissionCode", permissonCode);
                        SqlParameter param3 = new SqlParameter("PermissionCode", wareHouseCode);
                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            activeFlag = reader.GetBoolean(0);
                        }
                    }
                    conn.Close();
                }
            }
            return activeFlag;
        }


    }
}
