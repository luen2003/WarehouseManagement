using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IShipRepository : IRepository<MShip>
    {
    }
    public class ShipRepository : RepositoryBase<MShip>, IShipRepository
    {
        public ShipRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MShip ship)
        {
            if (String.IsNullOrEmpty(ship.InsertUser))
            {
                ship.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(ship.UpdateUser))
            {
                ship.UpdateUser = Constants.NULL;
            }

            ship.InsertDate = DateTime.Now;
            ship.UpdateDate = DateTime.Now;
            ship.VersionNo = Constants.VERSION_START;
            ship.DeleteFlg = Constants.FLAG_OFF;
            base.Add(ship);
        }

        public override void Update(MShip ship)
        {
            if (String.IsNullOrEmpty(ship.InsertUser))
            {
                ship.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(ship.UpdateUser))
            {
                ship.UpdateUser = Constants.NULL;
            }

            ship.UpdateDate = DateTime.Now;
            ship.VersionNo += 1;
            base.Update(ship);
        }
    }
}
