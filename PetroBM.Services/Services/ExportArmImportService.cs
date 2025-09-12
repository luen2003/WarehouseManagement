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
    public interface IExportArmImportService
    {
        IEnumerable<MExportArmImport> GetAllExportArmImport();
        bool CreateExportArmImport(MExportArmImport exportarmimport);
        bool UpdateExportArmImport(MExportArmImport exportarmimport);

        bool Update_StartExportArmImport(MExportArmImport exportarmimport);

        bool Update_EndExportArmImport(MExportArmImport exportarmimport);

        IEnumerable<MExportArmImport> GetAllExportArmImportByImportInfoId(int importInfoId);

    }

    public class ExportArmImportService : IExportArmImportService
    {
        private readonly IExportArmImportRepository exportarmimportRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public ExportArmImportService(IExportArmImportRepository exportarmimportRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.exportarmimportRepository = exportarmimportRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateExportArmImport(MExportArmImport exportarmimport)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    exportarmimportRepository.Add(exportarmimport);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }
            }
            catch (Exception e) { }

            return rs;
        }


        public IEnumerable<MExportArmImport> GetAllExportArmImport()
        {
            return exportarmimportRepository.GetAll().Where(p => p.DeleteFlag == Constants.FLAG_OFF);
        }

        public IEnumerable<MExportArmImport> GetAllExportArmImportByImportInfoId(int importInfoId)
        {
            return exportarmimportRepository.GetAll().Where(p => p.DeleteFlag == Constants.FLAG_OFF && p.ImportInfoId== importInfoId);
        }

        public bool UpdateExportArmImport(MExportArmImport exportarmimport)
        {
            var rs = false;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    exportarmimportRepository.Update(exportarmimport);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }
            }
            catch (Exception e) { }

            return rs;
        }

        public bool Update_StartExportArmImport(MExportArmImport exportarmimport)
        {
            var rs = false;
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Update_Start_MExportArmImport";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", exportarmimport.ImportInfoId);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", exportarmimport.WareHouseCode);
                            SqlParameter param3 = new SqlParameter("ArmNo", exportarmimport.ArmNo);
                            SqlParameter param4 = new SqlParameter("StartTotal", exportarmimport.StartTotal);
                            SqlParameter param5 = new SqlParameter("StartTotalBase", exportarmimport.StartTotalBase);
                            SqlParameter param6 = new SqlParameter("StartTotalE", exportarmimport.StartTotalE);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.Parameters.Add(param6);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception e) { }

            return rs;
        }

        public bool Update_EndExportArmImport(MExportArmImport exportarmimport)
        {
            var rs = false;
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Update_End_MExportArmImport";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", exportarmimport.ImportInfoId);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", exportarmimport.WareHouseCode);
                            SqlParameter param3 = new SqlParameter("ArmNo", exportarmimport.ArmNo);
                            SqlParameter param4 = new SqlParameter("EndTotal", exportarmimport.EndTotal);
                            SqlParameter param5 = new SqlParameter("EndTotalBase", exportarmimport.EndTotalBase);
                            SqlParameter param6 = new SqlParameter("EndTotalE", exportarmimport.EndTotalE);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.Parameters.Add(param6);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception e) { }

            return rs;
        }
    }
}
