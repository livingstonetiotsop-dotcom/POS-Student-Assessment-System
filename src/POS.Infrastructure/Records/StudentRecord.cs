using POS.Domain.Entities;

namespace POS.Infrastructure.Records;

public class StudentRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public static StudentRecord From(Student student) => new()
    {
        Id = student.Id,
        Name = student.Name
    };

    public Student ToDomain() => Student.Restore(Id, Name);
}
