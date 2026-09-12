using exam_system.Dtos.Diploma.CreateDiploma;
using exam_system.Dtos.Diploma.UpdateDiploma;

namespace exam_system.ViewModels.Diplomas.Mappers
{
    public static class DiplomaViewModelMapper
    {
        public static CreateDiplomaDto ToDto(this CreateDiplomaVM viewModel)
        {
            return new CreateDiplomaDto
            {
                Title = viewModel.Title,
                Description = viewModel.Description
            };
        }

        public static UpdateDiplomaDto ToDto( this UpdateDiplomaVM viewModel)
        {
            return new UpdateDiplomaDto
            {
                Title = viewModel.Title,
                Description = viewModel.Description
            };
        }

    }
}
