using POS.Domain.Entities;
using POS.Domain.ValueObjects;

namespace POS.Domain.Strategies;

public interface IResultCalculationStrategy
{
    FinalResult Calculate(
        Guid studentId,
        Rubric rubric,
        IReadOnlyCollection<Evaluation> evaluations);
}
