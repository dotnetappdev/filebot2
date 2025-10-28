using System;
using System.IO;
using Xunit;
using RenameIt.Core;

namespace RenameIt.Tests
{
    public class TemplateRepositoryTests : IDisposable
    {
        private readonly string _testDbPath;
        private readonly TemplateRepository _repository;

        public TemplateRepositoryTests()
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_templates_{Guid.NewGuid()}.db");
            _repository = new TemplateRepository(_testDbPath);
        }

        public void Dispose()
        {
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }
        }

        [Fact]
        public void Add_ShouldAddTemplateAndReturnId()
        {
            // Arrange
            var template = new RenameTemplate
            {
                Name = "Test Template",
                Pattern = "{n} - {s00e00}",
                Description = "Test description"
            };

            // Act
            var id = _repository.Add(template);

            // Assert
            Assert.True(id > 0);
            Assert.Equal(id, template.Id);
        }

        [Fact]
        public void GetAll_ShouldReturnAllTemplates()
        {
            // Arrange
            _repository.Add(new RenameTemplate { Name = "Template 1", Pattern = "{n}", Description = "Desc 1" });
            _repository.Add(new RenameTemplate { Name = "Template 2", Pattern = "{t}", Description = "Desc 2" });

            // Act
            var templates = _repository.GetAll();

            // Assert
            Assert.Equal(2, templates.Count);
        }

        [Fact]
        public void GetById_ShouldReturnCorrectTemplate()
        {
            // Arrange
            var template = new RenameTemplate
            {
                Name = "Test Template",
                Pattern = "{n} - {s00e00}",
                Description = "Test description"
            };
            var id = _repository.Add(template);

            // Act
            var retrieved = _repository.GetById(id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(template.Name, retrieved.Name);
            Assert.Equal(template.Pattern, retrieved.Pattern);
            Assert.Equal(template.Description, retrieved.Description);
        }

        [Fact]
        public void GetById_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var retrieved = _repository.GetById(999);

            // Assert
            Assert.Null(retrieved);
        }

        [Fact]
        public void Update_ShouldUpdateTemplate()
        {
            // Arrange
            var template = new RenameTemplate
            {
                Name = "Original Name",
                Pattern = "{n}",
                Description = "Original Description"
            };
            var id = _repository.Add(template);

            // Act
            template.Name = "Updated Name";
            template.Pattern = "{n} - {s00e00}";
            template.Description = "Updated Description";
            _repository.Update(template);

            var retrieved = _repository.GetById(id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("Updated Name", retrieved.Name);
            Assert.Equal("{n} - {s00e00}", retrieved.Pattern);
            Assert.Equal("Updated Description", retrieved.Description);
        }

        [Fact]
        public void Delete_ShouldRemoveTemplate()
        {
            // Arrange
            var template = new RenameTemplate
            {
                Name = "Template to Delete",
                Pattern = "{n}",
                Description = "Will be deleted"
            };
            var id = _repository.Add(template);

            // Act
            _repository.Delete(id);
            var retrieved = _repository.GetById(id);

            // Assert
            Assert.Null(retrieved);
        }

        [Fact]
        public void SeedDefaultTemplates_ShouldAddTemplates()
        {
            // Act
            _repository.SeedDefaultTemplates();
            var templates = _repository.GetAll();

            // Assert
            Assert.True(templates.Count >= 5);
        }

        [Fact]
        public void SeedDefaultTemplates_CalledTwice_ShouldNotDuplicate()
        {
            // Act
            _repository.SeedDefaultTemplates();
            var count1 = _repository.GetAll().Count;
            _repository.SeedDefaultTemplates();
            var count2 = _repository.GetAll().Count;

            // Assert
            Assert.Equal(count1, count2);
        }
    }
}
