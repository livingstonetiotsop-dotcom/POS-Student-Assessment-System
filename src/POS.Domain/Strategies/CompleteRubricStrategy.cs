using POS.Domain.Entities;
using POS.Domain.ValueObjects;

namespace POS.Domain.Strategies;

public class CompleteRubricStrategy : IResultCalculationStrategy
{
    private readonly WeightedAverageStrategy _weightedAverage = new();

    public FinalResult Calculate(
        Guid studentId,
        Rubric rubric,
        IReadOnlyCollection<Evaluation> evaluations)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        ArgumentNullException.ThrowIfNull(evaluations);

        var studentEvaluations = evaluations
            .Where(e => e.StudentId == studentId)
            .ToList();

        if (rubric.Criteria.Count == 0)
            throw new InvalidOperationException(
                "The rubric has no criteria.");

        var evaluatedCriterionIds = studentEvaluations
            .Select(e => e.CriterionId)
            .ToHashSet();

        if (rubric.Criteria.Any(c => !evaluatedCriterionIds.Contains(c.Id)))
            throw new InvalidOperationException(
                "All rubric criteria must be evaluated before calculating the final result.");

        return _weightedAverage.Calculate(studentId, rubric, evaluations);
    }
}
