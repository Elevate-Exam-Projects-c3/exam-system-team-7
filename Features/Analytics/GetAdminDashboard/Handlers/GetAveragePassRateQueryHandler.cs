using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Analytics.GetAdminDashboard.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Analytics.GetAdminDashboard.Handlers
{
    public class GetAveragePassRateQueryHandler : IRequestHandler<GetAveragePassRateQuery, double>
    {
        private readonly IGenericRepository<QuizAttempt> _genericRepository;
        public GetAveragePassRateQueryHandler(IGenericRepository<QuizAttempt> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<double> Handle(GetAveragePassRateQuery request, CancellationToken cancellationToken)
        {
           var totalAttepts= await _genericRepository.GetAll().AsNoTracking()
                .Where(e => e.Status == AttemptStatus.Submitted)
                .CountAsync();
            if (totalAttepts == 0)
            {
                return 0;
            }
            var passedAttempts = await _genericRepository.GetAll()
                .Where(e => e.Status == AttemptStatus.Submitted && e.Passed == true).CountAsync();


            return passedAttempts/totalAttepts *100;
        }
    }

}
