using log4net;
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;
using System.Data.Entity.Infrastructure;

namespace PetroBM.Data.Repositories
{
    public interface ICommandDetailRepository : IRepository<MCommandDetail>
    {
        int SPCalculate15CommandDetail();
    }
    public class CommandDetailRepository : RepositoryBase<MCommandDetail>, ICommandDetailRepository
    {
        ILog log = log4net.LogManager.GetLogger(typeof(CommandDetailRepository));
        public CommandDetailRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MCommandDetail commanddetail)
        {
            if (String.IsNullOrEmpty(commanddetail.InsertUser))
            {
                commanddetail.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(commanddetail.UpdateUser))
            {
                commanddetail.UpdateUser = Constants.NULL;
            }

            commanddetail.InsertDate = DateTime.Now;
            commanddetail.UpdateDate = DateTime.Now;
            commanddetail.VersionNo = Constants.VERSION_START;
            commanddetail.DeleteFlg = Constants.FLAG_OFF;
            base.Add(commanddetail);
        }

        public override void Update(MCommandDetail commanddetail)
        {
            if (String.IsNullOrEmpty(commanddetail.InsertUser))
            {
                commanddetail.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(commanddetail.UpdateUser))
            {
                commanddetail.UpdateUser = Constants.NULL;
            }

            commanddetail.UpdateDate = DateTime.Now;
            commanddetail.VersionNo += 1;
            base.Update(commanddetail);
        }

        public int SPCalculate15CommandDetail()
        {
            int result = 0;
            try{
                var objectContext = (dataContext as IObjectContextAdapter).ObjectContext;
                result = objectContext.ExecuteStoreCommand("SPCalculate15CommandDetail");
                //dataContext.SPCalculate15CommandDetail();
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
            return result;

        }
    }
}