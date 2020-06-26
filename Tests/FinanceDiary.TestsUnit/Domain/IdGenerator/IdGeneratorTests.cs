using FakeItEasy;
using FinanceDiary.Infra.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Xunit;

namespace FinanceDiary.TestsUnit.Domain.IdGenerator
{
    public class IdGeneratorTests
    {
        private const string NextIdHolderName = "id_producer.db";

        [Fact]
        public void GenerateId_ValidCsvPathInConfiguration_IdAsExpected()
        {
            string tempDirectory = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;
            string tempIdHolderFile = Path.Combine(tempDirectory, NextIdHolderName);

            DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration
            { 
                CsvPath = tempIdHolderFile
            };

            IOptionsMonitor<DatabaseConfiguration> datebaseOptions = 
                A.Fake<IOptionsMonitor<DatabaseConfiguration>>();

            A.CallTo(() => datebaseOptions.CurrentValue).Returns(databaseConfiguration);

            try
            {
                File.WriteAllText(tempIdHolderFile, "48");

                FinanceDiary.Domain.IdGenerators.IdGenerator idGenerator = new
                    FinanceDiary.Domain.IdGenerators.IdGenerator(
                    datebaseOptions, NullLogger<FinanceDiary.Domain.IdGenerators.IdGenerator>.Instance);

                Assert.Equal("48", idGenerator.GenerateId());
                Assert.Equal("49", idGenerator.GenerateId());
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }

        [Fact]
        public void GenerateId_InvalidCsvPathInConfiguration_ThrowsArgumentException()
        {
            string tempDirectory = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;
            string tempIdHolderFile = Path.Combine(tempDirectory, NextIdHolderName);

            DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration
            {
                CsvPath = tempIdHolderFile
            };

            IOptionsMonitor<DatabaseConfiguration> datebaseOptions =
                A.Fake<IOptionsMonitor<DatabaseConfiguration>>();

            A.CallTo(() => datebaseOptions.CurrentValue).Returns(databaseConfiguration);

            try
            {
                File.WriteAllText(tempIdHolderFile, "bla-bla");

                Assert.Throws<ArgumentException>(() => new FinanceDiary.Domain.IdGenerators.IdGenerator(
                    datebaseOptions, NullLogger<FinanceDiary.Domain.IdGenerators.IdGenerator>.Instance));
            }
            finally
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }
    }
}