using exam_system.Domain.Entities.Quizzes;
using exam_system.Dtos.Quizes;
using exam_system.Features.Quizzes.AdminQuizPublishCheck.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminQuizPublishCheck.Handlers
{
    public class GetQuestionsCorrectOptionCountQueryHandler : IRequestHandler<GetQuestionsCorrectOptionCountQuery, RequestResponse<List<QuestionCorrectOptionCountDto>>>
    {
        private readonly IGenericRepository<Question> _questionRepository;

        public GetQuestionsCorrectOptionCountQueryHandler(IGenericRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public async Task<RequestResponse<List<QuestionCorrectOptionCountDto>>> Handle(GetQuestionsCorrectOptionCountQuery request, CancellationToken cancellationToken)
        {
            var result = await _questionRepository.GetAll().Where(q => q.QuizId == request.quizId && !q.IsDeleted)
                    .Select(q => new QuestionCorrectOptionCountDto
                    {
                        questionId = q.Id,
                        CorrectOptionCount = q.Options.Count(o => o.IsCorrect && !o.IsDeleted)
                    }).ToListAsync(cancellationToken);
            return RequestResponse<List<QuestionCorrectOptionCountDto>>.Ok(result);
        }
    }
}
