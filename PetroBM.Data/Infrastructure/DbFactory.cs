using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        PetroBMContext dbContext;

        public PetroBMContext Init()
        {
            return dbContext ?? (dbContext = new PetroBMContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
