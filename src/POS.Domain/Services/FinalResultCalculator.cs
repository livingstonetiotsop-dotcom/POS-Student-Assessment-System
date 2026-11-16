using POS.Domain.Entities;
using POS.Domain.Strategies;
using POS.Domain.ValueObjects;

namespace POS.Domain.Services;

public class FinalResultCalculator
{
    private readonly IResultCalculationStrategy _strategy;

    public FinalResultCalculator(IResultCalculationStrategy strategy)
    {
        _strategy = strategy ??
            throw new ArgumentNullException(nameof(strategy));
    }

    public FinalResult Calculate(
        Student student,
        Rubric rubric,
        IReadOnlyCollection<Evaluation> evaluations)
    {
        ArgumentNullException.ThrowIfNull(student);

        return _strategy.Calculate(student.Id, rubric, evaluations);
    }
}
