using POS.Domain.Entities;

namespace POS.Infrastructure.Records;

public class CriterionRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public decimal MaxScore { get; set; }
    public decimal Weight { get; set; }

    public static CriterionRecord From(Criterion criterion) => new()
    {
        Id = criterion.Id,
        Name = criterion.Name,
        MaxScore = criterion.MaxScore,
        Weight = criterion.Weight
    };

    public Criterion ToDomain() =>
        Criterion.Restore(Id, Name, MaxScore, Weight);
}
