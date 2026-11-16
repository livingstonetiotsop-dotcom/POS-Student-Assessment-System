namespace POS.Domain.Entities;

public class Student
{
    public Guid Id { get; }
    public string Name { get; }

    public Student(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Student name cannot be empty.", nameof(name));

        Id = Guid.NewGuid();
        Name = name.Trim();
    }

    public static Student Restore(Guid id, string name)
    {
        var student = new Student(name);
        return new Student(id, student.Name);
    }

    private Student(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
