namespace exam_system.Dtos.Quizes
{
    public class QuizPublishCheckDto
    {
        public bool readyToPublish => Checks.All(c => c.IsPassed);
        public List<PublishCheckItem> Checks { get; set; } = new ();

    }
}
