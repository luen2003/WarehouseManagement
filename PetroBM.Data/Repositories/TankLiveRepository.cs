using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;

namespace PetroBM.Data.Repositories
{
    public interface ITankLiveRepository : IRepository<MTankLive>
    {
    }
    public class TankLiveRepository : RepositoryBase<MTankLive>, ITankLiveRepository
    {
        public TankLiveRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MTankLive tanklive)
        {
            base.Add(tanklive);
        }

        public override void Update(MTankLive tanklive)
        {
            base.Update(tanklive);
        }
    }
}
