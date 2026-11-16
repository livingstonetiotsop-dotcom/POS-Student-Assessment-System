using POS.Domain.Entities;

namespace POS.Infrastructure.Records;

public class EvaluationRecord
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CriterionId { get; set; }
    public decimal Score { get; set; }
    public decimal MaximumScore { get; set; }
    public DateTime EvaluatedAt { get; set; }
    public string? Comment { get; set; }

    public static EvaluationRecord From(
        Evaluation evaluation,
        decimal maximumScore) => new()
    {
        Id = evaluation.Id,
        StudentId = evaluation.StudentId,
        CriterionId = evaluation.CriterionId,
        Score = evaluation.Score.Value,
        MaximumScore = maximumScore,
        EvaluatedAt = evaluation.EvaluatedAt,
        Comment = evaluation.Comment
    };

    public Evaluation ToDomain() =>
        Evaluation.Restore(
            Id,
            StudentId,
            CriterionId,
            Score,
            MaximumScore,
            EvaluatedAt,
            Comment);
}
