
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using PetroBM.Data;
using System.Data;
using System.Data.SqlClient;


namespace PetroBM.Services.Services
{
    public interface IConfigArmGrpConfigArmService
    {
        IEnumerable<MConfigArmGrpConfigArm> GetAllConfigArmGrpConfigArm();
        bool DeleteConfigArmGrpConfigArm(int id);
        MConfigArmGrpConfigArm FindConfigArmGrpConfigArmById(int id);

        List<int> GetArmNo_By_ConfigArmGroupId(int configarmgroupId);
    }

    public class ConfigArmGrpConfigArmService : IConfigArmGrpConfigArmService
    {
        private readonly IConfigArmGrpConfigArmRepository configarmgrpconfigarmRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public ConfigArmGrpConfigArmService(IConfigArmGrpConfigArmRepository configarmgrpconfigarmRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.configarmgrpconfigarmRepository = configarmgrpconfigarmRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateConfigArmGrpConfigArm(MConfigArmGrpConfigArm configarmgrpconfigarm)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    configarmgrpconfigarmRepository.Add(configarmgrpconfigarm);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
            }
            catch (Exception e) { }

            return rs;
        }

        public bool DeleteConfigArmGrpConfigArm(int id)
        {
            MConfigArmGrpConfigArm configarmgrpconfigarm = this.FindConfigArmGrpConfigArmById(id);
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    configarmgrpconfigarmRepository.Update(configarmgrpconfigarm);
                    unitOfWork.Commit();
                    ts.Complete();

                    rs = true;
                }

            }
            catch { }

            return rs;
        }

        public IEnumerable<MConfigArmGrpConfigArm> GetAllConfigArmGrpConfigArm()
        {
            return configarmgrpconfigarmRepository.GetAll();
        }
       
        public MConfigArmGrpConfigArm FindConfigArmGrpConfigArmById(int id)
        {
            return configarmgrpconfigarmRepository.GetById(id);
        }

       public  List<int> GetArmNo_By_ConfigArmGroupId(int configarmgroupId)
        {
            var rs = false;

            List<int> listConfigArm = new List<int>();

            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Get_ArmNo_By_ConfigArmGroupId";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ConfigArmGroupId", configarmgroupId);
                            cmd.Parameters.Add(param);
                            SqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                listConfigArm.Add(int.Parse(reader["ArmNo"].ToString()));
      
                            }
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception e) { }
            return listConfigArm;
        }





    }
}