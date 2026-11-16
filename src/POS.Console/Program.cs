using Microsoft.Extensions.DependencyInjection;
using POS.Application.Ports;
using POS.Application.Services;
using POS.Console.Configuration;

var services = DependencyInjection.ConfigureServices();
var pos = services.GetRequiredService<POSFacade>();
var exporter = services.GetRequiredService<IResultExporter>();

while (true)
{
    System.Console.WriteLine();
    System.Console.WriteLine("==================================");
    System.Console.WriteLine("       POS - Student Assessment");
    System.Console.WriteLine("==================================");
    System.Console.WriteLine("1. Add student");
    System.Console.WriteLine("2. List students");
    System.Console.WriteLine("3. Add assessment criterion");
    System.Console.WriteLine("4. List criteria");
    System.Console.WriteLine("5. Add evaluation");
    System.Console.WriteLine("6. List evaluations");
    System.Console.WriteLine("7. Edit evaluation");
    System.Console.WriteLine("8. Cancel evaluation");
    System.Console.WriteLine("9. Calculate final result");
    System.Console.WriteLine("10. Export final results to CSV");
    System.Console.WriteLine("0. Exit");
    System.Console.Write("Choose an option: ");

    var choice = System.Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                AddStudent();
                break;
            case "2":
                ListStudents();
                break;
            case "3":
                AddCriterion();
                break;
            case "4":
                ListCriteria();
                break;
            case "5":
                AddEvaluation();
                break;
            case "6":
                ListEvaluations();
                break;
            case "7":
                EditEvaluation();
                break;
            case "8":
                CancelEvaluation();
                break;
            case "9":
                CalculateResult();
                break;
            case "10":
                ExportResults();
                break;
            case "0":
                System.Console.WriteLine("Goodbye!");
                return;
            default:
                System.Console.WriteLine("Invalid option. Try again.");
                break;
        }
    }
    catch (Exception ex)
    {
        System.Console.WriteLine($"Error: {ex.Message}");
    }
}

void AddStudent()
{
    System.Console.Write("Student name: ");
    var name = System.Console.ReadLine() ?? "";

    var student = pos.AddStudent(name);

    System.Console.WriteLine(
        $"Student added: {student.Name} (ID: {student.Id})");
}

void ListStudents()
{
    var students = pos.GetStudents().ToList();

    if (students.Count == 0)
    {
        System.Console.WriteLine("No students found.");
        return;
    }

    System.Console.WriteLine("\nStudents:");

    for (int i = 0; i < students.Count; i++)
    {
        System.Console.WriteLine(
            $"{i + 1}. {students[i].Name} | ID: {students[i].Id}");
    }
}

void AddCriterion()
{
    System.Console.Write("Criterion name: ");
    var name = System.Console.ReadLine() ?? "";

    var maxScore = ReadDecimal("Maximum score: ");
    var weight = ReadDecimal("Weight: ");

    pos.AddCriterion(name, maxScore, weight);

    System.Console.WriteLine("Criterion added successfully.");
}

void ListCriteria()
{
    var criteria = pos.GetCriteria().ToList();

    if (criteria.Count == 0)
    {
        System.Console.WriteLine("No criteria found.");
        return;
    }

    System.Console.WriteLine("\nAssessment criteria:");

    for (int i = 0; i < criteria.Count; i++)
    {
        var criterion = criteria[i];

        System.Console.WriteLine(
            $"{i + 1}. {criterion.Name} | Maximum: {criterion.MaxScore} | Weight: {criterion.Weight}");
    }
}

void AddEvaluation()
{
    var students = pos.GetStudents().ToList();
    var criteria = pos.GetCriteria().ToList();

    if (students.Count == 0)
    {
        System.Console.WriteLine("Add a student first.");
        return;
    }

    if (criteria.Count == 0)
    {
        System.Console.WriteLine("Add an assessment criterion first.");
        return;
    }

    System.Console.WriteLine("\nSelect a student:");

    for (int i = 0; i < students.Count; i++)
        System.Console.WriteLine($"{i + 1}. {students[i].Name}");

    int studentIndex = ReadIndex("Student number: ", students.Count);

    System.Console.WriteLine("\nSelect a criterion:");

    for (int i = 0; i < criteria.Count; i++)
        System.Console.WriteLine(
            $"{i + 1}. {criteria[i].Name} (maximum: {criteria[i].MaxScore})");

    int criterionIndex = ReadIndex("Criterion number: ", criteria.Count);

    var selectedCriterion = criteria[criterionIndex];

    var score = ReadDecimal(
        $"Score (0 - {selectedCriterion.MaxScore}): ");

    System.Console.Write("Comment (optional): ");
    var comment = System.Console.ReadLine();

    var evaluation = pos.AddEvaluation(
        students[studentIndex].Id,
        selectedCriterion.Id,
        score,
        comment);

    System.Console.WriteLine(
        $"Evaluation added. ID: {evaluation.Id}");
}

