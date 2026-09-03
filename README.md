# Examination System - Backend Starter Template

Welcome to the **Examination System** backend project. This solution is built with **.NET 10**, adhering to **Vertical Slice Architecture (VSA)**, **CQRS (Command Query Responsibility Segregation)** with MediatR, **Entity Framework Core**, and the **Unit of Work & Generic Repository** pattern.

---

## 1. Project Architecture & Standards

### A. Vertical Slice Architecture (VSA)
Features are organized around business capabilities rather than technical layers. Everything a feature needs lives inside its feature folder:

```
Features/<Module>/<FeatureName>/
├── Commands/          # MediatR Command definitions (Request payload for write operations)
├── Queries/           # MediatR Query definitions (Request parameters for read operations)
├── Handlers/          # MediatR Request Handlers containing business logic
├── Controllers/       # Thin API Controllers delegating directly to IMediator
├── Validators/        # FluentValidation rules for commands and queries
├── Orchestrators/     # Reserved for complex multi-entity workflows (e.g., attempt scoring & timing)
└── *Response.cs       # The single, strongly typed response model for the feature
```

### B. Standardized Response Envelope
All API endpoints return standard envelope structures found in `Features/Shared/`:
- **`ApiResponse<T>`**: Standard response envelope (`Success`, `StatusCode`, `Message`, `Data`, `Errors`, `Timestamp`).
- **`EndpointResponse<T>`**: Controller-facing response model that inherits from `ApiResponse<T>`.
- **`RequestResponse<T>`**: Handler result wrapper (`RequestResponse<T>.Ok(...)`, `Created(...)`, `Fail(...)`).
- **`PaginatedResult<T>`**: Standard container for paginated queries (`Items`, `TotalCount`, `PageIndex`, `PageSize`, `TotalPages`).

---

## 2. Solution Structure

```
├── Common/
│   ├── Behaviors/               # MediatR validation pipeline behavior (ValidationBehavior)
│   ├── Enums/                   # UserRole, AccountStatus, QuizStatus, AttemptStatus
│   ├── Exceptions/              # NotFoundException, ValidationException, ConflictException
│   └── Middleware/              # GlobalExceptionHandler formatting errors into ApiResponse
│
├── Domain/
│   ├── Common/
│   │   └── BaseEntity.cs        # Id (Guid), CreatedAt, UpdatedAt, IsDeleted, DeletedAt
│   └── Entities/
│       ├── Identity/            # ApplicationUser, Student, EmailVerificationOtp, PasswordResetOtp, RefreshToken
│       ├── Diplomas/            # Diploma, StudentEnrollment
│       ├── Quizzes/             # Quiz, Question, QuestionOption
│       └── Attempts/            # QuizAttempt, StudentQuestionAnswer
│
├── Persistence/
│   ├── Context/                 # AppDbContext (with soft-delete query filters) & AppDbContextSeed
│   ├── Configurations/          # Fluent API entity configurations
│   └── DataAccess/              # IGenericRepository<T>, GenericRepository<T>, IUnitOfWork, UnitOfWork
│
├── Features/                    # All 5 Modules and 27 feature slices
│   ├── Shared/                  # ApiResponse, EndpointResponse, RequestResponse, PaginatedResult
│   ├── Identity/                # Register, VerifyEmailOtp, Login, RefreshToken, Logout, ForgotPassword
│   ├── Diplomas/                # BrowseDiplomas, GetDiplomaDetail, EnrollDiploma, StudentDashboard, Admin Diploma CRUD
│   ├── Quizzes/                 # Admin Quiz CRUD, Question Management, Publish Readiness, Publish/Unpublish
│   ├── Attempts/                # StartAttempt, SubmitAnswer, CheckRemainingTime, SubmitAttempt, Results, History
│   └── Analytics/               # AdminDashboard, PerformanceAnalytics, SearchAttempts, AttemptDetail
│
├── Program.cs                   # Service registration, middleware, automatic seed, Swagger UI
└── appsettings.json             # Local SQL Server connection string
```

---

## 3. Core Domain Entities & Relationships

| Entity | Description | Key Relationships |
| :--- | :--- | :--- |
| **`ApplicationUser`** | System auth account, password hash, role (`Student`, `Admin`), lockout | 1-to-1 with `Student`, 1-to-many with security tokens |
| **`Student`** | Dedicated student profile | 1-to-1 with `ApplicationUser`, 1-to-many with `StudentEnrollment` & `QuizAttempt` |
| **`Diploma`** | Learning catalog programs | 1-to-many with `Quiz` & `StudentEnrollment` |
| **`StudentEnrollment`** | Student enrollment into a diploma | Unique composite `(StudentId, DiplomaId)` |
| **`Quiz`** | Assessment with duration, pass score, max attempts | Belongs to `Diploma`, 1-to-many with `Question` & `QuizAttempt` |
| **`Question`** | Assessment question with explanation and order index | Belongs to `Quiz`, 1-to-many with `QuestionOption` |
| **`QuestionOption`** | Possible answer option | Belongs to `Question`, has `IsCorrect` flag |
| **`QuizAttempt`** | Timed student exam attempt | Belongs to `Student` and `Quiz`, has server `Deadline`, `Score`, and `Passed` |
| **`StudentQuestionAnswer`** | Answer chosen by student per question per attempt | Belongs to `QuizAttempt` and `Question` |

---

## 4. Getting Started

### A. Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Local SQL Server instance (configured in `appsettings.json` as `Server=.;Database=...`)
- Visual Studio 2026 / 2022 or VS Code

### B. Running the Project
1. Open the `.slnx` solution file in Visual Studio or navigate to the directory in terminal.
2. Ensure local SQL Server is running.
3. Run the project:
   ```bash
   dotnet run
   ```
4. When the app starts in `Development` mode:
   - EF Core will automatically seed sample data if tables are empty (Admin user, Students, Diplomas, Quizzes, Questions, Attempts).
   - The browser will open directly to **Swagger UI** at `http://localhost:5210/swagger`.
   - You can immediately test the minimal API verification endpoint: `GET /api/test/diplomas`.

---

## 5. Team Implementation Guidelines

1. **Keep Vertical Slices Self-Contained**: Implement your feature endpoints inside their designated `Features/<Module>/<Feature>/` folder.
2. **Commands & Queries as Requests**: Let your `*Command` or `*Query` serve directly as the request model for your controller — avoid unnecessary intermediate DTO-to-Command mapping.
3. **Use the Generic Repository & Unit of Work**:
   ```csharp
   // Reading entities with multi-include
   var quiz = await _unitOfWork.Repository<Quiz>()
       .GetByIdAsync(quizId, q => q.Questions);

   // Adding entity
   await _unitOfWork.Repository<StudentEnrollment>().AddAsync(enrollment);
   await _unitOfWork.SaveChangesAsync(cancellationToken);
   ```
4. **Validation**: Add validation rules inside `Validators/` inheriting from `AbstractValidator<TCommand>`. MediatR’s `ValidationBehavior` will automatically execute it before reaching your handler.
