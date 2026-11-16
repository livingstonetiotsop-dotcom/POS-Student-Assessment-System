using POS.Domain.Entities;

namespace POS.Application.Ports;

public interface IRubricRepository
{
    void Save(Rubric rubric);
    Rubric? Get();
}
