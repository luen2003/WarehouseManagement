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
    public interface IExportArmImportRepository : IRepository<MExportArmImport>
    {
    }
    public class ExportArmImportRepository : RepositoryBase<MExportArmImport>, IExportArmImportRepository
    {
        public ExportArmImportRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }


        public override void Add(MExportArmImport exportarmimport)
        {
            base.Add(exportarmimport);
        }

        public override void Update(MExportArmImport exportarmimport)
        {
            base.Update(exportarmimport);
        }
    }
}

