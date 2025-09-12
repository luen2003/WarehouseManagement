using PetroBM.Common.Util;
using PetroBM.Data;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PetroBM.Services.Services
{
    public interface IImportService
    {
        MImportInfo GetImportInfo(int id);
        IEnumerable<MImportInfo> GetAllImportInfo();
        IEnumerable<MImportInfo> GetImportInfoByTime(DateTime? startDate, DateTime? endDate);
        IEnumerable<MImportInfo> GetImportInfoByTime(byte wareHousecode, DateTime? startDate, DateTime? endDate);
        IEnumerable<MImportInfo> GetAllImportInfoByID(int importInfoId);

        IEnumerable<MTankImport> GetTankImportById(int id);

        bool CreateImportInfo( MImportInfo importInfo, int[] tankIdList,List<byte> listArmNo);
        bool UpdateImportInfo(MImportInfo importInfo);
        bool UpdateImportInfoVttV15(MImportInfo importInfo);
        bool DeleteImportInfo(MImportInfo importInfo);
        bool GetStartHandleImportInfo(MImportInfo importInfo);
        bool GetStartHandleImportInfo(MImportInfo importInfo,DateTime startDate);
        bool UpdateStartHandleImportInfo(MImportInfo importInfo);
        bool FinishStartHandleImportInfo(MImportInfo importInfo, string user);
        bool FinishStartHandleImportInfoManual(MImportInfo importInfo, string user, string message);
        bool GetEndHandleImportInfo(MImportInfo importInfo);
        bool GetEndHandleImportInfo(MImportInfo importInfo, DateTime startDate);
        bool UpdateEndHandleImportInfo(MImportInfo importInfo);
        bool UpdateEndHandleTankImport(List<MTankImport> lstTankImport, MImportInfo importInfo);
        //bool UpdateEndHandleImportInfo();
        bool FinishEndHandleImportInfo(MImportInfo importInfo, string user);
        bool FinishEndHandleImportInfoManual(MImportInfo importInfo, string user, string message);
        DataTable GetLastVttAndV15(int importInfoId);

        #region Add TankImport bổ sung
        bool CreateTankImport(MTankImport tankimport);
        bool CreateTankImportTemp(MTankImportTemp tankimporttemp);
        #endregion


        IEnumerable<MClock> GetAllClock();
        bool ClockExport(MImportInfo importInfo, int tankId,
            double vtt, double v15, string user);
        IEnumerable<MClockExport> GetAllClockExportById(int id);

        int GetExportTankId(int importInfoId);
        double GetExportVtt(int importInfoId);
        double GetExportV15(int importInfoId);
        float? GetStartDensity(byte wareHouseCode, int tankId);
        bool Update_ClockExport_StarVtt_EndVtt(int importInfoId, int clockid, double start_Vtt, double end_Vtt);
        string GetListArmName(int importInfoId);

    }

    public class ImportService : IImportService
    {
        private static log4net.ILog Log { get; set; }
        private readonly IImportRepository importRepository;
        private readonly ITankImportRepository tankImportRepository;
        private readonly IClockRepository clockRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ITankService tankService;
        private readonly IEventService eventService;
        private readonly ITankImportService tankImportService;
        private readonly ITankImportTempService tankImportTempService;
        private readonly IClockExportRepository clockExportRepository;
        private readonly ITankImportTempRepository tankImportTempRepository;
        private readonly ITankLiveService tankLiveService;

        public ImportService(IImportRepository importRepository, ITankImportRepository tankImportRepository,
            IClockRepository clockRepository, ITankService tankService, IEventService eventService, ITankImportTempService tankImportTempService, ITankImportService tankImportService
            , IUnitOfWork unitOfWork, IClockExportRepository clockExportRepository, ITankImportTempRepository tankImportTempRepository, ITankLiveService tankLiveService)
        {
            this.importRepository = importRepository;
            this.tankImportRepository = tankImportRepository;
            this.clockRepository = clockRepository;
            this.eventService = eventService;
            this.tankService = tankService;
            this.unitOfWork = unitOfWork;
            this.clockExportRepository = clockExportRepository;
            this.tankImportTempRepository = tankImportTempRepository;
            this.tankImportTempService = tankImportTempService;
            this.tankImportService = tankImportService;
            this.tankLiveService = tankLiveService;

        }

        public MImportInfo GetImportInfo(int id)
        {
            return importRepository.GetById(id);
        }

        public IEnumerable<MImportInfo> GetAllImportInfoByID(int importInfoId)
        {
            return importRepository.GetAll().Where(im => im.DeleteFlg == Constants.FLAG_OFF && im.Id == importInfoId)
                .OrderByDescending(im => im.InsertDate);
        }


        public IEnumerable<MImportInfo> GetAllImportInfo()
        {
            return importRepository.GetAll().Where(im => im.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(im => im.InsertDate);
        }

        public IEnumerable<MImportInfo> GetImportInfoByTime(DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<MImportInfo> rs = null;

            if (startDate == null && endDate == null)
            {
                rs = GetAllImportInfo();
            }
            else if (startDate == null)
            {
                rs = importRepository.GetMany(im => (im.DeleteFlg == Constants.FLAG_OFF) && (im.InsertDate <= endDate))
                     .OrderByDescending(im => im.InsertDate);
            }
            else if (endDate == null)
            {
                rs = importRepository.GetMany(im => (im.DeleteFlg == Constants.FLAG_OFF) && (startDate <= im.InsertDate))
                     .OrderByDescending(im => im.InsertDate);
            }
            else
            {
                rs = importRepository.GetMany(im => (im.DeleteFlg == Constants.FLAG_OFF)
                && (startDate <= im.InsertDate) && (im.InsertDate <= endDate))
                     .OrderByDescending(im => im.InsertDate);
            }

            return rs;
        }


        public IEnumerable<MImportInfo> GetImportInfoByTime( byte wareHouseCode,DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<MImportInfo> rs = null;

            if (startDate == null && endDate == null)
            {
                rs = GetAllImportInfo();
            }
            else if (startDate == null)
            {
                rs = importRepository.GetMany(im => (im.DeleteFlg == Constants.FLAG_OFF) && (im.InsertDate <= endDate))
                     .OrderByDescending(im => im.InsertDate);
            }
            else if (endDate == null)
            {
                rs = importRepository.GetMany(im => (im.DeleteFlg == Constants.FLAG_OFF) && (startDate <= im.InsertDate))
                     .OrderByDescending(im => im.InsertDate);
            }
            else
            {
                rs = importRepository.GetMany(im => (im.DeleteFlg == Constants.FLAG_OFF)
                && (im.InsertDate >=startDate) && (im.InsertDate <= endDate) && (im.WareHouseCode == wareHouseCode))
                     .OrderByDescending(im => im.InsertDate);

            }

            return rs;
        }       


        public bool CreateImportInfo(MImportInfo importInfo, int[] tankIdList,List<byte>listArmNo)
        {
            var rs = false;
            var tankImport = new MTankImport();
            var tankImportTemp = new MTankImportTemp();
            float? density = 0;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    importInfo.UpdateUser = importInfo.InsertUser;
                    importRepository.Add(importInfo);
                    SaveImportInfo();
                    
                    ////Chuoi qua khong hieu Insert tai day khong duoc.
                    foreach (var id in tankIdList)
                    {
                        tankImport = new MTankImport();
                        tankImport.ImportInfoId = importInfo.Id;
                        tankImport.TankId = id;
                        tankImport.WareHouseCode = importInfo.WareHouseCode;
                        density = GetStartDensity(importInfo.WareHouseCode, id); // GetStartDensity(id);
                        tankImport.StartDensity = density;
                        tankImport.ExportFlg = Constants.FLAG_OFF;
                       // tankImportRepository.Add(tankImport);
                        CreateTankImport(tankImport);

                        //temp tank Import
                        tankImportTemp = new MTankImportTemp();
                        tankImportTemp.ImportInfoId = importInfo.Id;
                        tankImportTemp.TankId = id;
                        tankImportTemp.WareHouseCode = importInfo.WareHouseCode;
                        tankImportTemp.StartDensity = density;
                        // tankImportTempRepository.Add(tankImportTemp);
                        CreateTankImportTemp(tankImportTemp);
             
                    }
                    ////ThangNK comment lai;
                    ////Téc CC
                    //tankImport = new MTankImport();
                    //tankImport.ImportInfoId = importInfo.Id;
                    //tankImport.TankId = Constants.TEC_CC_ID;
                    //tankImport.StartDensity = density;
                    //tankImport.ExportFlg = Constants.FLAG_OFF;
                    //tankImportRepository.Add(tankImport);

                    //// temp tec cc
                    //tankImportTemp = new MTankImportTemp();
                    //tankImportTemp.ImportInfoId = importInfo.Id;
                    //tankImportTemp.TankId = Constants.TEC_CC_ID;
                    //tankImportTemp.WareHouseCode = importInfo.WareHouseCode;
                    //tankImportTemp.StartDensity = density;
                    //tankImportTempRepository.Add(tankImportTemp);


                    //thêm bảng đồng hồ xuất khi bảng ImportInfo được thêm
                    Add_ClockExports(importInfo.Id);

                    // Create ClockExport
                    //var clockList = GetAllClock();

                    //for (int i = 0; i < clockList.Count(); i++)
                    //{
                    //    var clockExport = new MClockExport();
                    //    clockExport.ClockId = clockList.ElementAt(i).ClockId;

                    //    importInfo.MClockExport.Add(clockExport);
                    //}
                    // SaveImportInfo();


                    if (listArmNo!=null)
                    {
                        Delete_ExportArmImport(importInfo.Id);
                        for (byte i = 0; i < listArmNo.Count(); i++)
                        {
                            //Update họng sẽ bơm ra khỏi kho
                            UpdateAdd_ExportArmImport(listArmNo[i], importInfo.WareHouseCode, importInfo.Id, (int)importInfo.ProductId);
                        }
                    }



                    ts.Complete();
                    rs = true;

                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_IMPORT,
                        Constants.EVENT_IMPORT_CREATE, importInfo.InsertUser);
                }
            }
            catch (Exception ex){

                Log.Error("Error CreateImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        private float? GetStartDensity(int tankId)
        {
            var tank = tankService.FindTankById(tankId);

            return tank.Density ?? Constants.MIN_DENSITY;
        }

        public float? GetStartDensity(byte wareHouseCode,int tankId)
        {
            var tank = tankService.FindTankById(tankId, wareHouseCode);

            return tank.Density ?? Constants.MIN_DENSITY;
        }

        public bool UpdateImportInfo(MImportInfo importInfo)
        {
            var rs = false;

            try
            {
                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_IMPORT,
                    Constants.EVENT_IMPORT_UPDATE, importInfo.UpdateUser);
            }
            catch (Exception ex)
            {
                Log.Error("Error CreateImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool UpdateImportInfoVttV15(MImportInfo importInfo)
        {
            var rs = false;

            try
            {
                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_IMPORT,
                    Constants.EVENT_IMPORT_UPDATE, importInfo.UpdateUser);
            }
            catch (Exception ex)
            {
                Log.Error("Error CreateImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }


        public bool DeleteImportInfo(MImportInfo importInfo)
        {
            var rs = false;

            try
            {
                importInfo.DeleteFlg = Constants.FLAG_ON;
                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_IMPORT,
                    Constants.EVENT_IMPORT_DELETE, importInfo.UpdateUser);
            }
            catch (Exception ex)
            {
                Log.Error("Error DeleteImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool GetStartHandleImportInfo(MImportInfo importInfo)
        {
            var rs = false;

            try
            {
                //for (int i = 0; i < importInfo.MTankImport.Count; i++)
                //{
                //    var tankImport = importInfo.MTankImport[i];

                //    var tankTempImport = importInfo.MTankImportTemps[i];

                //    if (importInfo.MTankImport[i].TankId != Constants.TEC_CC_ID)
                //    {
                 //    var tanklog = tankService.GetTankLogByTime(importInfo.WareHouseCode,tankImport.TankId, tankImport.StartDate.Value);

                //        tankImport.StartTemperature = tanklog.AvgTemperature ?? 0;
                //        tankImport.StartProductLevel = tanklog.ProductLevel ?? 0;
                //        tankImport.StartProductVolume = tanklog.ProductVolume ?? 0;
                //        tankImport.StartVCF = tanklog.VCF ?? 0;
                //        tankImport.StartProductVolume15 = (tankImport.StartProductVolume * tankImport.StartVCF) ?? 0;

                //        //temp
                //        tankTempImport.StartTemperature = tanklog.AvgTemperature ?? 0;
                //        tankTempImport.StartProductLevel = tanklog.ProductLevel ?? 0;

                //    }
                //    else
                //    {
                //        tankImport.StartProductLevel = 0;
                //        tankImport.StartProductVolume = 0;
                //        tankImport.StartDensity = importInfo.MTankImport[importInfo.MTankImport.Count - 2].StartDensity;
                //        tankImport.StartVCF = importInfo.MTankImport[importInfo.MTankImport.Count - 2].StartVCF;
                //        tankImport.StartProductVolume15 = 0;

                //        //temp

                //        tankTempImport.StartProductLevel = 0;
                //        tankTempImport.StartDensity = importInfo.MTankImport[importInfo.MTankImport.Count - 2].StartDensity;
                //    }

                //    tankImportService.UpdateTankImport(tankImport);
                //    tankImportTempService.UpdateTankImportTemp(tankTempImport);
                //}


                // Sử dụng hàm update phần trên bằng Store Procedure

                //Update_Start_TankImport_From_TankLog(importInfo.Id, Constants.MIN_DENSITY);

                importRepository.Update(importInfo);

                SaveImportInfo();
                rs = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error GetStartHandleImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }


        public bool GetStartHandleImportInfo(MImportInfo importInfo,DateTime startDate)
        {
            var rs = false;

            try
            {
                //for (int i = 0; i < importInfo.MTankImport.Count; i++)
                //{
                //    var tankImport = importInfo.MTankImport[i];

                //    var tankTempImport = importInfo.MTankImportTemps[i];

                //    if (importInfo.MTankImport[i].TankId != Constants.TEC_CC_ID)
                //    {
                //        var tanklog = tankService.GetTankLogByTime(importInfo.WareHouseCode, tankImport.TankId, tankImport.StartDate.Value);

                //        tankImport.StartTemperature = tanklog.AvgTemperature ?? 0;
                //        tankImport.StartProductLevel = tanklog.ProductLevel ?? 0;
                //        tankImport.StartProductVolume = tanklog.ProductVolume ?? 0;
                //        tankImport.StartVCF = tanklog.VCF ?? 0;
                //        tankImport.StartProductVolume15 = (tankImport.StartProductVolume * tankImport.StartVCF) ?? 0;

                //        //temp
                //        tankTempImport.StartTemperature = tanklog.AvgTemperature ?? 0;
                //        tankTempImport.StartProductLevel = tanklog.ProductLevel ?? 0;

                //    }
                //    else
                //    {
                //        tankImport.StartProductLevel = 0;
                //        tankImport.StartProductVolume = 0;
                //        tankImport.StartDensity = importInfo.MTankImport[importInfo.MTankImport.Count - 2].StartDensity;
                //        tankImport.StartVCF = importInfo.MTankImport[importInfo.MTankImport.Count - 2].StartVCF;
                //        tankImport.StartProductVolume15 = 0;

                //        //temp

                //        tankTempImport.StartProductLevel = 0;
                //        tankTempImport.StartDensity = importInfo.MTankImport[importInfo.MTankImport.Count - 2].StartDensity;
                //    }

                //    tankImportService.UpdateTankImport(tankImport);
                //    tankImportTempService.UpdateTankImportTemp(tankTempImport);
                //}


                // Sử dụng hàm update phần trên bằng Store Procedure

                Update_Start_TankImport_From_TankLog(importInfo.Id, Constants.MIN_DENSITY, startDate);
                Update_Start_ExportArmImport_By_DataLogArm(importInfo.Id, Constants.MIN_DENSITY, startDate);

                importRepository.Update(importInfo);

                SaveImportInfo();
                rs = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error GetStartHandleImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool UpdateStartHandleImportInfo(MImportInfo importInfo)
        {
            var rs = false;

            try
            {
                // Update lại StartVCF,StartProductVolume,StartProductVolume15 sẽ được controller Import
                //for (int i = 0; i < importInfo.MTankImport.Count; i++)
                //{
                //    var tankImport = importInfo.MTankImport[i];

                //    if (importInfo.MTankImport[i].TankId != Constants.TEC_CC_ID)
                //    {
                //        tankImport.StartProductVolume = tankService.SearchVolume(tankImport.TankId, tankImport.StartProductLevel ?? 0);
                //        tankImport.StartVCF = tankService.SearchVCF(tankImport.StartDensity ?? 0, tankImport.StartTemperature ?? 0);
                //        tankImport.StartProductVolume15 = (tankImport.StartProductVolume * tankImport.StartVCF) ?? 0;
                //    }
                //    else
                //    {
                //        tankImport.StartProductLevel = tankImport.StartProductLevel ?? 0;
                //        tankImport.StartProductVolume = tankImport.StartProductLevel * Constants.TEC_CC_CONSTANT;
                //        tankImport.StartDensity = importInfo.MTankImport[importInfo.MTankImport.Count - 2].StartDensity;
                //        tankImport.StartVCF = importInfo.MTankImport[importInfo.MTankImport.Count - 2].StartVCF;
                //        tankImport.StartProductVolume15 = (tankImport.StartProductVolume * tankImport.StartVCF) ?? 0;
                //    }
                //}

                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error UpdateStartHandleImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool FinishStartHandleImportInfo(MImportInfo importInfo, string user)
        {
            var rs = false;
            try
            {
                importInfo.StartFlag = Constants.FLAG_ON;

                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_IMPORT,
                    Constants.EVENT_IMPORT_START_HANDLE, user);
            }
            catch (Exception ex)
            {
                Log.Error("Error FinishStartHandleImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool FinishStartHandleImportInfoManual(MImportInfo importInfo, string user, string message)
        {
            var rs = false;

            try
            {
                importInfo.StartFlag = Constants.FLAG_ON;

                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;

                string infoBefore = "Giá trị trước khi sửa: ";
                foreach (var item in importInfo.MTankImportTemps)
                {
                    string temp = "\nNgày chốt đầu: " + item.StartDate + ", Tên bể: " + item.MTank.TankName + ", Nhiệt độ: " + item.StartTemperature
                        + ", Chiều cao hàng: " + item.StartProductLevel + ", Tỉ trọng: " + item.StartDensity;
                    infoBefore += temp;
                }

                string infoAfter = "\n\nGiá trị sau khi sửa: ";
                foreach (var item in importInfo.MTankImport)
                {
                    string temp = "\nNgày chốt đầu: " + item.StartDate + ", Tên bể: " + item.MTank.TankName + ", Nhiệt độ: " + item.StartTemperature
                        + ", Chiều cao hàng: " + item.StartProductLevel + ", Tỉ trọng: " + item.StartDensity;
                    infoAfter += temp;
                }

                string note = "\nLý do: " + message;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_IMPORT, Constants.EVENT_IMPORT_START_HANDLE + "\n" +
                    infoBefore + infoAfter + note, user);
            }
            catch (Exception ex)
            {
                Log.Error("Error FinishStartHandleImportInfoManual");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool GetEndHandleImportInfo(MImportInfo importInfo)
        {
            var rs = false;

            try
            {
                //for (int i = 0; i < importInfo.MTankImport.Count; i++)
                //{
                //    var tankImport = importInfo.MTankImport[i];
                //    var tankTempImport = importInfo.MTankImportTemps[i];

                //    if (tankImport.TankId != Constants.TEC_CC_ID)
                //    {
                //       var tanklog = tankService.GetTankLogByTime(tankImport.WareHouseCode, tankImport.TankId, tankImport.EndDate.Value);

                //        tankImport.EndTemperature = tanklog.AvgTemperature ?? 0;
                //        tankImport.EndProductLevel = tanklog.ProductLevel ?? 0;
                //        tankImport.EndProductVolume = tanklog.ProductVolume ?? 0;
                //        tankImport.EndVCF = tanklog.VCF ?? 0;
                //        tankImport.EndProductVolume15 = (tankImport.EndProductVolume * tankImport.EndVCF) ?? 0;

                //        //log so dau
                //        tankTempImport.EndTemperature = tanklog.AvgTemperature ?? 0;
                //        tankTempImport.EndProductLevel = tanklog.ProductLevel ?? 0;

                //        if (tankImport.EndProductVolume == tankImport.StartProductVolume)
                //        {
                //            tankImport.EndDensity = tankImport.StartDensity ?? Constants.MIN_DENSITY;
                //        }
                //        else if (tankImport.EndProductVolume == 0)
                //        {
                //            tankImport.EndDensity = Constants.MIN_DENSITY;
                //        }
                //        else
                //        {
                //            tankImport.EndDensity = ((importInfo.Density * (float)(tankImport.EndProductVolume - tankImport.StartProductVolume)
                //            + tankImport.StartDensity * (float)tankImport.StartProductVolume)
                //            / (float)tankImport.EndProductVolume) ?? Constants.MIN_DENSITY;

                //if (tankImport.EndDensity < Constants.MIN_DENSITY)
                //{
                //    tankImport.EndDensity = Constants.MIN_DENSITY;
                //}
                //        }
                //    }
                //    else
                //    {
                //        tankImport.EndProductLevel = 0;
                //        tankImport.EndProductVolume = 0;
                //        tankImport.EndDensity = importInfo.MTankImport[importInfo.MTankImport.Count - 2].EndDensity;
                //        tankImport.EndVCF = importInfo.MTankImport[importInfo.MTankImport.Count - 2].EndVCF;
                //        tankImport.EndProductVolume15 = 0;

                //        //log so dau
                //        tankTempImport.EndProductLevel = 0;
                //    }
                //}

              //  Update_Start_TankImport_From_TankLog(importInfo.Id, Constants.MIN_DENSITY);

                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error GetEndHandleImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        


        public bool GetEndHandleImportInfo(MImportInfo importInfo,DateTime startDate)
        {
            var rs = false;            
            try
            {
                for (int i = 0; i < importInfo.MTankImport.Count; i++)
                {
                    var tankImport = importInfo.MTankImport[i];
                    var tankTempImport = importInfo.MTankImportTemps[i];

                    if (tankImport.TankId != Constants.TEC_CC_ID)
                    {
                        var tanklog = tankService.GetTankLogByTime(tankImport.WareHouseCode, tankImport.TankId, tankImport.EndDate.Value);

                        tankImport.EndTemperature = tanklog.AvgTemperature ?? 0;
                        tankImport.EndProductLevel = tanklog.ProductLevel ?? 0;
                        tankImport.EndProductVolume = tanklog.ProductVolume ?? 0;
                        tankImport.EndVCF = tanklog.VCF ?? 0;
                        tankImport.EndProductVolume15 = (tankImport.EndProductVolume * tankImport.EndVCF) ?? 0;

                        //log so dau
                        tankTempImport.EndTemperature = tanklog.AvgTemperature ?? 0;
                        tankTempImport.EndProductLevel = tanklog.ProductLevel ?? 0;

                        if (tankImport.EndProductVolume == tankImport.StartProductVolume)
                        {
                            tankImport.EndDensity = tankImport.StartDensity ?? Constants.MIN_DENSITY;
                        }
                        else if (tankImport.EndProductVolume == 0)
                        {
                            tankImport.EndDensity = Constants.MIN_DENSITY;
                        }
                        else
                        {
                            tankImport.EndDensity = ((importInfo.Density * (float)(tankImport.EndProductVolume - tankImport.StartProductVolume)
                            + tankImport.StartDensity * (float)tankImport.StartProductVolume)
                            / (float)tankImport.EndProductVolume) ?? Constants.MIN_DENSITY;

                            if (tankImport.EndDensity < Constants.MIN_DENSITY)
                            {
                                tankImport.EndDensity = Constants.MIN_DENSITY;
                            }
                        }
                    }
                    else
                    {
                        tankImport.EndProductLevel = 0;
                        tankImport.EndProductVolume = 0;
                        tankImport.EndDensity = importInfo.MTankImport[importInfo.MTankImport.Count - 2].EndDensity;
                        tankImport.EndVCF = importInfo.MTankImport[importInfo.MTankImport.Count - 2].EndVCF;
                        tankImport.EndProductVolume15 = 0;

                        //log so dau
                        tankTempImport.EndProductLevel = 0;
                    }
                }

                Update_End_TankImport_From_TankLog(importInfo.Id, Constants.MIN_DENSITY, startDate);
                Update_End_ExportArmImport_By_DataLogArm(importInfo.Id, Constants.MIN_DENSITY, startDate);

                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error GetEndHandleImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool UpdateEndHandleTankImport(List<MTankImport> lstTankImport,MImportInfo importInfo)
        {
            var rs = false;
            try
            {
                for (int i = 0; i < lstTankImport.Count; i++)
                {
                    var tankImport = lstTankImport[i];

                    if (tankImport.TankId != Constants.TEC_CC_ID)
                    {
                        tankImport.EndProductVolume = tankService.SearchVolume(tankImport.TankId, tankImport.EndProductLevel ?? 0);

                        if (tankImport.EndProductVolume == tankImport.StartProductVolume)
                        {
                            tankImport.EndDensity = tankImport.StartDensity ?? 0;
                        }
                        else
                        {
                            tankImport.EndDensity = ((importInfo.Density * (float)(tankImport.EndProductVolume - tankImport.StartProductVolume)
                            + tankImport.StartDensity * (float)tankImport.StartProductVolume) / (float)tankImport.EndProductVolume) ?? 0;
                        }

                        tankImport.EndVCF = tankService.SearchVCF(tankImport.EndDensity ?? 0, tankImport.EndTemperature ?? 0);
                        tankImport.EndProductVolume15 = (tankImport.EndProductVolume * tankImport.EndVCF) ?? 0;
                        tankImportService.UpdateTankImport(tankImport);

                    }
                    else
                    {
                        tankImport.EndProductLevel = tankImport.EndProductLevel ?? 0;
                        tankImport.EndProductVolume = tankImport.EndProductLevel * Constants.TEC_CC_CONSTANT;
                        tankImport.EndDensity = importInfo.MTankImport[importInfo.MTankImport.Count - 2].EndDensity;
                        tankImport.EndVCF = importInfo.MTankImport[importInfo.MTankImport.Count - 2].EndVCF;
                        tankImport.EndProductVolume15 = (tankImport.EndProductVolume * tankImport.EndVCF) ?? 0;
                    }
                }

                GetEndHandleImportInfo(importInfo);
                //importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error UpdateEndHandleImportInfo");
                Log.Error("Error" + ex.Message);
            }
            return rs;
        }



        public bool UpdateEndHandleImportInfo(MImportInfo importInfo)
        {
                var rs = false;
            var lstImportTank = GetAllImportInfoByID(importInfo.Id);

            try
            {
                for (int i = 0; i < importInfo.MTankImport.Count; i++)
                {
                    var tankImport = importInfo.MTankImport[i];

                    if (tankImport.TankId != Constants.TEC_CC_ID)
                    {
                        tankImport.EndProductVolume = tankService.SearchVolume(tankImport.TankId, tankImport.EndProductLevel ?? 0);

                        if (tankImport.EndProductVolume == tankImport.StartProductVolume)
                        {
                            tankImport.EndDensity = tankImport.StartDensity ?? 0;
                        }
                        else
                        {
                            tankImport.EndDensity = ((importInfo.Density * (float)(tankImport.EndProductVolume - tankImport.StartProductVolume)
                            + tankImport.StartDensity * (float)tankImport.StartProductVolume) / (float)tankImport.EndProductVolume) ?? 0;
                        }

                        tankImport.EndVCF = tankService.SearchVCF(tankImport.EndDensity ?? 0, tankImport.EndTemperature ?? 0);
                        tankImport.EndProductVolume15 = (tankImport.EndProductVolume * tankImport.EndVCF) ?? 0;
                        tankImportService.UpdateTankImport(tankImport);

                    }
                    else
                    {
                        tankImport.EndProductLevel = tankImport.EndProductLevel ?? 0;
                        tankImport.EndProductVolume = tankImport.EndProductLevel * Constants.TEC_CC_CONSTANT;
                        tankImport.EndDensity = importInfo.MTankImport[importInfo.MTankImport.Count - 2].EndDensity;
                        tankImport.EndVCF = importInfo.MTankImport[importInfo.MTankImport.Count - 2].EndVCF;
                        tankImport.EndProductVolume15 = (tankImport.EndProductVolume * tankImport.EndVCF) ?? 0;
                        tankImportService.UpdateTankImport(tankImport);
                    }
                }

                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;
            }
            catch (Exception ex)
            {
                Log.Error("Error UpdateEndHandleImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool FinishEndHandleImportInfo(MImportInfo importInfo, string user)
        {
            var rs = false;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    importInfo.EndFlag = Constants.FLAG_ON;

                    importRepository.Update(importInfo);
                    SaveImportInfo();

                    for (int i = 0; i < importInfo.MTankImport.Count; i++)
                    {
                        var tankImport = importInfo.MTankImport[i];

                        if (importInfo.Id == GetAllImportInfo().Max(im => im.Id))
                        {
                            var tankDensity = new MTankDensity();
                            tankDensity.Density = tankImport.EndDensity;
                            tankDensity.TankId = tankImport.TankId;
                            tankDensity.InsertUser = user;
                            tankDensity.InsertDate = tankImport.EndDate.Value;

                            if (!tankService.CreateTankDensity(tankDensity))
                            {
                                throw new Exception();
                            }
                        }
                    }

                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_IMPORT,
                    Constants.EVENT_IMPORT_END_HANDLE, user);
            }
            catch (Exception ex)
            {
                Log.Error("Error FinishEndHandleImportInfo");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool FinishEndHandleImportInfoManual(MImportInfo importInfo, string user, string message)
        {
            var rs = false;

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    importInfo.EndFlag = Constants.FLAG_ON;

                    importRepository.Update(importInfo);
                    SaveImportInfo();

                    for (int i = 0; i < importInfo.MTankImport.Count; i++)
                    {
                        var tankImport = importInfo.MTankImport[i];

                        if (importInfo.Id == GetAllImportInfo().Max(im => im.Id))
                        {
                            var tankDensity = new MTankDensity();
                            tankDensity.Density = tankImport.EndDensity;
                            tankDensity.TankId = tankImport.TankId;
                            tankDensity.InsertUser = user;
                            tankDensity.InsertDate = tankImport.EndDate.Value;

                            if (!tankService.CreateTankDensity(tankDensity))
                            {
                                throw new Exception();
                            }
                        }
                    }

                    ts.Complete();
                    rs = true;
                }

                // Log event

                string infoBefore = "Giá trị trước khi sửa: ";
                foreach (var item in importInfo.MTankImportTemps)
                {
                    string temp = "\nNgày chốt cuối: " + item.EndDate + ", Tên bể: " + item.MTank.TankName + ", Nhiệt độ: " + item.EndTemperature
                        + ", Chiều cao hàng: " + item.EndProductLevel;
                    infoBefore += temp;
                }

                string infoAfter = "\n\nGiá trị sau khi sửa: ";
                foreach (var item in importInfo.MTankImport)
                {
                    string temp = "\nNgày chốt cuối: " + item.EndDate + ", Tên bể: " + item.MTank.TankName + ", Nhiệt độ: " + item.EndTemperature
                        + ", Chiều cao hàng: " + item.EndProductLevel;
                    infoAfter += temp;
                }

                string note = "\nLý do: " + message;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_IMPORT, Constants.EVENT_IMPORT_END_HANDLE + "\n" +
                    infoBefore + infoAfter + note, user);
            }
            catch (Exception ex)
            {
                Log.Error("Error FinishEndHandleImportInfoManual");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public IEnumerable<MClock> GetAllClock()
        {
            return clockRepository.GetAll();
        }

        public bool ClockExport(MImportInfo importInfo, int tankId,
            double vtt, double v15, string user)
        {
            var rs = false;

            try
            {
                //foreach (var clockExport in importInfo.MClockExport)
                //{
                //    if ((clockExport.StartVtt == null) || (clockExport.EndVtt == null) || ((clockExport.StartVtt > clockExport.EndVtt)))
                //    {
                //        clockExport.StartVtt = null;
                //        clockExport.EndVtt = null;
                //    }
                //}

                //foreach (var tankImport in importInfo.MTankImport)
                //{
                //    if (tankImport.TankId == tankId)
                //    {
                //        tankImport.ExportVolume = vtt;
                //        tankImport.ExportVolume15 = v15;
                //        tankImport.ExportFlg = Constants.FLAG_ON;
                //    }
                //    else
                //    {
                //        tankImport.ExportVolume = null;
                //        tankImport.ExportVolume15 = null;
                //        tankImport.ExportFlg = Constants.FLAG_OFF;
                //    }
                //}

                //Cập nhật lại bảng Tank import
                Update_TankImport_With_ExportVolume_ExportVolume15_ExportFlg(importInfo.Id, tankId, vtt, v15, Constants.FLAG_ON);

                importRepository.Update(importInfo);
                SaveImportInfo();
                rs = true;

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_IMPORT,
                    Constants.EVENT_IMPORT_CLOCK_EXPORT, user);
            }
            catch (Exception ex)
            {
                Log.Error("Error ClockExport");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }


        public bool Update_TankImport_With_ExportVolume_ExportVolume15_ExportFlg(int importInfoId,int tankId, double ExportVolume, double exportVolume15,bool exportFlg)
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
                            cmd.CommandText = "Update_TankImport_With_ExportVolume_ExportVolume15_ExportFlg";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("TankId", tankId);
                            SqlParameter param3 = new SqlParameter("ExportVolume", ExportVolume);
                            SqlParameter param4 = new SqlParameter("ExportVolume15", exportVolume15);
                            SqlParameter param5 = new SqlParameter("ExportFlg", exportFlg);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Update_TankImport_With_ExportVolume_ExportVolume15_ExportFlg");
                Log.Error("Error" + ex.Message);
            }

            return rs;

        }


        public int GetExportTankId(int importInfoId)
        {
            //int rs = 0;
            //var import = GetImportInfo(importInfoId);
            //var tank = import.MTankImport.Where(ti => ti.ExportFlg == Constants.FLAG_ON).FirstOrDefault();

            //if (tank != null)
            //{
            //    rs = tank.TankId;
            //}
            //return rs;

            int tankId = 0;

            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Get_TankId_From_TankImport_By_ExportFlg";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("ExportFlg", Constants.FLAG_ON);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                tankId = int.Parse(reader["TankId"].ToString());
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error GetExportTankId");
                Log.Error("Error" + ex.Message);
            }

            return tankId;

        }

        public double GetExportVtt(int importInfoId)
        {
            //double rs = 0;

            //var import = GetImportInfo(importInfoId);
            //var tank = import.MTankImport.Where(ti => ti.ExportFlg == Constants.FLAG_ON).FirstOrDefault();

            //if (tank != null)
            //{
            //    rs = tank.ExportVolume ?? 0;
            //}

            //return rs;

            double ExportVolume = 0;

            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Get_ExportVolume_From_TankImport_By_ExportFlg";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("ExportFlg", Constants.FLAG_ON);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ExportVolume = double.Parse(reader["ExportVolume"].ToString());
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error GetExportVtt");
                Log.Error("Error" + ex.Message);
            }

            return ExportVolume;

        }

        public double GetExportV15(int importInfoId)
        {
            //double rs = 0;

            //var import = GetImportInfo(importInfoId);
            //var tank = import.MTankImport.Where(ti => ti.ExportFlg == Constants.FLAG_ON).FirstOrDefault();

            //if (tank != null)
            //{
            //    rs = tank.ExportVolume15 ?? 0;
            //}

            //return rs;

            double ExportVolume15 = 0;

            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "Get_ExportVolume15_From_TankImport_By_ExportFlg";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("ExportFlg", Constants.FLAG_ON);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                ExportVolume15 = double.Parse(reader["ExportVolume15"].ToString());
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error GetExportV15");
                Log.Error("Error" + ex.Message);
            }


            return ExportVolume15;

        }

        public IEnumerable<MTankImport> GetTankImportById(int id)
        {
            return tankImportRepository.GetMany(ti => ti.ImportInfoId == id);
        }

        public IEnumerable<MClockExport> GetAllClockExportById(int id)
        {
           // return clockExportRepository.GetMany(ce => ce.ImportInfoId == id && ce.StartVtt!=null && ce.EndVtt!=null);
            return clockExportRepository.GetMany(ce => ce.ImportInfoId == id  && ce.EndVtt > 0);
        }

        private void SaveImportInfo()
        {
            unitOfWork.Commit();
        }


        //var tanklog = tankService.GetTankLogByTime(tankImport.WareHouseCode, tankImport.TankId, tankImport.EndDate.Value);

        public bool Update_Start_ExportArmImport_By_DataLogArm(int importInfoId, float min_density, DateTime startDate)
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
                            cmd.CommandText = "Update_Start_MExportArmImport_From_DataLogArm_By_ImportInfoId";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("MIN_DENSITY", min_density);//Thừa, thôi để đó
                            SqlParameter param3 = new SqlParameter("StartDate", startDate);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Update_Start_ExportArmImport_By_DataLogArm");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool Update_End_ExportArmImport_By_DataLogArm(int importInfoId, float min_density, DateTime startDate)
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
                            cmd.CommandText = "Update_End_MExportArmImport_From_DataLogArm_By_ImportInfoId";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("MIN_DENSITY", min_density);//Thừa, thôi để đó
                            SqlParameter param3 = new SqlParameter("StartDate", startDate);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Update_End_ExportArmImport_By_DataLogArm");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool Update_Start_TankImport_From_TankLog(int importInfoId,float min_density,DateTime startDate)
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
                            cmd.CommandText = "Update_Start_TankImport_From_TankLog_By_ImportInfoId";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("MIN_DENSITY", min_density);
                            SqlParameter param3 = new SqlParameter("StartDate", startDate);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Update_Start_TankImport_From_TankLog");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public DataTable GetLastVttAndV15(int importInfoId)
        {
            DataTable dt = new DataTable();
            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "Report_WarehouseCard_By_Date_Tank_Import";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                        cmd.Parameters.Add(param);
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public bool Update_End_TankImport_From_TankLog(int importInfoId, float min_density, DateTime endDate)
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
                            cmd.CommandText = "Update_End_TankImport_From_TankLog_By_ImportInfoId";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("MIN_DENSITY", min_density);
                            SqlParameter param3 = new SqlParameter("EndDate", endDate);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.ExecuteNonQuery();
                            rs = true;


                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Update_End_TankImport_From_TankLog");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        #region Add TankImport bổ sung
        public bool CreateTankImport(MTankImport tankimport)
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
                            cmd.CommandText = "Insert_MTankImport_From_ImportInfo";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", tankimport.ImportInfoId);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", tankimport.WareHouseCode);
                            SqlParameter param3 = new SqlParameter("TankId", tankimport.TankId);
                            SqlParameter param4 = new SqlParameter("StartDensity", tankimport.StartDensity);
                            SqlParameter param5 = new SqlParameter("ExportFlg", tankimport.ExportFlg);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.Parameters.Add(param5);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error CreateTankImport");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }
        public bool CreateTankImportTemp(MTankImportTemp tankimporttemp)
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
                            cmd.CommandText = "Insert_MTankImportTemp_From_ImportInfo";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", tankimporttemp.ImportInfoId);
                            SqlParameter param2 = new SqlParameter("WareHouseCode", tankimporttemp.WareHouseCode);
                            SqlParameter param3 = new SqlParameter("TankId", tankimporttemp.TankId);
                            SqlParameter param4 = new SqlParameter("StartDensity", tankimporttemp.StartDensity);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
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


        public bool Add_ClockExports(int importInfoId )
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
                            cmd.CommandText = "Insert_ClockExport_By_ImportInfoId";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            cmd.Parameters.Add(param);
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

        public bool UpdateAdd_ExportArmImport(byte armno,byte warehousecode,int importInfoId,int productId)
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
                            cmd.CommandText = "InsertUpdate_ExportArmImport_By_ImportInfoId_ArmNo_WareHouseCode";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("ArmNo", armno);
                            SqlParameter param3 = new SqlParameter("WareHousecode", warehousecode);
                            SqlParameter param4 = new SqlParameter("ProductId", productId);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error UpdateAdd_ExportArmImport");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public bool Delete_ExportArmImport( int importInfoId)
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
                            cmd.CommandText = "Delete_ExportArmImport_By_ImportInfoId";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            cmd.Parameters.Add(param);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Delete_ExportArmImport");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }


        public bool Update_ClockExport_StarVtt_EndVtt(int importInfoId, int clockid, double start_Vtt, double end_Vtt)
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
                            cmd.CommandText = "Update_ClockExport_By_ImportInfoId_ClockId_StarVtt_EndVtt";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            SqlParameter param2 = new SqlParameter("ClockId", clockid);
                            SqlParameter param3 = new SqlParameter("StartVtt", start_Vtt);
                            SqlParameter param4 = new SqlParameter("EndVtt", end_Vtt);
                            cmd.Parameters.Add(param);
                            cmd.Parameters.Add(param2);
                            cmd.Parameters.Add(param3);
                            cmd.Parameters.Add(param4);
                            cmd.ExecuteNonQuery();
                            rs = true;
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Update_ClockExport_StarVtt_EndVtt");
                Log.Error("Error" + ex.Message);
            }

            return rs;
        }

        public string GetListArmName(int importInfoId)
        {
            string strArmName = string.Empty;
            
            try
            {
                using (var context = new PetroBMContext())
                {
                    using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "GetListArmName_ExportArmImport_By_ImportInfoId";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlParameter param = new SqlParameter("ImportInfoId", importInfoId);
                            cmd.Parameters.Add(param);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                strArmName += reader["ArmName"].ToString().Trim() + ", ";
                            }

                            if (strArmName.Length>2)
                            strArmName = strArmName.Remove(strArmName.Length - 2, 2);
                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error GetListArmName");
                Log.Error("Error" + ex.Message);
            }

            return strArmName;
        }



        #endregion

    }
}
