using exam_system.Features.Questions.AdminCreateQuestion.Orchestrators;
using exam_system.Features.Questions.AdminGetQuestion.Queries;
using exam_system.Features.Shared;
using exam_system.ViewModels.Options;
using exam_system.ViewModels.Questions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Questions.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase {

        private readonly IMediator mediator;

        public QuestionsController(IMediator mediator) {
            this.mediator = mediator;
        }

        [HttpPost("{quizId:guid}/questions")]
        public async Task<ActionResult<EndpointResponse<bool>>> CreateQuestion(Guid quizId, [FromBody] CreateQuestionViewModel createQuestion, CancellationToken cancellationToken) {
            try {
                var result = await mediator.Send(
                    new CreateQuestionsAndOptionsOrchestrator(
                        quizId,
                        createQuestion.Text,
                        createQuestion.Explanation,
                        createQuestion.OrderIndex,
                        createQuestion.Options

                    ), cancellationToken
                );
                return StatusCode(result.StatusCode,EndpointResponse<bool>.FromResult(result));

            } catch (FluentValidation.ValidationException ex) {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(new EndpointResponse<Guid>(
                    success: false,
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "Validation failed.",
                    data: default,
                    errors: errors
                ));
            }
        }




        [HttpGet("{quizId:guid}")]
        public async Task<ActionResult<EndpointResponse<List<QuestionViewModel>>>> GetQuestions(Guid quizId , CancellationToken cancellationToken) {

            var questions = await mediator.Send(new GetQuizQuestionsQuery(quizId), cancellationToken);

            if (!questions.Success) {
                return StatusCode(questions.StatusCode,
                    EndpointResponse<List<QuestionViewModel>>.FromResult(
                        RequestResponse<List<QuestionViewModel>>.Fail(
                            questions.Message,
                            questions.StatusCode,
                            questions.Errors)));
            }

            var result = questions.Data!
                 .Select(q => new QuestionViewModel {
                     Id = q.Id,
                     Text = q.Text,
                     Options = q.Options
                     .Select(o => new QuizOptionViewModel {
                         Id = o.Id,
                         OptionText = o.OptionText
                     }).ToList()
                 }).ToList();


            

            return StatusCode(questions.StatusCode,
                EndpointResponse<List<QuestionViewModel>>.FromResult(RequestResponse<List<QuestionViewModel>>
                .Ok(result, questions.Message, questions.StatusCode))
            );

        }

    }
}
