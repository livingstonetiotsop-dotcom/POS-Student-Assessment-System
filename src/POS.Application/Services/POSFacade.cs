using POS.Application.DTOs;
using POS.Application.Ports;
using POS.Domain.Entities;
using POS.Domain.Factories;
using POS.Domain.Services;
using POS.Domain.ValueObjects;

namespace POS.Application.Services;

public class POSFacade
{
    private readonly IStudentRepository _students;
    private readonly IRubricRepository _rubricRepository;
    private readonly IEvaluationRepository _evaluations;
    private readonly EvaluationFactory _evaluationFactory;
    private readonly FinalResultCalculator _resultCalculator;

    public POSFacade(
        IStudentRepository students,
        IRubricRepository rubricRepository,
        IEvaluationRepository evaluations,
        EvaluationFactory evaluationFactory,
        FinalResultCalculator resultCalculator)
    {
        _students = students;
        _rubricRepository = rubricRepository;
        _evaluations = evaluations;
        _evaluationFactory = evaluationFactory;
        _resultCalculator = resultCalculator;
    }

    public Student AddStudent(string name)
    {
        var student = new Student(name);
        _students.Add(student);
        return student;
    }

    public IReadOnlyCollection<Student> GetStudents()
        => _students.GetAll();

    public Rubric AddCriterion(string name, decimal maxScore, decimal weight)
    {
        var rubric = _rubricRepository.Get()
            ?? new Rubric("Course Rubric");

        var criterion = new Criterion(name, maxScore, weight);
        rubric.AddCriterion(criterion);
        _rubricRepository.Save(rubric);

        return rubric;
    }

    public IReadOnlyCollection<Criterion> GetCriteria()
    {
        var rubric = _rubricRepository.Get();

        if (rubric == null)
            return Array.Empty<Criterion>();

        return rubric.Criteria.ToList();
    }

    public Evaluation AddEvaluation(
        Guid studentId,
        Guid criterionId,
        decimal score,
        string? comment = null)
    {
        var student = _students.GetById(studentId)
            ?? throw new InvalidOperationException("Student not found.");

        var rubric = _rubricRepository.Get()
            ?? throw new InvalidOperationException("Rubric not found.");

        var evaluation = _evaluationFactory.Create(
            student, criterionId, score, rubric, comment);

        var existing = _evaluations.GetAll()
            .FirstOrDefault(e =>
                e.StudentId == studentId &&
                e.CriterionId == criterionId);

        if (existing != null)
            throw new InvalidOperationException(
                "An evaluation already exists for this student and criterion. Edit it instead.");

        _evaluations.Add(evaluation);
        return evaluation;
    }

    public IReadOnlyCollection<Evaluation> GetEvaluations()
        => _evaluations.GetAll();

    public void ReviseEvaluation(
        Guid evaluationId,
        decimal newScore,
        string? comment = null)
    {
        var evaluation = _evaluations.GetById(evaluationId)
            ?? throw new InvalidOperationException("Evaluation not found.");

        var rubric = _rubricRepository.Get()
            ?? throw new InvalidOperationException("Rubric not found.");

        var criterion = rubric.GetCriterion(evaluation.CriterionId);

        evaluation.ReviseScore(newScore, criterion, comment);
        _evaluations.Update(evaluation);
    }

    public void CancelEvaluation(Guid evaluationId)
    {
        if (_evaluations.GetById(evaluationId) == null)
            throw new InvalidOperationException("Evaluation not found.");

        _evaluations.Remove(evaluationId);
    }

    public FinalResult CalculateFinalResult(Guid studentId)
    {
        var student = _students.GetById(studentId)
            ?? throw new InvalidOperationException("Student not found.");

        var rubric = _rubricRepository.Get()
            ?? throw new InvalidOperationException("Rubric not found.");

        return _resultCalculator.Calculate(
            student, rubric, _evaluations.GetAll());
    }

    public IReadOnlyCollection<StudentResultDto> GetFinalResults()
    {
        var students = _students.GetAll();
        var evaluations = _evaluations.GetAll();
        var rubric = _rubricRepository.Get();

        return students.Select(student =>
        {
            decimal? score = null;

            if (rubric != null &&
                evaluations.Any(e => e.StudentId == student.Id))
            {
                score = _resultCalculator.Calculate(
                    student, rubric, evaluations).Value;
            }

            return new StudentResultDto
            {
                StudentId = student.Id,
                StudentName = student.Name,
                FinalScore = score
            };
        }).ToList().AsReadOnly();
    }
}
