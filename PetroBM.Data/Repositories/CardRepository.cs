using PetroBM.Common.Util;
using PetroBM.Data.Infrastructure;
using PetroBM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetroBM.Data.Repositories
{
    public interface ICardRepository : IRepository<MCard>
    {
    }
    public class CardRepository : RepositoryBase<MCard>, ICardRepository
    {
        public CardRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
        public override void Add(MCard card)
        {
            if (String.IsNullOrEmpty(card.InsertUser))
            {
                card.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(card.UpdateUser))
            {
                card.UpdateUser = Constants.NULL;
            }

            card.InsertDate = DateTime.Now;
            card.UpdateDate = DateTime.Now;
            card.VersionNo = Constants.VERSION_START;
            card.DeleteFlg = Constants.FLAG_OFF;
            base.Add(card);
        }

        public override void Update(MCard card)
        {
            if (String.IsNullOrEmpty(card.InsertUser))
            {
                card.InsertUser = Constants.NULL;
            }

            if (String.IsNullOrEmpty(card.UpdateUser))
            {
                card.UpdateUser = Constants.NULL;
            }

            card.UpdateDate = DateTime.Now;
            card.VersionNo += 1;
            base.Update(card);
        }
    }
}
