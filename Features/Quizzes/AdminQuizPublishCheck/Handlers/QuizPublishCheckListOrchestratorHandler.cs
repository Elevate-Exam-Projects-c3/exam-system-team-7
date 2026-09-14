using exam_system.Dtos.Quizes;
using exam_system.Features.Questions.AdminGetQuestion.Queries;
using exam_system.Features.Quizzes.AdminQuizPublishCheck.Orchestrators;
using exam_system.Features.Quizzes.AdminQuizPublishCheck.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Quizzes.AdminQuizPublishCheck.Handlers
{
    public class QuizPublishCheckListOrchestratorHandler : IRequestHandler<QuizPublishCheckListOrchestrator, RequestResponse<QuizPublishCheckDto>>
    {
        private readonly IMediator _mediator;

        public QuizPublishCheckListOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<RequestResponse<QuizPublishCheckDto>> Handle(QuizPublishCheckListOrchestrator request, CancellationToken cancellationToken)
        {
            // var existsResult = await _mediator.Send(new CheckIfQuizExistQuery(Guid quizId),Cancellationtoken);
            //if (!existsResult)
            //    return RequestResponse<QuizPublishCheckDto>.Fail("Quiz does not exist");
            //assume it exists

            var InfoResult = await _mediator.Send(new GetQuizInfoQuery(request.quizId), cancellationToken);
            var questionsResult = await _mediator.Send(new GetQuestionsCorrectOptionCountQuery(request.quizId), cancellationToken);

            var QuizPublishDto = new QuizPublishCheckDto
            {
                Checks = new List<PublishCheckItem>
                {
                    CheckIfHasAtLeastOneQuestion(questionsResult.Data),
                    CheckDurationMinutesIsValid(InfoResult.Data),
                    CheckEachQuestionHasExactlyOneCorrectOption(questionsResult.Data),
                    CheckPassScoreIsValid(InfoResult.Data)

                }
            };

            return RequestResponse<QuizPublishCheckDto>.Ok(QuizPublishDto);

        }
    private static PublishCheckItem CheckIfHasAtLeastOneQuestion(List<QuestionCorrectOptionCountDto> questions)
        {
            var passed = questions.Count() > 0;
            return new PublishCheckItem
            {
                CheckName = "HasAtLeastOneQuestion",
                IsPassed = passed,
                Message = passed ? $"Quiz has {questions.Count} questions" : "Quiz must has at least one question!"
            };
        }

        private static PublishCheckItem CheckEachQuestionHasExactlyOneCorrectOption(List<QuestionCorrectOptionCountDto> questions)
        {
            var problematicIds = questions.Where(q => q.CorrectOptionCount != 1).Select(q => q.questionId).ToList();

            return new PublishCheckItem
            {
                CheckName = "EachQuestionHasExactlyOneCorrectOption",
                IsPassed = problematicIds.Count == 0,
                Message = problematicIds.Count == 0
                    ? "All questions have exactly one correct option marked."
                    : $"{problematicIds.Count} question(s) don't have exactly one correct option marked.",
                RelatedItems = problematicIds.Count > 0 ? problematicIds : null
            };
        }

        private static PublishCheckItem CheckDurationMinutesIsValid(QuizInfoDto schedule)
        {
            var passed = schedule.durationminutes > 0;
            return new PublishCheckItem
            {
                CheckName = "DurationMinutesIsValid",
                IsPassed = passed,
                Message = passed ? "Duration is valid." : "DurationMinutes must be greater than 0."
            };
        }

        private static PublishCheckItem CheckPassScoreIsValid(QuizInfoDto schedule)
        {
            var passed = schedule.passScore.HasValue && schedule.passScore >= 0 && schedule.passScore <= 100;
            return new PublishCheckItem
            {
                CheckName = "PassScoreIsValid",
                IsPassed = passed,
                Message = passed ? "Pass score is within range." : "PassScore must be between 0 and 100."
            };
        }
    }




    }
