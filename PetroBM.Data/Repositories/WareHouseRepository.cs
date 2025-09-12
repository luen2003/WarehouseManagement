using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IWareHouseRepository : IRepository<MWareHouse>
    {
    }
    public class WareHouseRepository : RepositoryBase<MWareHouse>, IWareHouseRepository
    {
        public WareHouseRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MWareHouse warehouse)
        {
            if (String.IsNullOrEmpty(warehouse.InsertUser))
            {
                warehouse.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(warehouse.UpdateUser))
            {
                warehouse.UpdateUser = Constants.NULL;
            }

            warehouse.InsertDate = DateTime.Now;
            warehouse.UpdateDate = DateTime.Now;
            warehouse.VersionNo = Constants.VERSION_START;
            warehouse.DeleteFlg = Constants.FLAG_OFF;
            base.Add(warehouse);
        }

        public override void Update(MWareHouse warehouse)
        {
            if (String.IsNullOrEmpty(warehouse.InsertUser))
            {
                warehouse.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(warehouse.UpdateUser))
            {
                warehouse.UpdateUser = Constants.NULL;
            }

            warehouse.UpdateDate = DateTime.Now;
            warehouse.VersionNo += 1;
            base.Update(warehouse);
        }
    }
}