using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;

namespace PetroBM.Data.Repositories
{
    public interface ITankLogRepository : IRepository<MTankLog>
    {
    }
    public class TankLogRepository : RepositoryBase<MTankLog>, ITankLogRepository
    {
        public TankLogRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
