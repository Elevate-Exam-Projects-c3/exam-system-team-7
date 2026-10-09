using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Analytics.GetAdminDashboard.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Analytics.GetAdminDashboard.Handlers
{
    public class GetTotalAttemptsQueryHandler : IRequestHandler<GetTotalAttemptsQuery, double>
    {
        private readonly IGenericRepository<QuizAttempt> _genericRepository;
        public GetTotalAttemptsQueryHandler(IGenericRepository<QuizAttempt> genericRepository) 
        {
            _genericRepository = genericRepository;
        }
        public async Task<double> Handle(GetTotalAttemptsQuery request, CancellationToken cancellationToken)
        {
          int result = await _genericRepository.GetAll().AsNoTracking().Where(e => e.Status == AttemptStatus.Submitted).CountAsync();
            return result;
        }
    }
}
