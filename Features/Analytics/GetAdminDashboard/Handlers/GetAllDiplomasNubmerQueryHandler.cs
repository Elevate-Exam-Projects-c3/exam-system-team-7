using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Analytics.GetAdminDashboard.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Analytics.GetAdminDashboard.Handlers
{
    public class GetAllDiplomasNubmerQueryHandler : IRequestHandler<GetAllDiplomasNubmerQuery, double>
    {
        private readonly IGenericRepository<Diploma> _genericRepository;
        public GetAllDiplomasNubmerQueryHandler(IGenericRepository<Diploma> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<double> Handle(GetAllDiplomasNubmerQuery request, CancellationToken cancellationToken)
        {
          int numberOfAllDiplomas= await _genericRepository.GetAll().AsNoTracking().Where(e => e.IsDeleted == false).CountAsync();
            return numberOfAllDiplomas;
        }
    }
}
