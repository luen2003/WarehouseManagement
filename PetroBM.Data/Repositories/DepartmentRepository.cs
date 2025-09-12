using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;

namespace PetroBM.Data.Repositories
{
    public interface IDepartmentRepository : IRepository<MDepartment>
    {
    }
    public class DepartmentRepository : RepositoryBase<MDepartment>, IDepartmentRepository
    {
        public DepartmentRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public override void Add(MDepartment department)
        {
            if (String.IsNullOrEmpty(department.InsertUser))
            {
                department.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(department.UpdateUser))
            {
                department.UpdateUser = Constants.NULL;
            }

            department.InsertDate = DateTime.Now;
            department.UpdateDate = DateTime.Now;
            department.VersionNo = Constants.VERSION_START;
            department.DeleteFlg = Constants.FLAG_OFF;
            base.Add(department);
        }

        public override void Update(MDepartment department)
        {
            if (String.IsNullOrEmpty(department.InsertUser))
            {
                department.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(department.UpdateUser))
            {
                department.UpdateUser = Constants.NULL;
            }

            department.UpdateDate = DateTime.Now;
            department.VersionNo += 1;
            base.Update(department);
        }
    }
}