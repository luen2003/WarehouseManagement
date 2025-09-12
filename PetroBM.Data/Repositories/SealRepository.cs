using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Data.Repositories
{
    public interface ISealRepository : IRepository<MSeal>
    {
    }
    public class SealRepository : RepositoryBase<MSeal>, ISealRepository
    {
        public SealRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
        public override void Add(MSeal seal)
        {
            if (String.IsNullOrEmpty(seal.InsertUser))
            {
                seal.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(seal.UpdateUser))
            {
                seal.UpdateUser = Constants.NULL;
            }

            seal.InsertDate = DateTime.Now;
            seal.UpdateDate = DateTime.Now;
            seal.VersionNo = Constants.VERSION_START;
            seal.DeleteFlg = Constants.FLAG_OFF;
            base.Add(seal);
        }

        public override void Update(MSeal seal)
        {
            if (String.IsNullOrEmpty(seal.InsertUser))
            {
                seal.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(seal.UpdateUser))
            {
                seal.UpdateUser = Constants.NULL;
            }

            seal.UpdateDate = DateTime.Now;
            seal.VersionNo += 1;
            base.Update(seal);
        }
    }

}

