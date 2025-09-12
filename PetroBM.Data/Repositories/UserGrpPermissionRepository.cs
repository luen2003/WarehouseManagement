using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Data.Repositories
{
    public interface IUserGrpPermissionRepository : IRepository<MUserGrpPermission>
    {
    }

    public class UserGrpPermissionRepository : RepositoryBase<MUserGrpPermission>, IUserGrpPermissionRepository
    {
        public UserGrpPermissionRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
