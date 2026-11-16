# POS — Student Assessment System

## 1. Project Overview

POS (Преподавательская Оценка Студентов) is a console application for managing students, assessment criteria, student evaluations and weighted final results.

The application supports:
- Adding and listing students.
- Creating and listing assessment criteria.
- Adding evaluations with optional comments.
- Editing existing evaluations.
- Cancelling evaluations.
- Calculating weighted final results.
- Persisting application data in JSON files.
- Exporting final results to CSV.

## 2. Technologies

- C#
- .NET 8
- xUnit
- System.Text.Json
- Microsoft.Extensions.DependencyInjection

## 3. Architecture

The project follows the Ports & Adapters (Hexagonal Architecture) approach.

### Domain Layer

Location: `src/POS.Domain`

Contains the core business model and rules:
- Entities: Student, Criterion, Rubric, Evaluation.
- Value Objects: Score, FinalResult.
- Factory: EvaluationFactory.
- Domain Service: FinalResultCalculator.
- Calculation strategies: WeightedAverageStrategy and CompleteRubricStrategy.

The Domain layer does not depend on the Console or Infrastructure layers.

### Application Layer

Location: `src/POS.Application`

Contains application-level operations and abstractions:
- POSFacade coordinates the main application use cases.
- Repository interfaces define the persistence ports.
- IResultExporter defines the CSV export port.
- StudentResultDto transfers final-result data for reporting.

The Application layer depends on Domain abstractions and models, not on concrete JSON storage.

### Infrastructure Layer

Location: `src/POS.Infrastructure`

Implements the outer adapters:
- JsonStudentRepository
- JsonRubricRepository
- JsonEvaluationRepository
- JsonDataStore
- CsvResultExporter

JSON data is stored in the user's home directory under `POSData`.
CSV reports are saved under `POSData/exports`.

### Console Layer

Location: `src/POS.Console`

Provides the command-line interface and the composition root.

DependencyInjection registers concrete implementations and connects them to application interfaces.

Dependencies point inward: Infrastructure and Console depend on inner layers, while Domain remains independent of the outer layers.

## 4. Domain Model and Ubiquitous Language

- **Student** — a person whose academic performance is assessed.
- **Criterion** — an assessment component with a maximum score and weight.
- **Rubric** — the collection of assessment criteria for a course.
- **Evaluation** — a student's score for one criterion.
- **Score** — a value object that validates a score against its maximum.
- **FinalResult** — a value object representing a final result on a 0–10 scale.

### Business Invariants

1. Student names cannot be empty.
2. Criterion names cannot be empty.
3. A criterion must have a positive maximum score and weight.
4. A score must be between zero and the criterion's maximum.
5. An evaluation must refer to a criterion belonging to the rubric.
6. A student cannot have multiple current evaluations for the same criterion.
7. Final results must be between zero and ten.

The Domain model enforces these rules rather than relying only on console input validation.

## 5. Final Result Calculation

The default strategy is WeightedAverageStrategy.

Each evaluation is normalized against its criterion's maximum score. The normalized score is multiplied by the criterion's weight.

Formula:

Final Result =
10 × sum((score / maximum score) × weight)
    / sum(weights of evaluated criteria)

The current weighted-average strategy calculates using evaluated criteria only. It does not require every criterion in the rubric to have an evaluation.

CompleteRubricStrategy is an alternative strategy that requires all rubric criteria to be evaluated before calculating the result.

The default strategy is selected through Dependency Injection.

## 6. Design Patterns

The project uses three GoF design patterns.

### Creational — Factory

EvaluationFactory creates Evaluation objects.

It checks that the student and rubric exist and that the criterion belongs to the rubric. It also creates a validated Score.

### Structural — Facade

POSFacade provides a unified interface for application operations.

The Console does not coordinate repositories and Domain services directly. It calls the facade.

### Behavioral — Strategy

IResultCalculationStrategy defines the final-result calculation contract.

WeightedAverageStrategy and CompleteRubricStrategy implement different calculation policies. This allows the calculation algorithm to vary without changing the Domain Service.

## 7. Dependency Injection

Microsoft.Extensions.DependencyInjection is used in the Console composition root.

Repository interfaces are mapped to JSON implementations. The calculation strategy, factory, Domain Service and POSFacade are also registered in the container.

Constructor injection supplies dependencies to the classes that need them.

## 8. Persistence and Export

The application stores data in JSON files under:

`~/POSData/`

The following files are used:
- `students.json`
- `rubric.json`
- `evaluations.json`

CSV reports are exported to:

`~/POSData/exports/`

The final-result export includes student ID, student name and final score. Students without evaluations have an empty final-score field.
## 9. Build and Run

Requirements: .NET 8 SDK.

Build the solution:

```bash
dotnet build
```

Run the application:

```bash
dotnet run --project src/POS.Console
```

## 10. Tests

Run the automated domain tests:

```bash
dotnet test
```

The tests cover:
- Score validation.
- Criterion validation.
- Evaluation creation.
- Weighted result calculation.
- Duplicate evaluation prevention.
- Complete-rubric calculation.

## 11. AI Assistance

AI tools were used as a supporting resource during the implementation of this assignment. They helped with understanding architectural concepts, discussing implementation approaches, writing and improving parts of the code, identifying and correcting mistakes, troubleshooting errors, and preparing tests and documentation.

AI assistance supported the development process, but the implementation was reviewed, tested, and refined by the author. The author is responsible for understanding the code and explaining the design decisions made in the project.