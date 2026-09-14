namespace exam_system.Dtos.Options {
    public class OptionDto {

         public Guid Id { get; set; }

         public string OptionText { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }


    }
}
