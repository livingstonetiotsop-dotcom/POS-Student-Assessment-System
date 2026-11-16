using POS.Domain.Entities;

namespace POS.Infrastructure.Records;

public class RubricRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public List<CriterionRecord> Criteria { get; set; } = new();

    public static RubricRecord From(Rubric rubric) => new()
    {
        Id = rubric.Id,
        Name = rubric.Name,
        Criteria = rubric.Criteria
            .Select(CriterionRecord.From)
            .ToList()
    };

    public Rubric ToDomain() =>
        Rubric.Restore(
            Id,
            Name,
            Criteria.Select(record => record.ToDomain()));
}
