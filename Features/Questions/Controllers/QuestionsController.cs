using exam_system.Features.Questions.AdminCreateQuestion.Orchestrators;
using exam_system.Features.Shared;
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
    }
}
