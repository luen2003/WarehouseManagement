using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;
using System.Data.Entity.Core.Objects;

namespace PetroBM.Data.Repositories
{
    public interface ITankRepository : IRepository<MTank>
    {
        ObjectResult<ReportTankExport> ReportTankExport(DateTime? startDate, DateTime? endDate, byte? wareHouseCode, byte? armNo, string productCode, string vehicle, string customerName, byte? typeExport);
    }
    public class TankRepository : RepositoryBase<MTank>, ITankRepository
    {
        public TankRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public ObjectResult<ReportTankExport> ReportTankExport(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, 
            Nullable<byte> wareHouseCode, Nullable<byte> armNo, string productCode, string vehicle, string customerName, Nullable<byte> typeExport)
        {
            ObjectResult objResult = base.dataContext.Report_TankExport(startDate, endDate, wareHouseCode, armNo, productCode, vehicle, customerName, typeExport);
            //var listResult = objResult.GetNextResult().
            return null;
        }

        public override void Add(MTank tank)
        {
            if (String.IsNullOrEmpty(tank.InsertUser))
            {
                tank.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(tank.UpdateUser))
            {
                tank.UpdateUser = Constants.NULL;
            }

            tank.InsertDate = DateTime.Now;
            tank.UpdateDate = DateTime.Now;
            tank.VersionNo = Constants.VERSION_START;
            tank.DeleteFlg = Constants.FLAG_OFF;
            base.Add(tank);
        }

        public override void Update(MTank tank)
        {
            if (String.IsNullOrEmpty(tank.InsertUser))
            {
                tank.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(tank.UpdateUser))
            {
                tank.UpdateUser = Constants.NULL;
            }

            tank.UpdateDate = DateTime.Now;
            tank.VersionNo += 1;
            base.Update(tank);
        }
    }
}
