using FakeItEasy;
using FinanceDiary.Domain.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDiary.TestsUnit.Domain.IdGenerator
{
    public class IdGeneratorTests
    {
        private const string NextIdHolderName = "id_producer.db";

        [Fact]
        public async Task Ctor_InvalidCsvPathInConfiguration_ThrowsArgumentException()
        {
            string tempDirectory = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;
            string tempIdHolderFile = Path.Combine(tempDirectory, NextIdHolderName);

            DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration
            {
                DatabasePath = tempDirectory
            };

            IOptionsMonitor<DatabaseConfiguration> datebaseOptions =
                A.Fake<IOptionsMonitor<DatabaseConfiguration>>();

            A.CallTo(() => datebaseOptions.CurrentValue).Returns(databaseConfiguration);

            try
            {
                await File.WriteAllTextAsync(tempIdHolderFile, "bla-bla").ConfigureAwait(false);

                Assert.Throws<ArgumentException>(() => new FinanceDiary.Domain.IdGenerators.IdGenerator(datebaseOptions));
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task GenerateId_ValidCsvPathInConfiguration_IdAsExpected()
        {
            string tempDirectory = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;
            string tempIdHolderFile = Path.Combine(tempDirectory, NextIdHolderName);

            DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration
            { 
                DatabasePath = tempDirectory
            };

            IOptionsMonitor<DatabaseConfiguration> datebaseOptions = 
                A.Fake<IOptionsMonitor<DatabaseConfiguration>>();

            A.CallTo(() => datebaseOptions.CurrentValue).Returns(databaseConfiguration);

            try
            {
                await File.WriteAllTextAsync(tempIdHolderFile, "48").ConfigureAwait(false);

                FinanceDiary.Domain.IdGenerators.IdGenerator idGenerator = new
                    FinanceDiary.Domain.IdGenerators.IdGenerator(datebaseOptions);

                Assert.Equal("48", idGenerator.GenerateId());
                Assert.Equal("49", idGenerator.GenerateId());
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task SaveState_ValidCsvPathInConfiguration_IdAsExpected()
        {
            string tempDirectory = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;
            string tempIdHolderFile = Path.Combine(tempDirectory, NextIdHolderName);

            DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration
            {
                DatabasePath = tempDirectory
            };

            IOptionsMonitor<DatabaseConfiguration> datebaseOptions =
                A.Fake<IOptionsMonitor<DatabaseConfiguration>>();

            A.CallTo(() => datebaseOptions.CurrentValue).Returns(databaseConfiguration);

            try
            {
                await File.WriteAllTextAsync(tempIdHolderFile, "48").ConfigureAwait(false);

                FinanceDiary.Domain.IdGenerators.IdGenerator idGenerator = new
                    FinanceDiary.Domain.IdGenerators.IdGenerator(datebaseOptions);

                idGenerator.GenerateId();
                idGenerator.GenerateId();
                await idGenerator.SaveState().ConfigureAwait(false);

                Assert.Equal("50", await File.ReadAllTextAsync(tempIdHolderFile).ConfigureAwait(false));
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }
    }
}