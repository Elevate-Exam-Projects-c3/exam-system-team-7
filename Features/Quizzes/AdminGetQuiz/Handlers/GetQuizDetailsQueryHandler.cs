using exam_system.Domain.Entities.Quizzes;
using exam_system.Dtos.Quizes;
using exam_system.Features.Quizzes.AdminGetQuiz.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminGetQuiz.Handlers {
    public class GetQuizDetailsQueryHandler : IRequestHandler<GetQuizDetailsQuery, RequestResponse<QuizDetailsDto>> {

        private readonly IGenericRepository<Quiz> quizRepository;
        public GetQuizDetailsQueryHandler(IGenericRepository<Quiz> quizRepository) {
            this.quizRepository = quizRepository;
        }
        public async Task<RequestResponse<QuizDetailsDto>> Handle(GetQuizDetailsQuery request, CancellationToken cancellationToken) {

            var quiz = await quizRepository.GetAll().FirstOrDefaultAsync(x => x.Id == request.QuizId);

            if (quiz == null) 
                return RequestResponse<QuizDetailsDto>.Fail("Quiz not found");
            
            var quizDetailsDto = new QuizDetailsDto {
                Id = quiz.Id,
                Title = quiz.Title,
                Instructions = quiz?.Instructions,
                Status = quiz?.Status,
                DurationMinutes = quiz?.DurationMinutes,
                StartTime = quiz?.StartDate,
                EndTime = quiz?.EndDate,
                MaxAttempts = quiz?.MaxAttempts
            };
            return RequestResponse<QuizDetailsDto>.Ok(quizDetailsDto);
        }
    }
}
