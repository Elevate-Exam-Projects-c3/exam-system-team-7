using exam_system.Common.Enums;

namespace exam_system.Dtos.Attempts {
    public class AttemptTimeRemainingDto {
              public Guid AttemptId { get; set; }

              public DateTime ServerTimeUtc { get; set; }

               public DateTime Deadline { get; set; }

               public long RemainingSeconds { get; set; }

                public AttemptStatus Status { get; set; }
}
}
