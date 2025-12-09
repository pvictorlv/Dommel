using Xunit;

namespace DommelExtended.IntegrationTests;

// Apply the text fixture to all tests in the "Database" collection
[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}
