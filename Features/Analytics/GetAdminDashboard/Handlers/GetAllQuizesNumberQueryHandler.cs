using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Analytics.GetAdminDashboard.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Analytics.GetAdminDashboard.Handlers
{
    public class GetAllQuizesNumberQueryHandler : IRequestHandler<GetAllQuizesNumberQuery, double>
    {
        private readonly IGenericRepository<Quiz> _genericRepository;
        public GetAllQuizesNumberQueryHandler(IGenericRepository<Quiz> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<double> Handle(GetAllQuizesNumberQuery request, CancellationToken cancellationToken)
        {
          var numberOfAllActiveQuizes = await _genericRepository.GetAll().AsNoTracking().Where(e=>e.Status== QuizStatus.Published).CountAsync();

            return numberOfAllActiveQuizes;
        }
    }
}
