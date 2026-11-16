using POS.Application.Ports;
using POS.Domain.Entities;
using POS.Infrastructure.Records;
using POS.Infrastructure.Storage;

namespace POS.Infrastructure.Repositories;

public class JsonEvaluationRepository : IEvaluationRepository
{
    private const string FileName = "evaluations.json";

    private readonly JsonDataStore _store;
    private readonly IRubricRepository _rubricRepository;
    private readonly List<EvaluationRecord> _records;

    public JsonEvaluationRepository(
        JsonDataStore store,
        IRubricRepository rubricRepository)
    {
        _store = store;
        _rubricRepository = rubricRepository;
        _records = _store.Load<EvaluationRecord>(FileName);
    }

    public void Add(Evaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);

        if (_records.Any(record => record.Id == evaluation.Id))
            throw new InvalidOperationException(
                "This evaluation already exists.");

        var criterion = GetCriterion(evaluation.CriterionId);

        _records.Add(
            EvaluationRecord.From(evaluation, criterion.MaxScore));

        Save();
    }

    public void Update(Evaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);

        var index = _records.FindIndex(
            record => record.Id == evaluation.Id);

        if (index < 0)
            throw new InvalidOperationException(
                "Evaluation not found.");

        var criterion = GetCriterion(evaluation.CriterionId);

        _records[index] =
            EvaluationRecord.From(evaluation, criterion.MaxScore);

        Save();
    }

    public void Remove(Guid evaluationId)
    {
        var record = _records.FirstOrDefault(
            item => item.Id == evaluationId);

        if (record == null)
            throw new InvalidOperationException(
                "Evaluation not found.");

        _records.Remove(record);
        Save();
    }

    public Evaluation? GetById(Guid evaluationId)
    {
        var record = _records.FirstOrDefault(
            item => item.Id == evaluationId);

        return record == null ? null : ToDomain(record);
    }

    public IReadOnlyCollection<Evaluation> GetAll() =>
        _records.Select(ToDomain).ToList().AsReadOnly();

    private Criterion GetCriterion(Guid criterionId)
    {
        var rubric = _rubricRepository.Get()
            ?? throw new InvalidOperationException(
                "Cannot access evaluations because the rubric does not exist.");

        return rubric.GetCriterion(criterionId);
    }

    private Evaluation ToDomain(EvaluationRecord record)
    {
        var criterion = GetCriterion(record.CriterionId);

        return Evaluation.Restore(
            record.Id,
            record.StudentId,
            record.CriterionId,
            record.Score,
            criterion.MaxScore,
            record.EvaluatedAt,
            record.Comment);
    }

    private void Save()
    {
        _store.Save(FileName, _records);
    }
}
