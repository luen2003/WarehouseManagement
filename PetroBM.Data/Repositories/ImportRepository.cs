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
    public interface IImportRepository : IRepository<MImportInfo>
    {
    }

    public class ImportRepository : RepositoryBase<MImportInfo>, IImportRepository
    {
        public ImportRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public override void Add(MImportInfo importInfo)
        {
            if (String.IsNullOrEmpty(importInfo.InsertUser))
            {
                importInfo.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(importInfo.UpdateUser))
            {
                importInfo.UpdateUser = Constants.NULL;
            }

            importInfo.StartFlag = Constants.FLAG_OFF;
            importInfo.EndFlag = Constants.FLAG_OFF;
            importInfo.InsertDate = DateTime.Now;
            importInfo.UpdateDate = DateTime.Now;
            importInfo.VersionNo = Constants.VERSION_START;
            importInfo.DeleteFlg = Constants.FLAG_OFF;
            base.Add(importInfo);
        }

        public override void Update(MImportInfo importInfo)
        {
            if (String.IsNullOrEmpty(importInfo.InsertUser))
            {
                importInfo.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(importInfo.UpdateUser))
            {
                importInfo.UpdateUser = Constants.NULL;
            }

            importInfo.UpdateDate = DateTime.Now;
            importInfo.VersionNo += 1;
            base.Update(importInfo);
        }
    }
}
