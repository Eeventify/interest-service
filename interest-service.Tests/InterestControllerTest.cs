using System;
using System.Data.Common;
using System.Linq;
using interest_service.Controllers;
using interest_service.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace interest_service.Tests
{
    public class InterestControllerTest : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<InterestContext> _contextOptions;

        #region ConstructorAndDispose
        public InterestControllerTest()
        {
            // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
            // at the end of the test (see Dispose below).
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // These options will be used by the context instances in this test suite, including the connection opened above.
            _contextOptions = new DbContextOptionsBuilder<InterestContext>()
                .UseSqlite(_connection)
                .Options;

            // Create the schema and seed some data
            using var context = new InterestContext(_contextOptions);

            if (context.Database.EnsureCreated())
            {
                using var viewCommand = context.Database.GetDbConnection().CreateCommand();
                viewCommand.CommandText = @"
CREATE VIEW AllResources AS
SELECT *
FROM Interests;";
                viewCommand.ExecuteNonQuery();
            }

            context.AddRange(
                new Interest { Name = "BBB", Description = "Desc One" },
                new Interest { Name = "CCC", Description = "Desc Two" },
                new Interest { Name = "AAA", Description = "Desc Three" });
            context.SaveChanges();
        }

        InterestContext CreateContext() => new InterestContext(_contextOptions);

        public void Dispose() => _connection.Dispose();
        #endregion

        [Fact]
        public async void GetInterest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new InterestsController(context);

            // Act
            var result = await controller.GetInterest(1);
            var interest = result.Value;

            // Assert
            Assert.NotNull(interest);
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("Desc One", interest.Description);
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        [Fact]
        public async void GetInterests()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new InterestsController(context);

            // Act
            var result = await controller.GetInterests();
            var interests = result.Value;

            // Assert
            Assert.Collection(
                interests,
                b => Assert.Equal("BBB", b.Name),
                b => Assert.Equal("CCC", b.Name),
                b => Assert.Equal("AAA", b.Name));
        }

        [Fact]
        public async void GetInterestsSorted()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new InterestsController(context);

            // Act
            var result = await controller.GetInterests(sort: true);
            var interests = result.Value;

            // Assert
            Assert.Collection(
                interests,
                b => Assert.Equal("AAA", b.Name),
                b => Assert.Equal("BBB", b.Name),
                b => Assert.Equal("CCC", b.Name));
        }

        [Fact]
        public async void GetInterestsSearch()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new InterestsController(context);

            // Act
            var result = await controller.GetInterests(search: "A");
            var interests = result.Value;

            // Assert
            Assert.Collection(
                interests,
                b => Assert.Equal("AAA", b.Name));
        }

        [Fact]
        public async void AddInterest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new InterestsController(context);
            var interest = new Interest { Name = "DDD", Description = "Desc Four" };

            // Act
            await controller.PostInterest(interest);

            // Assert
            var result = context.Interests.Single(b => b.Name == "DDD");
            Assert.Equal("Desc Four", result.Description);
        }

        [Fact]
        public async void UpdateInterestDesc()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new InterestsController(context);
            var interest = new Interest { Id = 2, Name = "CCC", Description = "New Desc" };

            // Act
            await controller.PutInterest(2, interest);

            // Assert
            var result = context.Interests.Single(b => b.Name == "CCC");
            Assert.Equal("New Desc", result.Description);
        }

        [Fact]
        public async void DeleteInterest()
        {
            // Arrange
            using var context = CreateContext();
            var controller = new InterestsController(context);

            // Act
            await controller.DeleteInterest(1);

            // Assert
            Assert.Throws<ArgumentException>(() => context.Interests.Find(1));
            Assert.Equal(2, context.Interests.Count());
        }
    }
}