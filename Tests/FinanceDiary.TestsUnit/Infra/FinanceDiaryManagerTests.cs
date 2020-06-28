using FakeItEasy;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.Database;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerators;
using FinanceDiary.Infra;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDiary.TestsUnit.Infra
{
    public class FinanceDiaryManagerTests
    {
        private const string DefaultCashRegisterName = "Default_Account";
        private readonly IFinanceDiaryDatabase mFinanceDiaryDatabase = A.Fake<IFinanceDiaryDatabase>();

        public FinanceDiaryManagerTests()
        {
            List<CashRegister> cashRegisters = new List<CashRegister>
            {
                new CashRegister(DefaultCashRegisterName)
            };

            A.CallTo(() => mFinanceDiaryDatabase.LoadCashRegistersFromCsv()).Returns(cashRegisters);
        }

        [Fact]
        public void Ctor_HasNoDefaultCachRegister_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new FinanceDiaryManager(
               A.Dummy<IOperationsFactory>(),
               A.Dummy<IFinanceDiaryDatabase>(),
               NullLogger<FinanceDiaryManager>.Instance));
        }

        [Fact]
        public void AddCashRegister_CashRegisterAlreadyExists_DoesNotAdd()
        {
            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                A.Dummy<IOperationsFactory>(),
                mFinanceDiaryDatabase,
                NullLogger<FinanceDiaryManager>.Instance);

            Assert.Single(financeDiaryManager.GetAllCashRegisters());
            Assert.False(financeDiaryManager.AddCashRegister("Default_Account"));

            Assert.Single(financeDiaryManager.GetAllCashRegisters());
        }

        [Fact]
        public void AddCashRegister_CashRegisterNotExists_CashRegisterAdded()
        {
            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                A.Dummy<IOperationsFactory>(),
                mFinanceDiaryDatabase,
                NullLogger<FinanceDiaryManager>.Instance);

            Assert.Single(financeDiaryManager.GetAllCashRegisters());
            Assert.True(financeDiaryManager.AddCashRegister("another new account"));

            Assert.Equal(2, financeDiaryManager.GetAllCashRegisters().Count());
        }

        [Theory]
        [InlineData("26/04//2012", OperationType.Deposit, 200, OperationKind.Family, "reason")]
        [InlineData("26/04/2012", OperationType.Deposit, -200, OperationKind.Family, "reason")]
        [InlineData("26/04/2012", OperationType.Deposit, 200, OperationKind.Family, "")]
        public void AddFinanceOperation_InvalidFinancialOperation_FinancialOperationNotAdded(
            string date, OperationType operationType, int amount, OperationKind operationKind, string reason)
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            A.CallTo(() => idGenerator.GenerateId()).Returns("1");

            IOperationsFactory operationsFactory = new OperationsFactory(idGenerator);

            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                operationsFactory,
                mFinanceDiaryDatabase,
                NullLogger<FinanceDiaryManager>.Instance);

            Assert.False(financeDiaryManager.AddFinanceOperation(
                date, operationType, amount, new List<OperationKind> { operationKind }, reason));
        }

        [Theory]
        [InlineData("26/04/2012", OperationType.Deposit, 200, OperationKind.Family, "reason")]
        public void AddFinanceOperation_ValidFinancialOperation_FinancialOperationAdded(
            string date, OperationType operationType, int amount, OperationKind operationKind, string reason)
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            A.CallTo(() => idGenerator.GenerateId()).Returns("1");

            IOperationsFactory operationsFactory = new OperationsFactory(idGenerator);

            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                operationsFactory,
                mFinanceDiaryDatabase,
                NullLogger<FinanceDiaryManager>.Instance);

            Assert.True(financeDiaryManager.AddFinanceOperation(
                date, operationType, amount, new List<OperationKind> { operationKind }, reason));
        }

        [Theory]
        [InlineData("26/04//2012", 200, "cash1", "cash2", "reason")]
        [InlineData("26/04/2012", 200, "cash1", "cash1", "reason")]
        [InlineData("26/04/2012", -200, "cash1", "cash2", "reason")]
        [InlineData("26/04/2012", 200, "cash1", "cash2", "")]
        [InlineData("26/04/2012", 200, "guid", "cash2", "reason")]
        [InlineData("26/04/2012", 200, "cash1", "guid", "reason")]
        public void AddNeutralOperation_InvalidNeutralOperation_NeutralOperationNotAdded(
            string date, int amount, string sourceCashRegisterName, string destinationCashRegisterName, string reason)
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            A.CallTo(() => idGenerator.GenerateId()).Returns("1");

            IOperationsFactory operationsFactory = new OperationsFactory(idGenerator);

            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                operationsFactory,
                mFinanceDiaryDatabase,
                NullLogger<FinanceDiaryManager>.Instance);

            financeDiaryManager.AddCashRegister("cash1");
            financeDiaryManager.AddCashRegister("cash2");

            if (sourceCashRegisterName.Equals("guid"))
                sourceCashRegisterName = Path.GetRandomFileName();

            if (destinationCashRegisterName.Equals("guid"))
                destinationCashRegisterName = Path.GetRandomFileName();

            Assert.False(financeDiaryManager.AddNeutralOperation(
                date, amount, sourceCashRegisterName, destinationCashRegisterName, reason));
        }

        [Theory]
        [InlineData("26/04/2012", 200, "cash1", "cash2", "reason")]
        public void AddNeutralOperation_ValidNeutralOperation_NeutralOperationAdded(
            string date, int amount, string sourceCashRegisterName, string destinationCashRegisterName, string reason)
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            A.CallTo(() => idGenerator.GenerateId()).Returns("1");

            IOperationsFactory operationsFactory = new OperationsFactory(idGenerator);

            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                operationsFactory,
                mFinanceDiaryDatabase,
                NullLogger<FinanceDiaryManager>.Instance);

            financeDiaryManager.AddCashRegister("cash1");
            financeDiaryManager.AddCashRegister("cash2");

            Assert.True(financeDiaryManager.AddNeutralOperation(
                date, amount, sourceCashRegisterName, destinationCashRegisterName, reason));
        }

        [Fact]
        public async Task SaveToDatabase_SaveMethodsAreCalled()
        {
            IFinanceDiaryDatabase financeDiaryDatabase = A.Fake<IFinanceDiaryDatabase>();
            IOperationsFactory operationsFactory = A.Fake<IOperationsFactory>();

            List <CashRegister> cashRegisters = new List<CashRegister>
            {
                new CashRegister(DefaultCashRegisterName)
            };

            A.CallTo(() => financeDiaryDatabase.LoadCashRegistersFromCsv()).Returns(cashRegisters);

            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                operationsFactory,
                financeDiaryDatabase,
                NullLogger<FinanceDiaryManager>.Instance);

            await financeDiaryManager.SaveToDatabase().ConfigureAwait(false);

            A.CallTo(() => financeDiaryDatabase.SaveCashRegistersToCsv(A<HashSet<CashRegister>>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => financeDiaryDatabase.SaveFinanceOperationsToCsv(A<List<FinanceOperation>>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => financeDiaryDatabase.SaveNeutralOperationsToCsv(A<List<NeutralOperation>>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => operationsFactory.SaveState()).MustHaveHappenedOnceExactly();
        }
    }
}