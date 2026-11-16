using POS.Domain.Entities;
using POS.Domain.Factories;
using POS.Domain.Strategies;
using POS.Domain.ValueObjects;

namespace POS.Domain.Tests;

public class DomainRulesTests
{
    [Fact]
    public void Score_ShouldAcceptValueWithinRange()
    {
        var score = new Score(8, 10);

        Assert.Equal(8, score.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void Score_ShouldRejectValueOutsideRange(decimal value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Score(value, 10));
    }

    [Fact]
    public void Criterion_ShouldRejectNonPositiveMaximumScore()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Criterion("Homework", 0, 2));
    }

    [Fact]
    public void Criterion_ShouldRejectNonPositiveWeight()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Criterion("Homework", 10, 0));
    }

    [Fact]
    public void EvaluationFactory_ShouldCreateValidEvaluation()
    {
        var student = new Student("Alice");
        var criterion = new Criterion("Homework", 10, 2);
        var rubric = new Rubric("Course");
        rubric.AddCriterion(criterion);

        var factory = new EvaluationFactory();

        var evaluation = factory.Create(
            student,
            criterion.Id,
            8,
            rubric);

        Assert.Equal(student.Id, evaluation.StudentId);
        Assert.Equal(criterion.Id, evaluation.CriterionId);
        Assert.Equal(8, evaluation.Score.Value);
    }

    [Fact]
    public void EvaluationFactory_ShouldRejectCriterionOutsideRubric()
    {
        var student = new Student("Alice");
        var criterion = new Criterion("Homework", 10, 2);
        var rubric = new Rubric("Course");

        var factory = new EvaluationFactory();

        Assert.Throws<InvalidOperationException>(
            () => factory.Create(
                student,
                criterion.Id,
                8,
                rubric));
    }

    [Fact]
    public void EvaluationFactory_ShouldRejectScoreAboveMaximum()
    {
        var student = new Student("Alice");
        var criterion = new Criterion("Homework", 10, 2);
        var rubric = new Rubric("Course");
        rubric.AddCriterion(criterion);

        var factory = new EvaluationFactory();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => factory.Create(
                student,
                criterion.Id,
                11,
                rubric));
    }

    [Fact]
    public void WeightedAverageStrategy_ShouldCalculateCorrectResult()
    {
        var student = new Student("Alice");
        var rubric = new Rubric("Course");

        var homework = new Criterion("Homework", 10, 2);
        var midterm = new Criterion("Midterm", 100, 3);
        var finalExam = new Criterion("Final Exam", 100, 5);

        rubric.AddCriterion(homework);
        rubric.AddCriterion(midterm);
        rubric.AddCriterion(finalExam);

        var factory = new EvaluationFactory();

        var evaluations = new List<Evaluation>
        {
            factory.Create(student, homework.Id, 8, rubric),
            factory.Create(student, midterm.Id, 80, rubric),
            factory.Create(student, finalExam.Id, 90, rubric)
        };

        var strategy = new WeightedAverageStrategy();

        var result = strategy.Calculate(
            student.Id, rubric, evaluations);

        Assert.Equal(8.5m, result.Value);
    }

    [Fact]
    public void WeightedAverageStrategy_ShouldRejectDuplicateEvaluations()
    {
        var student = new Student("Alice");
        var rubric = new Rubric("Course");
        var homework = new Criterion("Homework", 10, 2);

        rubric.AddCriterion(homework);

        var factory = new EvaluationFactory();

        var evaluations = new List<Evaluation>
        {
            factory.Create(student, homework.Id, 8, rubric),
            factory.Create(student, homework.Id, 9, rubric)
        };

        var strategy = new WeightedAverageStrategy();

        Assert.Throws<InvalidOperationException>(
            () => strategy.Calculate(
                student.Id, rubric, evaluations));
    }

    [Fact]
    public void CompleteRubricStrategy_ShouldRejectIncompleteEvaluations()
    {
        var student = new Student("Alice");
        var rubric = new Rubric("Course");

        var homework = new Criterion("Homework", 10, 2);
        var midterm = new Criterion("Midterm", 100, 3);

        rubric.AddCriterion(homework);
        rubric.AddCriterion(midterm);

        var factory = new EvaluationFactory();

        var evaluations = new List<Evaluation>
        {
            factory.Create(student, homework.Id, 8, rubric)
        };

        var strategy = new CompleteRubricStrategy();

        Assert.Throws<InvalidOperationException>(
            () => strategy.Calculate(
                student.Id, rubric, evaluations));
    }

    [Fact]
    public void CompleteRubricStrategy_ShouldCalculateWhenAllCriteriaAreEvaluated()
    {
        var student = new Student("Alice");
        var rubric = new Rubric("Course");

        var homework = new Criterion("Homework", 10, 2);
        var midterm = new Criterion("Midterm", 100, 3);

        rubric.AddCriterion(homework);
        rubric.AddCriterion(midterm);

        var factory = new EvaluationFactory();

        var evaluations = new List<Evaluation>
        {
            factory.Create(student, homework.Id, 8, rubric),
            factory.Create(student, midterm.Id, 80, rubric)
        };

        var strategy = new CompleteRubricStrategy();

        var result = strategy.Calculate(
            student.Id, rubric, evaluations);

        Assert.Equal(8m, result.Value);
    }
}
