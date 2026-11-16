using POS.Domain.Entities;
using POS.Domain.ValueObjects;

namespace POS.Domain.Strategies;

public class WeightedAverageStrategy : IResultCalculationStrategy
{
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

        if (studentEvaluations.Count == 0)
            throw new InvalidOperationException(
                "The student has no evaluations.");

        var duplicateCriterion = studentEvaluations
            .GroupBy(e => e.CriterionId)
            .Any(group => group.Count() > 1);

        if (duplicateCriterion)
            throw new InvalidOperationException(
                "A student cannot have multiple current evaluations for the same criterion.");

        decimal weightedSum = 0;
        decimal totalWeight = 0;

        foreach (var evaluation in studentEvaluations)
        {
            var criterion = rubric.GetCriterion(evaluation.CriterionId);

            weightedSum +=
                (evaluation.Score.Value / criterion.MaxScore) * criterion.Weight;

            totalWeight += criterion.Weight;
        }

        var result = (weightedSum / totalWeight) * 10;

        return new FinalResult(result);
    }
}
