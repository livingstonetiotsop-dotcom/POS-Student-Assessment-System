using POS.Application.Ports;
using POS.Domain.Entities;
using POS.Infrastructure.Records;
using POS.Infrastructure.Storage;

namespace POS.Infrastructure.Repositories;

public class JsonRubricRepository : IRubricRepository
{
    private const string FileName = "rubric.json";

    private readonly JsonDataStore _store;

    public JsonRubricRepository(JsonDataStore store)
    {
        _store = store;
    }

    public void Save(Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);

        var record = RubricRecord.From(rubric);
        _store.Save(FileName, new List<RubricRecord> { record });
    }

    public Rubric? Get()
    {
        var records = _store.Load<RubricRecord>(FileName);

        return records.FirstOrDefault()?.ToDomain();
    }
}
