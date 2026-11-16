namespace POS.Domain.Entities;

public class Rubric
{
    private readonly List<Criterion> _criteria = new();

    public Guid Id { get; }
    public string Name { get; }
    public IReadOnlyCollection<Criterion> Criteria => _criteria.AsReadOnly();

    public Rubric(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Rubric name cannot be empty.", nameof(name));

        Id = Guid.NewGuid();
        Name = name.Trim();
    }

    private Rubric(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Rubric Restore(
        Guid id,
        string name,
        IEnumerable<Criterion> criteria)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "Rubric ID cannot be empty.", nameof(id));

        var rubric = new Rubric(id, name);

        foreach (var criterion in criteria)
            rubric.AddCriterion(criterion);

        return rubric;
    }

    public void AddCriterion(Criterion criterion)
    {
        ArgumentNullException.ThrowIfNull(criterion);

        if (_criteria.Any(c => c.Id == criterion.Id))
            throw new InvalidOperationException(
                "This criterion is already in the rubric.");

        if (_criteria.Any(c =>
            string.Equals(c.Name, criterion.Name,
                StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException(
                "A criterion with this name already exists.");

        _criteria.Add(criterion);
    }

    public Criterion GetCriterion(Guid criterionId)
    {
        return _criteria.FirstOrDefault(c => c.Id == criterionId)
            ?? throw new InvalidOperationException(
                "The criterion does not belong to this rubric.");
    }
}
