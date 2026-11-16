namespace POS.Application.DTOs;

public class StudentResultDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public decimal? FinalScore { get; set; }
}
