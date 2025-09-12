using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ILocationRepository : IRepository<MLocation>
    {
    }
    public class LocationRepository : RepositoryBase<MLocation>, ILocationRepository
    {
        public LocationRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MLocation location)
        {
            if (String.IsNullOrEmpty(location.InsertUser))
            {
                location.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(location.UpdateUser))
            {
                location.UpdateUser = Constants.NULL;
            }

            location.InsertDate = DateTime.Now;
            location.UpdateDate = DateTime.Now;
            location.VersionNo = Constants.VERSION_START;
            location.DeleteFlg = Constants.FLAG_OFF;
            base.Add(location);
        }

        public override void Update(MLocation location)
        {
            if (String.IsNullOrEmpty(location.InsertUser))
            {
                location.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(location.UpdateUser))
            {
                location.UpdateUser = Constants.NULL;
            }

            location.UpdateDate = DateTime.Now;
            location.VersionNo += 1;
            base.Update(location);
        }
    }
}