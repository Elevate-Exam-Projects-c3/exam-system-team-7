using exam_system.Domain.Entities.Quizzes;
using exam_system.Dtos.Options;
using exam_system.Dtos.Questions;
using exam_system.Features.Questions.AdminGetQuestion.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Questions.AdminGetQuestion.Handlers {
    public class GetQuizQuestionsQueryHandler : IRequestHandler<GetQuizQuestionsQuery, RequestResponse<List<QuestionDto>>> {

        private readonly IGenericRepository<Question> questionRepository;

        public GetQuizQuestionsQueryHandler(IGenericRepository<Question> questionRepository) {
            this.questionRepository = questionRepository;
        }


        public async Task<RequestResponse<List<QuestionDto>>> Handle(GetQuizQuestionsQuery request, CancellationToken cancellationToken) {

            var questions = await questionRepository.GetAll()

                .Where (q => q.QuizId == request.QuizId && !q.IsDeleted)

                .Select (q => new QuestionDto {

                    Id = q.Id,
                    Text = q.Text,
                    Options = q.Options

                    .Where(o => !o.IsDeleted)

                    .Select(o => new QuizOptionDto { 
                        Id = o.Id,
                        OptionText = o.OptionText

                    }).ToList()
                }).ToListAsync();

            if (questions.Count == 0)
                return RequestResponse<List<QuestionDto>>.Ok(questions, "Questions list is empty.");

            return RequestResponse<List<QuestionDto>>.Ok (questions, "Questions loaded successfully.");
        }
    }
}
