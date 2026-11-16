namespace POS.Domain.Entities;

public class Criterion
{
    public Guid Id { get; }
    public string Name { get; }
    public decimal MaxScore { get; }
    public decimal Weight { get; }

    public Criterion(string name, decimal maxScore, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Criterion name cannot be empty.", nameof(name));

        if (maxScore <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(maxScore), "Maximum score must be greater than zero.");

        if (weight <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(weight), "Criterion weight must be greater than zero.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        MaxScore = maxScore;
        Weight = weight;
    }

    public static Criterion Restore(
        Guid id, string name, decimal maxScore, decimal weight)
    {
        var criterion = new Criterion(name, maxScore, weight);

        return new Criterion(
            id, criterion.Name, criterion.MaxScore, criterion.Weight);
    }

    private Criterion(
        Guid id, string name, decimal maxScore, decimal weight)
    {
        Id = id;
        Name = name;
        MaxScore = maxScore;
        Weight = weight;
    }
}
