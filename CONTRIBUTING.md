# Contributing to FileBot2

Thank you for considering contributing to FileBot2! This document provides guidelines and information for contributors.

## Development Setup

### Prerequisites

- Windows 10 version 1809 (build 17763) or later / Windows 11
- .NET 8 SDK or later
- Visual Studio 2022 with:
  - .NET Desktop Development workload
  - Windows App SDK components
- Git

### Getting Started

1. Fork the repository
2. Clone your fork:
   ```bash
   git clone https://github.com/YOUR-USERNAME/filebot2.git
   cd filebot2
   ```

3. Open the solution in Visual Studio:
   ```bash
   start FileBot2.sln
   ```

4. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

5. Build the solution:
   ```bash
   dotnet build
   ```

## Project Structure

```
filebot2/
├── FileBot2/              # Main WinUI 3 application
│   ├── App.xaml          # Application entry point
│   ├── MainWindow.xaml   # Main window UI
│   └── ...
├── FileBot2.Core/        # Core business logic (platform-independent)
│   ├── FileNameParser.cs
│   ├── FileRenamer.cs
│   └── MetadataProviders.cs
└── FileBot2.Tests/       # Unit tests
    ├── FileNameParserTests.cs
    └── FileRenamerTests.cs
```

## Development Guidelines

### Code Style

- Follow standard C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and single-purpose

### Making Changes

1. Create a feature branch:
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. Make your changes following the code style guidelines

3. Write tests for new functionality:
   - Add tests to `FileBot2.Tests` project
   - Ensure all tests pass:
     ```bash
     dotnet test
     ```

4. Update documentation if needed

5. Commit your changes:
   ```bash
   git commit -m "Description of your changes"
   ```

6. Push to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```

7. Create a Pull Request

## Testing

### Running Tests

Run all tests:
```bash
dotnet test
```

Run specific test class:
```bash
dotnet test --filter FileNameParserTests
```

Run specific test:
```bash
dotnet test --filter ParseFileName_TVShow_StandardFormat_ShouldParseCorrectly
```

### Writing Tests

- Use xUnit for testing
- Follow the Arrange-Act-Assert pattern
- Name tests descriptively: `MethodName_Scenario_ExpectedBehavior`
- Test both success and failure cases
- Keep tests independent and isolated

Example:
```csharp
[Fact]
public void ParseFileName_TVShow_StandardFormat_ShouldParseCorrectly()
{
    // Arrange
    var parser = new FileNameParser();
    string fileName = "Breaking.Bad.S01E02.Title.mkv";

    // Act
    var result = parser.ParseFileName(fileName);

    // Assert
    Assert.True(result.IsTV);
    Assert.Equal("Breaking Bad", result.ShowName);
}
```

## Adding New Features

### Format Tokens

To add a new format token:

1. Add the token to `FileRenamer.cs`:
   ```csharp
   result = result.Replace("{token}", value);
   ```

2. Update the documentation in `SYNTAX_GUIDE.md`

3. Add tests to `FileRenamerTests.cs`

### Metadata Providers

To add a new metadata provider:

1. Implement `IMetadataProvider` in `MetadataProviders.cs`
2. Add the provider to the ComboBox in `MainWindow.xaml`
3. Update the switch statement in `MainWindow.xaml.cs`
4. Document the new provider in `README.md`

### File Format Parsers

To support a new file naming format:

1. Add a regex pattern to `FileNameParser.cs`
2. Add test cases to `FileNameParserTests.cs`
3. Document the format in `SYNTAX_GUIDE.md`

## Pull Request Process

1. Ensure all tests pass
2. Update the README.md with details of changes if applicable
3. Update the SYNTAX_GUIDE.md if you've added new format tokens
4. The PR will be reviewed by maintainers
5. Address any feedback from code review
6. Once approved, your PR will be merged

## Bug Reports

When filing a bug report, please include:

- Windows version
- .NET version
- Steps to reproduce
- Expected behavior
- Actual behavior
- Screenshots if applicable
- Sample filenames that cause the issue (if applicable)

## Feature Requests

Feature requests are welcome! Please:

- Check if the feature has already been requested
- Clearly describe the feature and its benefits
- Provide examples of how it would be used
- Consider contributing the feature yourself!

## Code of Conduct

- Be respectful and inclusive
- Welcome newcomers and help them learn
- Focus on constructive feedback
- Keep discussions professional and on-topic

## Questions?

If you have questions about contributing, feel free to:
- Open an issue with the "question" label
- Start a discussion in the repository

Thank you for contributing to FileBot2! 🎉
