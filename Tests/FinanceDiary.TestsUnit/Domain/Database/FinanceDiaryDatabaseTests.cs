﻿using FakeItEasy;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.Database;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerators;
using FinanceDiary.Infra.Options;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDiary.TestsUnit.Domain.Database
{
    public class FinanceDiaryDatabaseTests
    {
        private const string CashRegisterCsv = "cash_register.csv";
        private const string FinanceOperationsCsv = "finance_operations.csv";
        private const string NeutralOperationsCsv = "neutral_operations.csv";

        private readonly IOperationsFactory mOperationsFactory;

        public FinanceDiaryDatabaseTests()
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            A.CallTo(() => idGenerator.GenerateId()).Returns("1");

            mOperationsFactory = new OperationsFactory(idGenerator);
        }

        [Fact]
        public async Task SaveCashRegistersToCsv_FileCreated()
        {
            string tempDatabaseDirectory = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;

            FinanceDiaryDatabase financeDiaryDatabase = CreateFakeDatabase(tempDatabaseDirectory);

            try
            {
                HashSet<CashRegister> cashRegisters = new HashSet<CashRegister>()
                {
                    new CashRegister("cash1"),
                    new CashRegister("cash2"),
                };

                await financeDiaryDatabase.SaveCashRegistersToCsv(cashRegisters).ConfigureAwait(false);
                string expectedCsvPath = Path.Combine(tempDatabaseDirectory, CashRegisterCsv);
                string[] lines = await File.ReadAllLinesAsync(expectedCsvPath)
                    .ConfigureAwait(false);

                Assert.Equal(3, lines.Length);
            }
            finally
            {
                Directory.Delete(tempDatabaseDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task SaveFinanceOperationsToCsv_FileCreated()
        {
            string tempDatabaseDirectory = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;

            FinanceDiaryDatabase financeDiaryDatabase = CreateFakeDatabase(tempDatabaseDirectory);

            try
            {
                List<FinanceOperation> financeOperations = new List<FinanceOperation>
                {
                    mOperationsFactory.CreateFinanceOperation("24/06/2018", OperationType.Deposit, 200, OperationKind.Commission, "reason"),
                    mOperationsFactory.CreateFinanceOperation("24/06/2019", OperationType.Deposit, 300, OperationKind.Commission, "reason"),
                    mOperationsFactory.CreateFinanceOperation("24/06/2020", OperationType.Deposit, 400, OperationKind.Commission, "reason"),
                };

                await financeDiaryDatabase.SaveFinanceOperationsToCsv(financeOperations)
                    .ConfigureAwait(false);
                string expectedCsvPath = Path.Combine(tempDatabaseDirectory, FinanceOperationsCsv);
                string[] lines = await File.ReadAllLinesAsync(expectedCsvPath)
                    .ConfigureAwait(false);

                Assert.Equal(4, lines.Length);
            }
            finally
            {
                Directory.Delete(tempDatabaseDirectory, recursive: true);
            }
        }

        [Fact]
        public async Task SaveNeutralOperationsToCsv_FileCreated()
        {
            string tempDatabaseDirectory = Directory.CreateDirectory(Path.GetRandomFileName()).FullName;

            FinanceDiaryDatabase financeDiaryDatabase = CreateFakeDatabase(tempDatabaseDirectory);

            try
            {
                List<NeutralOperation> neutralOperations = new List<NeutralOperation>
                {
                    mOperationsFactory.CreateNeutralOperation("24/06/2018", 200, new CashRegister("cash1"), new CashRegister("cash2"), "reason"),
                    mOperationsFactory.CreateNeutralOperation("24/06/2019", 300, new CashRegister("cash1"), new CashRegister("cash2"), "reason"),
                };

                await financeDiaryDatabase.SaveNeutralOperationsToCsv(neutralOperations)
                    .ConfigureAwait(false);
                string expectedCsvPath = Path.Combine(tempDatabaseDirectory, NeutralOperationsCsv);
                string[] lines = await File.ReadAllLinesAsync(expectedCsvPath)
                    .ConfigureAwait(false);

                Assert.Equal(3, lines.Length);
            }
            finally
            {
                Directory.Delete(tempDatabaseDirectory, recursive: true);
            }
        }

        private FinanceDiaryDatabase CreateFakeDatabase(string databaseDirectory)
        {
            DatabaseConfiguration databaseConfiguration = new DatabaseConfiguration
            {
                DatabasePath = databaseDirectory
            };

            IOptionsMonitor<DatabaseConfiguration> databaseOptions =
                A.Fake<IOptionsMonitor<DatabaseConfiguration>>();

            A.CallTo(() => databaseOptions.CurrentValue).Returns(databaseConfiguration);

            return new FinanceDiaryDatabase(databaseOptions);
        }

        [Fact]
        public async Task LoadCashRegistersFromCsv_CashRegistersLoadedCorrectly()
        {
            throw new System.NotImplementedException();
        }

        [Fact]
        public async Task LoadFinanceOperationsFromCsv_FinanceOperationsLoadedCorrectly()
        {
            throw new System.NotImplementedException();
        }

        [Fact]
        public async Task LoadNeutralOperationsFromCsvNeutralOperationsLoadedCorrectly()
        {
            throw new System.NotImplementedException();
        }
    }
}