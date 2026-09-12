using exam_system.Dtos.Diploma.BrowseDiplomas;
using exam_system.Dtos.Diploma.CreateDiploma;
using exam_system.Dtos.Diploma.UpdateDiploma;
using exam_system.ViewModels.Diplomas.BrowseDiplomas;

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

        public static BrowseDiplomasDto ToDto(
            this BrowseDiplomasViewModel viewModel)
        {
            return new BrowseDiplomasDto
            {
                PageIndex = viewModel.PageIndex,
                PageSize = viewModel.PageSize
            };
        }


        public static DiplomaListItemViewModel ToViewModel(this BrowseDiplomaItemDto dto)
        {
            return new DiplomaListItemViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                TotalQuizzes = dto.TotalQuizzes,
                CompletedQuizzes = dto.CompletedQuizzes
            };
        }
    }
}
