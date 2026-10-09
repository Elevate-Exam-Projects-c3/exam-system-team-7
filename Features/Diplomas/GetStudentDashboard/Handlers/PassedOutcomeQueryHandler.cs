using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Diplomas.GetStudentDashboard.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Handlers
{
    public class PassedOutcomeQueryHandler : IRequestHandler<PassedOutcomeQuery, bool>
    {
        private readonly IGenericRepository<QuizAttempt> _repo;
        public PassedOutcomeQueryHandler(IGenericRepository<QuizAttempt> repo)
        {
            _repo = repo;
        }
        public async Task<bool> Handle(PassedOutcomeQuery request, CancellationToken cancellationToken)
        {
           var result = await _repo.GetAll().AsNoTracking().Where(e => e.StudentId == request.UserId).Select(e =>e.Passed).FirstOrDefaultAsync();

            return (bool)result;
        }
    }
}
