# Isolated DB Tests (IsoTests)

## Overview
This is a POC project to making a similar SQL DB TRANSACTION mechanisms like in the [rspec-rails](https://rspec.info/features/8-0/rspec-rails/Transactions/).

The name was just quickly made up so currently, the actual app's feature and data contexts doesn't really have any specific contexts.

## Architecture

- **Framework**: .NET 8 Web API
- **Database**: PostgreSQL with Entity Framework Core
- **ORM**: Entity Framework Core with Npgsql provider
- **Testing**: xUnit v3 with custom transactional test base

## Domain Model

Currently the domain model context for this POC is about a simple video game catalog system with:
- **VideoGame**: Games with name, description, and release date
- **Genre**: Game genres (Action, Adventure, RPG, etc.)
- **VideoGameGenreMapping**: Many-to-many relationship between games and genres

## Key Features

### Isolated Testing Pattern
The core innovation is the [`TransactionalTestBase<TContext>`](tests/IsoTests.Tests/Fixtures/TransactionalTestBase.cs) class that:
- Begins a database transaction before each test.
- Rolls back the transaction after test completion.
- Ensures complete test isolation without data pollution.
- Allows tests to run in parallel safely.

### Test Infrastructure
- [`DbContextFactoryFixture`](tests/IsoTests.Tests/Fixtures/DbContextFactoryFixture.cs): Sets up test database context factory.
- Uses separate test database (`iso_test_db`) from development (`iso_db`).
- Automatic database creation and seeding.

### Code Coverage Report
- Automatic code coverage report using [ReportGenerator](https://reportgenerator.io/usage).
- Automatic code coverage report on PR.

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL server
- Connection to localhost:5432 with `postgres` user

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd IsoTests
   ```

2. **Start PostgreSQL** (ensure it's running on localhost:5432)

3. **Run database migrations**
   ```bash
   cd src
   dotnet tool restore
   dotnet dotnet-ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run --project src
   ```

5. **Run tests**
   ```bash
   dotnet test
   ```

### Database Configuration

- **Development**: `iso_db` (configured in [appsettings.json](src/appsettings.json))
- **Testing**: `iso_test_db` (configured in [appsettings.Test.json](src/appsettings.Test.json))

## Testing Examples

### Basic Entity Test
```csharp
public class GenreTests(DbContextFactoryFixture factoryFixture)
    : TransactionalTestBase<IsoDbContext>(factoryFixture.Factory)
{
    [Theory]
    [InlineData(1, "Action")]
    public void Should_Initialize_Genre_Correctly(int id, string name)
    {
        var genre = new Genre { Id = id, Name = name };
        Assert.Equal(id, genre.Id);
        Assert.Equal(name, genre.Name);
    }
}
```

### Controller Integration Test
```csharp
[Theory]
[ClassData(typeof(VideoGameFormFactory))]
public async Task Can_Add_VideoGame(CreateVideoGameForm newGame)
{
    var vgController = new VideoGameController(Db);
    var result = await vgController.Create(newGame, CancellationToken.None);
    
    // Assertions...
}
```

> [!NOTE]
> For better testing setups, we should consider using [ASP.NET Core integration test technique](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0).

## Project Structure

```
├── src/                          # Main application
│   ├── Controllers/              # API controllers
│   ├── Entities/                 # Domain models & DbContext
│   ├── Dtos/                     # Data transfer objects
│   └── Migrations/               # EF Core migrations
├── tests/
│   └── IsoTests.Tests/          # Test project
│       ├── Fixtures/            # Test infrastructure
│       └── Tests/               # Test classes
└── .github/workflows/           # CI/CD pipeline
```

## CI/CD

GitHub Actions workflow ([build-and-test.yml](.github/workflows/build-and-test.yml)) provides:
- Automated testing with PostgreSQL service.
- Code coverage reporting with Cobertura format.
- Coverage reports uploaded as artifacts.
- PR comments with coverage summaries.