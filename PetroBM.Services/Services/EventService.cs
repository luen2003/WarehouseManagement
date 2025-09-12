using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PetroBM.Domain.Entities;
using PetroBM.Data.Repositories;
using PetroBM.Data.Infrastructure;
using PetroBM.Common.Util;

namespace PetroBM.Services.Services
{
    public interface IEventService
    {
        IEnumerable<MEvent> GetAllEvent();
        IEnumerable<MEvent> GetTopEvent(int count);
        IEnumerable<MEvent> GetEventByTime(DateTime? startDate, DateTime? endDate, string user, int? eventTypeId);
        bool CreateEvent(int typeId, string eventName, string insertUser);

        IEnumerable<MEventType> GetAllEventType();
    }

    public class EventService : IEventService
    {
        private readonly IEventRepository eventRepository;
        private readonly IEventTypeRepository eventTypeRepository;
        private readonly IUnitOfWork unitOfWork;

        public EventService(IEventRepository eventRepository, IEventTypeRepository eventTypeRepository, IUnitOfWork unitOfWork)
        {
            this.eventRepository = eventRepository;
            this.eventTypeRepository = eventTypeRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IEventService Members

        public IEnumerable<MEvent> GetAllEvent()
        {
            return eventRepository.GetAll().OrderByDescending(ev => ev.InsertDate);
        }

        public IEnumerable<MEvent> GetTopEvent(int count)
        {
            return GetAllEvent().Take(count);
        }

        public IEnumerable<MEvent> GetEventByTime(DateTime? startDate, DateTime? endDate, string user, int? eventTypeId)
        {
            IEnumerable<MEvent> rs = null;

            if (String.IsNullOrEmpty(user) && eventTypeId == null)
            {
                if (startDate == null && endDate == null)
                {
                    rs = GetAllEvent();
                }
                else if (startDate == null)
                {
                    rs = eventRepository.GetMany(ev => ev.InsertDate <= endDate)
                         .OrderByDescending(ev => ev.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = eventRepository.GetMany(ev => startDate <= ev.InsertDate)
                         .OrderByDescending(ev => ev.InsertDate);
                }
                else
                {
                    rs = eventRepository.GetMany(ev => (startDate <= ev.InsertDate) && (ev.InsertDate <= endDate))
                         .OrderByDescending(ev => ev.InsertDate);
                }
            }
            else if (String.IsNullOrEmpty(user))
            {
                if (startDate == null && endDate == null)
                {
                    rs = eventRepository.GetMany(ev => ev.MEventType.TypeId == eventTypeId)
                        .OrderByDescending(ev => ev.InsertDate);
                }
                else if (startDate == null)
                {
                    rs = eventRepository.GetMany(ev => (ev.MEventType.TypeId == eventTypeId) && (ev.InsertDate < endDate))
                    .OrderByDescending(ev => ev.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = eventRepository.GetMany(ev => (ev.MEventType.TypeId == eventTypeId) && (startDate < ev.InsertDate))
                    .OrderByDescending(ev => ev.InsertDate);
                }
                else
                {
                    rs = eventRepository.GetMany(ev => (ev.MEventType.TypeId == eventTypeId)
                    && (startDate < ev.InsertDate) && (ev.InsertDate < endDate))
                    .OrderByDescending(ev => ev.InsertDate);
                }
            }
            else if (eventTypeId == null)
            {
                if (startDate == null && endDate == null)
                {
                    rs = eventRepository.GetMany(ev => ev.InsertUser == user)
                        .OrderByDescending(ev => ev.InsertDate);
                }
                else if (startDate == null)
                {
                    rs = eventRepository.GetMany(ev => (ev.InsertUser == user) && (ev.InsertDate < endDate))
                    .OrderByDescending(ev => ev.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = eventRepository.GetMany(ev => (ev.InsertUser == user) && (startDate < ev.InsertDate))
                    .OrderByDescending(ev => ev.InsertDate);
                }
                else
                {
                    rs = eventRepository.GetMany(ev => (ev.InsertUser == user)
                    && (startDate < ev.InsertDate) && (ev.InsertDate < endDate))
                    .OrderByDescending(ev => ev.InsertDate);
                }
            }
            else
            {
                if (startDate == null && endDate == null)
                {
                    rs = eventRepository.GetMany(ev => (ev.InsertUser == user) && (ev.MEventType.TypeId == eventTypeId))
                        .OrderByDescending(ev => ev.InsertDate);
                }
                else if (startDate == null)
                {
                    rs = eventRepository.GetMany(ev => (ev.InsertUser == user) && (ev.MEventType.TypeId == eventTypeId) 
                    && (ev.InsertDate < endDate))
                    .OrderByDescending(ev => ev.InsertDate);
                }
                else if (endDate == null)
                {
                    rs = eventRepository.GetMany(ev => (ev.InsertUser == user) && (ev.MEventType.TypeId == eventTypeId)
                    && (startDate < ev.InsertDate))
                    .OrderByDescending(ev => ev.InsertDate);
                }
                else
                {
                    rs = eventRepository.GetMany(ev => (ev.InsertUser == user) && (ev.MEventType.TypeId == eventTypeId)
                    && (startDate < ev.InsertDate) && (ev.InsertDate < endDate))
                    .OrderByDescending(ev => ev.InsertDate);
                }
            }


            return rs;
        }

        public bool CreateEvent(int typeId, string eventName, string insertUser)
        {
            var rs = true;

            try
            {
                var ev = new MEvent();
                ev.TypeId = typeId;
                ev.EventName = eventName;
                ev.InsertUser = insertUser;
                eventRepository.Add(ev);
                SaveEvent();
            }
            catch
            {
                rs = false;
            }

            return rs;
        }

        public IEnumerable<MEventType> GetAllEventType()
        {
            return eventTypeRepository.GetAll();
        }

        private void SaveEvent()
        {
            unitOfWork.Commit();
        }
        #endregion
    }
}
