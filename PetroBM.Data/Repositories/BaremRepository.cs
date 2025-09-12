using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetroBM.Common.Util;

namespace PetroBM.Data.Repositories
{
    public interface IBaremRepository : IRepository<MBarem>
    {
    }

    public class BaremRepository : RepositoryBase<MBarem>, IBaremRepository
    {
        public BaremRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public override void Add(MBarem barem)
        {
            if (String.IsNullOrEmpty(barem.InsertUser))
            {
                barem.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(barem.UpdateUser))
            {
                barem.UpdateUser = Constants.NULL;
            }

            barem.InsertDate = DateTime.Now;
            barem.UpdateDate = DateTime.Now;
            barem.VersionNo = Constants.VERSION_START;
            barem.DeleteFlg = Constants.FLAG_OFF;
            base.Add(barem);
        }

        public override void Update(MBarem barem)
        {
            if (String.IsNullOrEmpty(barem.InsertUser))
            {
                barem.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(barem.UpdateUser))
            {
                barem.UpdateUser = Constants.NULL;
            }

            barem.UpdateDate = DateTime.Now;
            barem.VersionNo += 1;
            base.Update(barem);
        }
    }
}
