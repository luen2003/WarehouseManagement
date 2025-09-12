
using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface ICommandRepository : IRepository<MCommand>
    {
    }
    public class CommandRepository : RepositoryBase<MCommand>, ICommandRepository
    {
        public CommandRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MCommand command)
        {
            if (String.IsNullOrEmpty(command.InsertUser))
            {
                command.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(command.UpdateUser))
            {
                command.UpdateUser = Constants.NULL;
            }

            command.InsertDate = DateTime.Now;
            command.UpdateDate = DateTime.Now;
            command.VersionNo = Constants.VERSION_START;
            command.DeleteFlg = Constants.FLAG_OFF;
            base.Add(command);
        }

        public override void Update(MCommand command)
        {
            if (String.IsNullOrEmpty(command.InsertUser))
            {
                command.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(command.UpdateUser))
            {
                command.UpdateUser = Constants.NULL;
            }

            command.UpdateDate = DateTime.Now;
            command.VersionNo += 1;
            base.Update(command);
        }
    }
}
