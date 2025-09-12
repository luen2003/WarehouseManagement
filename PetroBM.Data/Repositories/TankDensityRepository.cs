using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;

namespace PetroBM.Data.Repositories
{
    public interface ITankDensityRepository : IRepository<MTankDensity>
    {
    }
    public class TankDensityRepository : RepositoryBase<MTankDensity>, ITankDensityRepository
    {
        public TankDensityRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
