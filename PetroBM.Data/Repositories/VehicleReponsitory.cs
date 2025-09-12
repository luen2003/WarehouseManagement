using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IVehicleRepository : IRepository<MVehicle>
    {
    }
    public class VehicleRepository : RepositoryBase<MVehicle>, IVehicleRepository
    {
        public VehicleRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MVehicle vehicle)
        {
            if (String.IsNullOrEmpty(vehicle.InsertUser))
            {
                vehicle.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(vehicle.UpdateUser))
            {
                vehicle.UpdateUser = Constants.NULL;
            }

            vehicle.InsertDate = DateTime.Now;
            vehicle.UpdateDate = DateTime.Now;
            vehicle.VersionNo = Constants.VERSION_START;
            vehicle.DeleteFlg = Constants.FLAG_OFF;
            base.Add(vehicle);
        }

        public override void Update(MVehicle vehicle)
        {
            if (String.IsNullOrEmpty(vehicle.InsertUser))
            {
                vehicle.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(vehicle.UpdateUser))
            {
                vehicle.UpdateUser = Constants.NULL;
            }

            vehicle.UpdateDate = DateTime.Now;
            vehicle.VersionNo += 1;
            base.Update(vehicle);
        }
    }
}
