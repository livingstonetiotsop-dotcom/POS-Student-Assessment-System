using POS.Domain.Entities;

namespace POS.Application.Ports;

public interface IStudentRepository
{
    void Add(Student student);
    Student? GetById(Guid id);
    IReadOnlyCollection<Student> GetAll();
}
