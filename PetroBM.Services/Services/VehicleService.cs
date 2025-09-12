using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Data.Repositories;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using log4net;
using PetroBM.Data;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Validation;

namespace PetroBM.Services.Services
{
    public interface IVehicleService
    {
        IEnumerable<MVehicle> GetAllVehicle();
        IEnumerable<MVehicle> GetAllVehicleOrderByName();
        IEnumerable<MVehicle> GetDriverNameByVehicleNumber(string vehiclenumber); 
        IEnumerable<MCard> GetCardDataByVehicleNumber(string vehiclenumber);

        bool Import(HttpPostedFileBase file, string user);
        bool CreateVehicle(MVehicle vehicle);
        bool UpdateVehicle(MVehicle vehicle);
        bool DeleteVehicle(int id);

        MVehicle FindVehicleById(int id);
        DataTable FindVehicle(string vehicle, string driverName, string CardSerial);
        List<MVehicle> ConvertDataTableToMVehicleList(DataTable dt);
    }

    public class VehicleService : IVehicleService
    {
        ILog log = log4net.LogManager.GetLogger(typeof(VehicleService));

        private readonly IVehicleRepository vehicleRepository;
        private readonly ICardRepository cardRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEventService eventService;

        public VehicleService(IVehicleRepository vehicleRepository, ICardRepository cardRepository, IUnitOfWork unitOfWork, IEventService eventService)
        {
            this.vehicleRepository = vehicleRepository;
            this.cardRepository = cardRepository;
            this.unitOfWork = unitOfWork;
            this.eventService = eventService;
        }

        public bool CreateVehicle(MVehicle vehicle)
        {
            var rs = false;
            log.Info("Start CreateVehicle");
            
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    vehicleRepository.Add(vehicle);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                    Constants.EVENT_CONFIG_VEHICEL_CREATE, vehicle.InsertUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish CreateVehicle");
            return rs;
        }

        public bool DeleteVehicle(int id)
        {
            MVehicle vehicle = this.FindVehicleById(id);
            var rs = false;
            log.Info("Start DeleteVehicle");
            
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                //{
                //    vehicle.DeleteFlg = Constants.FLAG_ON;
                //    vehicleRepository.Update(vehicle);
                //    unitOfWork.Commit();
                //    ts.Complete();

                //    rs = true;
                //}

                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        vehicle.DeleteFlg = Constants.FLAG_ON;
                        vehicleRepository.Update(vehicle);
                        unitOfWork.Commit();
                        ts.Complete(); 
                    }     

