using POS.Domain.Entities;

namespace POS.Application.Ports;

public interface IEvaluationRepository
{
    void Add(Evaluation evaluation);
    void Update(Evaluation evaluation);
    void Remove(Guid evaluationId);
    Evaluation? GetById(Guid evaluationId);
    IReadOnlyCollection<Evaluation> GetAll();
}
