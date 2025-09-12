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
    public interface IUserGroupRepository : IRepository<MUserGrp>
    {
    }

    public class UserGroupRepository : RepositoryBase<MUserGrp>, IUserGroupRepository
    {
        public UserGroupRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public override void Add(MUserGrp userGrp)
        {
            if (String.IsNullOrEmpty(userGrp.InsertUser))
            {
                userGrp.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(userGrp.UpdateUser))
            {
                userGrp.UpdateUser = Constants.NULL;
            }

            userGrp.InsertDate = DateTime.Now;
            userGrp.UpdateDate = DateTime.Now;
            userGrp.VersionNo = Constants.VERSION_START;
            userGrp.DeleteFlg = Constants.FLAG_OFF;
            base.Add(userGrp);
        }

        public override void Update(MUserGrp userGrp)
        {
            if (String.IsNullOrEmpty(userGrp.InsertUser))
            {
                userGrp.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(userGrp.UpdateUser))
            {
                userGrp.UpdateUser = Constants.NULL;
            }

            userGrp.UpdateDate = DateTime.Now;
            userGrp.VersionNo += 1;
            base.Update(userGrp);
        }
    }
}