                    rs = true;
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Console.WriteLine("Property: {0} Error: {1}",
                                              validationError.PropertyName,
                                              validationError.ErrorMessage);
                        }
                    }

                    rs = false; // or handle the error as needed
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                    Constants.EVENT_CONFIG_VEHICEL_DELETE, vehicle.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish DeleteVehicle");

            return rs;
        }

        public IEnumerable<MVehicle> GetAllVehicle()
        {
            return vehicleRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderByDescending(p => p.InsertDate);
        }

        public IEnumerable<MVehicle> GetAllVehicleOrderByName()
        {
            return vehicleRepository.GetAll().Where(p => p.DeleteFlg == Constants.FLAG_OFF)
                .OrderBy(p => p.ID);
        }

        public bool UpdateVehicle(MVehicle vehicle)
        {
            var rs = false;
            log.Info("Start UpdateVehicle");
     
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    vehicleRepository.Update(vehicle);
                    unitOfWork.Commit();
                    ts.Complete();
                    rs = true;
                }

                // Log event
                eventService.CreateEvent(Constants.EVENT_TYPE_MANAGEMENT,
                   Constants.EVENT_CONFIG_VEHICEL_UPDATE, vehicle.UpdateUser);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish UpdateVehicle");
            return rs;
        }

        public MVehicle FindVehicleById(int id)
        {
            return vehicleRepository.GetById(id);
        }

        public IEnumerable<MVehicle> GetDriverNameByVehicleNumber(string vehiclenumber)
        {
            return vehicleRepository.GetAll().Where(p => p.VehicleNumber == vehiclenumber && p.DeleteFlg == false).OrderBy(p => p.ID);
        }

        public IEnumerable<MCard> GetCardDataByVehicleNumber(string VehicleNumber)
        {
            var vehicle = vehicleRepository.GetAll().Where(p => p.VehicleNumber == VehicleNumber && p.DeleteFlg == false).OrderBy(p => p.ID);
            log.Info("GetCardDataByVehicleNumber " + VehicleNumber);
            return cardRepository.GetAll().Join(vehicle, p => p.ID, v => v.CardID, (p, v) => new { p, v }).Where(x => x.v.VehicleNumber == VehicleNumber).OrderBy(x => x.p.ID).Select(x => x.p).ToList();
        }

        public bool Import(HttpPostedFileBase file, string user)
        {
            var rs = false;
            log.Info("Start Import");            
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(HttpContext.Current.Server.MapPath(Constants.FILE_PATH), fileName);
                    file.SaveAs(path);

                    var data = ExcelUtil.CommonReadFromExcelfile(path, Constants.Import_Column_Vehicle);                  

                    using (TransactionScope ts = new TransactionScope())
                    {
                        var lstVehicle = new List<MVehicle>();
                        foreach (System.Data.DataRow row in data.Rows)
                        {

                            var objectVehicle = new MVehicle();
                            objectVehicle.VehicleNumber = row.ItemArray[0].ToString();
                            objectVehicle.RegisterNumber = row.ItemArray[1].ToString();
                            objectVehicle.ExpireDate = Convert.ToDateTime(row.ItemArray[2]);
                            objectVehicle.AccreditationNumber = row.ItemArray[3].ToString();
                            objectVehicle.AccreditationExpire = Convert.ToDateTime(row.ItemArray[4]);
                            objectVehicle.Driverdefault = row.ItemArray[5].ToString();                            
                            objectVehicle.Volume1 = Convert.ToInt32(row.ItemArray[6]);
                            objectVehicle.Volume2 = Convert.ToInt32(row.ItemArray[7]);
                            objectVehicle.Volume3 = Convert.ToInt32(row.ItemArray[8]);
                            objectVehicle.Volume4 = Convert.ToInt32(row.ItemArray[9]);
                            objectVehicle.Volume5 = Convert.ToInt32(row.ItemArray[10]);
                            objectVehicle.Volume6 = Convert.ToInt32(row.ItemArray[11]);
                            objectVehicle.Volume7 = Convert.ToInt32(row.ItemArray[12]);
                            objectVehicle.Volume8 = Convert.ToInt32(row.ItemArray[13]);
                            objectVehicle.Volume9 = Convert.ToInt32(row.ItemArray[14]);
                            objectVehicle.CommandType = Byte.Parse(row.ItemArray[15].ToString());
                            objectVehicle.InsertUser = user;
                            objectVehicle.UpdateUser = user;
                            objectVehicle.InsertDate = DateTime.Now;
                            objectVehicle.UpdateDate = DateTime.Now;
                            objectVehicle.VersionNo = Constants.VERSION_START;
                            objectVehicle.DeleteFlg = Constants.FLAG_OFF;
                            objectVehicle.FirePreventLicense = row.ItemArray[16].ToString();
                            lstVehicle.Add(objectVehicle);
                        }

                        vehicleRepository.AddRange(lstVehicle);
                        unitOfWork.Commit();
                        ts.Complete();
                        rs = true;
                    }

                    // Log event
                    eventService.CreateEvent(Constants.EVENT_TYPE_CONFIGURATION,
                        Constants.EVENT_CONFIG_VEHICEL_IMPORT, user);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            log.Info("Finish Import");
            return rs;
        }

        public DataTable FindVehicle(string vehicle, string driverName, string cardSerial)
        {
            DataTable dt = new DataTable();

            using (var context = new PetroBMContext())
            {
                using (var conn = new SqlConnection(context.Database.Connection.ConnectionString))
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "FindVehicle";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param1 = new SqlParameter("Vehicle", vehicle ?? "");
                        SqlParameter param2 = new SqlParameter("DriverName", driverName ?? "");
                        SqlParameter param3 = new SqlParameter("CardSerial", cardSerial ?? ""); 

                        cmd.Parameters.Add(param1);
                        cmd.Parameters.Add(param2);
                        cmd.Parameters.Add(param3); 
                        using (var adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }

            return dt;
        }

        public List<MVehicle> ConvertDataTableToMVehicleList(DataTable dt)
        {
            return dt.AsEnumerable().Select(row => new MVehicle
            {
                ID = row.IsNull("ID") ? 0 : Convert.ToInt32(row["ID"]),
                VehicleNumber = row.IsNull("VehicleNumber") ? string.Empty : Convert.ToString(row["VehicleNumber"]),
                RegisterNumber = row.IsNull("RegisterNumber") ? string.Empty : Convert.ToString(row["RegisterNumber"]),

                // Kiểm tra DBNull trước khi parse DateTime
                ExpireDate = row.IsNull("ExpireDate") ? DateTime.MinValue : ParseDate(row["ExpireDate"]),
                FirePreventLicense = row.IsNull("FirePreventLicense") ? string.Empty : Convert.ToString(row["FirePreventLicense"]),
                FirePreventExpire = row.IsNull("FirePreventExpire") ? DateTime.MinValue : ParseDate(row["FirePreventExpire"]),
                AccreditationNumber = row.IsNull("AccreditationNumber") ? string.Empty : Convert.ToString(row["AccreditationNumber"]),
                AccreditationExpire = row.IsNull("AccreditationExpire") ? DateTime.MinValue : ParseDate(row["AccreditationExpire"]),
                Driverdefault = row.IsNull("Driverdefault") ? string.Empty : Convert.ToString(row["Driverdefault"]),
                CardID = row.IsNull("CardID") ? 0 : Convert.ToInt32(row["CardID"]),
                Volume1 = row.IsNull("Volume1") ? 0 : Convert.ToInt32(row["Volume1"]),
                Volume2 = row.IsNull("Volume2") ? 0 : Convert.ToInt32(row["Volume2"]),
                Volume3 = row.IsNull("Volume3") ? 0 : Convert.ToInt32(row["Volume3"]),
                Volume4 = row.IsNull("Volume4") ? 0 : Convert.ToInt32(row["Volume4"]),
                DeleteFlg = bool.Parse(row["DeleteFlg"].ToString())
                // Các thuộc tính khác của MVehicle nếu có
            }).ToList();
        }

        public string IsNumberNull(string value)
        {
            string result = "0";
            if(!string.IsNullOrEmpty(value))
            {
                result = value;
            }
            return result;
        }

        public string IsNull(string value)
        {
            string result = "";
            if (!string.IsNullOrEmpty(value))
            {
                result = value;
            }
            return result;
        }

        private DateTime ParseDate(object dateObj)
        {
            DateTime parsedDate;
            // Try parsing with a default format or handle multiple formats
            if (DateTime.TryParse(Convert.ToString(dateObj), out parsedDate))
            {
                return parsedDate;
            }
            // Handle invalid dates by returning a default date or handling it as needed
            return DateTime.MinValue; // Or handle it differently, e.g., throw an exception, return null, etc.
        }
    }
}