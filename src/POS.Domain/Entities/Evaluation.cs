using POS.Domain.ValueObjects;

namespace POS.Domain.Entities;

public class Evaluation
{
    public Guid Id { get; }
    public Guid StudentId { get; }
    public Guid CriterionId { get; }
    public Score Score { get; private set; }
    public DateTime EvaluatedAt { get; }
    public string? Comment { get; private set; }

    internal Evaluation(
        Guid studentId,
        Guid criterionId,
        Score score,
        string? comment)
    {
        Id = Guid.NewGuid();
        StudentId = studentId;
        CriterionId = criterionId;
        Score = score;
        Comment = comment;
        EvaluatedAt = DateTime.UtcNow;
    }

    public static Evaluation Restore(
        Guid id,
        Guid studentId,
        Guid criterionId,
        decimal score,
        decimal maximum,
        DateTime evaluatedAt,
        string? comment)
    {
        return new Evaluation(
            id,
            studentId,
            criterionId,
            new Score(score, maximum),
            evaluatedAt,
            comment);
    }

    private Evaluation(
        Guid id,
        Guid studentId,
        Guid criterionId,
        Score score,
        DateTime evaluatedAt,
        string? comment)
    {
        Id = id;
        StudentId = studentId;
        CriterionId = criterionId;
        Score = score;
        EvaluatedAt = evaluatedAt;
        Comment = comment;
    }

    public void ReviseScore(
        decimal newValue,
        Criterion criterion,
        string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(criterion);

        if (criterion.Id != CriterionId)
            throw new InvalidOperationException(
                "The criterion does not match this evaluation.");

        Score = new Score(newValue, criterion.MaxScore);
        Comment = comment;
    }
}
