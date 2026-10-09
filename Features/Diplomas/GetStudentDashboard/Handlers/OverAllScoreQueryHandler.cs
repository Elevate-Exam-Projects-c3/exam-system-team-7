using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Diplomas.GetStudentDashboard.Queries;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.GetStudentDashboard.Handlers
{
    public class OverAllScoreQueryHandler : IRequestHandler<OverAllScoreQuery, double>
    {
        private readonly IGenericRepository<QuizAttempt> _repo;
        public OverAllScoreQueryHandler(IGenericRepository<QuizAttempt> repo)
        {
            _repo = repo;
        }
        public async Task<double> Handle(OverAllScoreQuery request, CancellationToken cancellationToken)
        {
         var result =  await _repo.GetAll().AsNoTracking().Where(e => e.StudentId == request.UserId && e.Status == AttemptStatus.Submitted).SumAsync(e => e.Score);
            return (double)result;
        }
    }
}
