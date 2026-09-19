using exam_system.Common.Enums;
using exam_system.Dtos.Quizes;
using exam_system.Features.Attempts.GetOldAttempt.Queries;
using exam_system.Features.Attempts.StartAttempt.Commands;
using exam_system.Features.Attempts.StartAttempt.Orchestrators;
using exam_system.Features.Questions.AdminGetQuestion.Queries;
using exam_system.Features.Quizzes.AdminGetQuiz.Queries;
using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Handlers {
    public class StartQuizOrchestratorHandler: IRequestHandler<StartQuizOrchestrator,RequestResponse<StartQuizDto>> {

        private readonly IMediator mediator;

        public StartQuizOrchestratorHandler(IMediator mediator) {
            this.mediator = mediator;
        }

        public async Task<RequestResponse<StartQuizDto>> Handle(StartQuizOrchestrator request,CancellationToken cancellationToken) {

            // 1. Get Quiz
            var quiz = await mediator.Send(new GetQuizDetailsQuery(request.quizId),cancellationToken);

            if (!quiz.Success || quiz.Data == null) 
                return RequestResponse<StartQuizDto>.Fail("Quiz not found.",StatusCodes.Status404NotFound);
            

            // 2. Check Quiz Status
            if (quiz.Data.Status != QuizStatus.Published) 
                return RequestResponse<StartQuizDto>.Fail("Quiz not published yet.",StatusCodes.Status409Conflict);
            

            // 3. Check Existing InProgress Attempt
            var existingAttempt = await mediator.Send(new CheckExistingAttemptQuery(request.StudentId,request.quizId),cancellationToken);

            if (!existingAttempt.Success) 
                return RequestResponse<StartQuizDto>.Fail(existingAttempt.Message,existingAttempt.StatusCode);
            

            Guid attemptId;

            // 4. Resume Existing Attempt
            if (existingAttempt.Data.HasValue) 
                attemptId = existingAttempt.Data.Value;
             else {
                // 5. Check Max Attempts
                var maxAttempt = await mediator.Send(new CheckMaxAttemptsQuery(request.StudentId,request.quizId,quiz.Data.MaxAttempts),cancellationToken);

                if (!maxAttempt.Data) 
                    return RequestResponse<StartQuizDto>.Fail(maxAttempt.Message,maxAttempt.StatusCode);
                

                // 6. Create New Attempt
                var newAttempt = await mediator.Send(new StartQuizAttemptCommand(request.quizId,request.StudentId,quiz.Data.DurationMinutes.Value),cancellationToken);

                if (!newAttempt.Success) 
                    return RequestResponse<StartQuizDto>.Fail(newAttempt.Message,newAttempt.StatusCode);
                

                attemptId = newAttempt.Data;
            }

            // 7. Get Attempt Details
            var attempt = await mediator.Send(
                new GetOldQuizAttemptQuery(attemptId),
                cancellationToken);

            if (!attempt.Success || attempt.Data == null) {
                return RequestResponse<StartQuizDto>.Fail(
                    attempt.Message,
                    attempt.StatusCode);
            }

            // 8. Get Questions
            var questions = await mediator.Send(
                new GetQuizQuestionsQuery(request.quizId),
                cancellationToken);

            if (!questions.Success || questions.Data == null) {
                return RequestResponse<StartQuizDto>.Fail(
                    questions.Message,
                    questions.StatusCode);
            }

            // 9. Make sure Quiz has Questions
            if (questions.Data.Count == 0) {
                return RequestResponse<StartQuizDto>.Fail(
                    "Quiz has no questions.",
                    StatusCodes.Status409Conflict);
            }

            // 10. Shuffle Questions
            var shuffledQuestions = questions.Data
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            // 11. Shuffle Options independently for each Question
            foreach (var question in shuffledQuestions) {
                question.Options = question.Options
                    .OrderBy(_ => Guid.NewGuid())
                    .ToList();
            }

            // 12. Build Response
            var response = new StartQuizDto {
                AttemptId = attemptId,
                StartTime = attempt.Data.StartTime,
                Deadline = attempt.Data.Deadline,
                Questions = shuffledQuestions
            };

            // 13. Return
            return RequestResponse<StartQuizDto>.Ok(
                response,
                "Quiz started successfully.");
        }
    }
}