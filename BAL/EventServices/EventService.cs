using DAL.Core.IConfiguration;
using DAL.Models;

namespace BAL.EventServices
{
    public class EventService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<Event> CreateEvent(Event _event, Guid companyId)
        {
            _event.CompanyId = companyId;

            await _unitOfWork.Events.AddAsync(_event);
            await _unitOfWork.CommitAsync();
            return _event;
        }


        public async Task<IEnumerable<Event>> GetEventsByCompanyId(Guid companyId)
        {
            var result = await _unitOfWork.Events.GetEventsByCompanyIdAsync(companyId);
            return result;
        }

        public async Task<Event> UpdateEvent(Event newEvent,Event eventToBeUpdated)
        {
            eventToBeUpdated.EventDate = newEvent.EventDate;
            eventToBeUpdated.EventName = newEvent.EventName;
            eventToBeUpdated.IsActive = newEvent.IsActive;

            await _unitOfWork.CommitAsync();
            return eventToBeUpdated;
        }

        public async Task<Event> GetEventById(Guid eventId)
        {
            var result = await _unitOfWork.Events.GetByIdAsync(eventId);
            return result;
        }

        public async Task<Event> DeleteEvent(Event eventToBeDeleted)
        {
            _unitOfWork.Events.Remove(eventToBeDeleted);
            return eventToBeDeleted;
        }


    }
}
