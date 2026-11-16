using POS.Domain.Entities;
using POS.Domain.ValueObjects;

namespace POS.Domain.Factories;

public class EvaluationFactory
{
    public Evaluation Create(
        Student student,
        Guid criterionId,
        decimal scoreValue,
        Rubric rubric,
        string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(rubric);

        // The criterion must exist in the rubric.
        Criterion criterion = rubric.GetCriterion(criterionId);

        // Score validates the permitted range.
        var score = new Score(scoreValue, criterion.MaxScore);

        return new Evaluation(
            student.Id,
            criterion.Id,
            score,
            comment);
    }
}
