using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IDriverRepository : IRepository<MDriver>
    {
    }
    public class DriverRepository : RepositoryBase<MDriver>, IDriverRepository
    {
        public DriverRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MDriver driver)
        {
            if (String.IsNullOrEmpty(driver.InsertUser))
            {
                driver.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(driver.UpdateUser))
            {
                driver.UpdateUser = Constants.NULL;
            }

            driver.InsertDate = DateTime.Now;
            driver.UpdateDate = DateTime.Now;
            driver.VersionNo = Constants.VERSION_START;
            driver.DeleteFlg = Constants.FLAG_OFF;
            base.Add(driver);
        }

        public override void Update(MDriver driver)
        {
            if (String.IsNullOrEmpty(driver.InsertUser))
            {
                driver.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(driver.UpdateUser))
            {
                driver.UpdateUser = Constants.NULL;
            }

            driver.UpdateDate = DateTime.Now;
            driver.VersionNo += 1;
            base.Update(driver);
        }
    }
}
