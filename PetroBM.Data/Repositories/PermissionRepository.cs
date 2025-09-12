using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Data.Repositories
{
    public interface IPermissionRepository : IRepository<MPermission>
    {
    }

    public class PermissionRepository : RepositoryBase<MPermission>, IPermissionRepository
    {
        public PermissionRepository(IDbFactory dbFactory)
            : base(dbFactory) { }        
    }
}
