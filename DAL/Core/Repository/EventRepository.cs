using DAL.Core.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Core.Repository
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(PersonnelManagementDBContext context) : base(context) { }
        private PersonnelManagementDBContext PersonnelManagementDBContext => Context;

        public async Task<IEnumerable<Event>> GetEventsByCompanyIdAsync(Guid companyId)
        {
            var result = await PersonnelManagementDBContext.Events.Where(x => x.CompanyId == companyId).ToListAsync();
            return result;
        }




    }
}