void ListEvaluations()
{
    var evaluations = pos.GetEvaluations().ToList();
    var students = pos.GetStudents().ToList();
    var criteria = pos.GetCriteria().ToList();

    if (evaluations.Count == 0)
    {
        System.Console.WriteLine("No evaluations found.");
        return;
    }

    System.Console.WriteLine("\nEvaluations:");

    for (int i = 0; i < evaluations.Count; i++)
    {
        var evaluation = evaluations[i];

        var student = students.FirstOrDefault(
            s => s.Id == evaluation.StudentId);

        var criterion = criteria.FirstOrDefault(
            c => c.Id == evaluation.CriterionId);

        System.Console.WriteLine(
            $"{i + 1}. Student: {student?.Name ?? "Unknown"} | " +
            $"Criterion: {criterion?.Name ?? "Unknown"} | " +
            $"Score: {evaluation.Score.Value} | " +
            $"ID: {evaluation.Id}");
    }
}

void EditEvaluation()
{
    var evaluations = pos.GetEvaluations().ToList();

    if (evaluations.Count == 0)
    {
        System.Console.WriteLine("No evaluations to edit.");
        return;
    }

    ListEvaluations();

    int index = ReadIndex(
        "Evaluation number to edit: ", evaluations.Count);

    var evaluation = evaluations[index];

    var criterion = pos.GetCriteria()
        .First(c => c.Id == evaluation.CriterionId);

    var newScore = ReadDecimal(
        $"New score (0 - {criterion.MaxScore}): ");

    System.Console.Write("New comment (optional): ");
    var comment = System.Console.ReadLine();

    pos.ReviseEvaluation(evaluation.Id, newScore, comment);

    System.Console.WriteLine("Evaluation updated successfully.");
}

void CancelEvaluation()
{
    var evaluations = pos.GetEvaluations().ToList();

    if (evaluations.Count == 0)
    {
        System.Console.WriteLine("No evaluations to cancel.");
        return;
    }

    ListEvaluations();

    int index = ReadIndex(
        "Evaluation number to cancel: ", evaluations.Count);

    pos.CancelEvaluation(evaluations[index].Id);

    System.Console.WriteLine("Evaluation cancelled successfully.");
}

void CalculateResult()
{
    var students = pos.GetStudents().ToList();

    if (students.Count == 0)
    {
        System.Console.WriteLine("No students found.");
        return;
    }

    System.Console.WriteLine("\nSelect a student:");

    for (int i = 0; i < students.Count; i++)
        System.Console.WriteLine($"{i + 1}. {students[i].Name}");

    int index = ReadIndex("Student number: ", students.Count);
    var student = students[index];

    var result = pos.CalculateFinalResult(student.Id);

    System.Console.WriteLine(
        $"Final result for {student.Name}: {result.Value:F2}/10");
}

void ExportResults()
{
    var results = pos.GetFinalResults();
    var path = exporter.Export(results);

    System.Console.WriteLine(
        $"CSV report exported successfully: {path}");
}

int ReadIndex(string prompt, int count)
{
    while (true)
    {
        System.Console.Write(prompt);

        if (int.TryParse(System.Console.ReadLine(), out int number) &&
            number >= 1 && number <= count)
        {
            return number - 1;
        }

        System.Console.WriteLine(
            $"Enter a number between 1 and {count}.");
    }
}

decimal ReadDecimal(string prompt)
{
    while (true)
    {
        System.Console.Write(prompt);
        var input = System.Console.ReadLine();

        if (decimal.TryParse(
            input,
            System.Globalization.NumberStyles.Number,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal value))
        {
            return value;
        }

        System.Console.WriteLine(
            "Enter a valid number. Use a dot for decimals, for example 7.5.");
    }
}
