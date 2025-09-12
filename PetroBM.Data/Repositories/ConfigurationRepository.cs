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
    public interface IConfigurationRepository : IRepository<WSystemSetting>
    {
    }

    public class ConfigurationRepository : RepositoryBase<WSystemSetting>, IConfigurationRepository
    {
        public ConfigurationRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }
}
