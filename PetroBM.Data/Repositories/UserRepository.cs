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
    public interface IUserRepository : IRepository<MUser>
    {
        MUser getUserLogged(string username, string pass);
    }

    public class UserRepository : RepositoryBase<MUser>, IUserRepository
    {
        public UserRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MUser user)
        {
            if (String.IsNullOrEmpty(user.InsertUser))
            {
                user.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(user.UpdateUser))
            {
                user.UpdateUser = Constants.NULL;
            }

            user.InsertDate = DateTime.Now;
            user.UpdateDate = DateTime.Now;
            user.VersionNo = Constants.VERSION_START;
            user.DeleteFlg = Constants.FLAG_OFF;
            base.Add(user);
        }

        public override void Update(MUser user)
        {
            if (String.IsNullOrEmpty(user.InsertUser))
            {
                user.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(user.UpdateUser))
            {
                user.UpdateUser = Constants.NULL;
            }

            user.UpdateDate = DateTime.Now;
            user.VersionNo += 1;
            base.Update(user);
        }

        public MUser getUserLogged(string username, string pass)
        {
            var user = this.DbContext.MUsers.Where(u => (u.UserName == username) && (u.Passwd == pass)).SingleOrDefault();

            return user;
        }
    }

}
