using POS.Application.DTOs;

namespace POS.Application.Ports;

public interface IResultExporter
{
    string Export(IReadOnlyCollection<StudentResultDto> results);
}
