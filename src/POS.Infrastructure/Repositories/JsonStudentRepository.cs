using POS.Application.Ports;
using POS.Domain.Entities;
using POS.Infrastructure.Records;
using POS.Infrastructure.Storage;

namespace POS.Infrastructure.Repositories;

public class JsonStudentRepository : IStudentRepository
{
    private const string FileName = "students.json";

    private readonly JsonDataStore _store;
    private readonly List<Student> _students;

    public JsonStudentRepository(JsonDataStore store)
    {
        _store = store;
        _students = _store.Load<StudentRecord>(FileName)
            .Select(record => record.ToDomain())
            .ToList();
    }

    public void Add(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (_students.Any(s => s.Id == student.Id))
            throw new InvalidOperationException(
                "This student already exists.");

        _students.Add(student);
        Save();
    }

    public Student? GetById(Guid id) =>
        _students.FirstOrDefault(s => s.Id == id);

    public IReadOnlyCollection<Student> GetAll() =>
        _students.AsReadOnly();

    private void Save()
    {
        var records = _students
            .Select(StudentRecord.From)
            .ToList();

        _store.Save(FileName, records);
    }
}
