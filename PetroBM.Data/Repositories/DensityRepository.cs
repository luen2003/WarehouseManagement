using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IDensityRepository : IRepository<MDensity>
    {
    }
    public class DensityRepository : RepositoryBase<MDensity>, IDensityRepository
    {
        public DensityRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MDensity density)
        {
            if (String.IsNullOrEmpty(density.InsertUser))
            {
                density.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(density.UpdateUser))
            {
                density.UpdateUser = Constants.NULL;
            }

            density.InsertDate = DateTime.Now;
            density.UpdateDate = DateTime.Now;
            density.VersionNo = Constants.VERSION_START;
            density.DeleteFlg = Constants.FLAG_OFF;
            base.Add(density);
        }

        public override void Update(MDensity density)
        {
            if (String.IsNullOrEmpty(density.InsertUser))
            {
                density.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(density.UpdateUser))
            {
                density.UpdateUser = Constants.NULL;
            }

            density.UpdateDate = DateTime.Now;
            density.VersionNo += 1;
            base.Update(density);
        }
    }
}