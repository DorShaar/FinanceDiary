using FakeItEasy;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerators;
using FinanceDiary.Infra;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDiary.TestsUnit.Infra
{
    public class FinanceDiaryManagerTests
    {
        [Fact]
        public void Ctor_HasDefaultAccount()
        {
            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                A.Dummy<IOperationsFactory>(), NullLogger<FinanceDiaryManager>.Instance);

            IEnumerable<CashRegister> cashRegisters = financeDiaryManager.GetAllCashRegisters();
            Assert.Equal("Default Account", cashRegisters.First().Name);
        }

        [Fact]
        public void AddCashRegister_CashRegisterAlreadyExists_DoesNotAdd()
        {
            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                A.Dummy<IOperationsFactory>(), NullLogger<FinanceDiaryManager>.Instance);

            Assert.Single(financeDiaryManager.GetAllCashRegisters());
            Assert.False(financeDiaryManager.AddCashRegister("Default Account"));

            Assert.Single(financeDiaryManager.GetAllCashRegisters());
        }

        [Fact]
        public void AddCashRegister_CashRegisterNotExists_CashRegisterAdded()
        {
            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                 A.Dummy<IOperationsFactory>(), NullLogger<FinanceDiaryManager>.Instance);

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
                operationsFactory, NullLogger<FinanceDiaryManager>.Instance);

            Assert.False(financeDiaryManager.AddFinanceOperation(
                date, operationType, amount, operationKind, reason));
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
                operationsFactory, NullLogger<FinanceDiaryManager>.Instance);

            Assert.True(financeDiaryManager.AddFinanceOperation(
                date, operationType, amount, operationKind, reason));
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
                operationsFactory, NullLogger<FinanceDiaryManager>.Instance);

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
                operationsFactory, NullLogger<FinanceDiaryManager>.Instance);

            financeDiaryManager.AddCashRegister("cash1");
            financeDiaryManager.AddCashRegister("cash2");

            Assert.True(financeDiaryManager.AddNeutralOperation(
                date, amount, sourceCashRegisterName, destinationCashRegisterName, reason));
        }

        [Fact]
        public async Task SaveToCsv_Test()
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            A.CallTo(() => idGenerator.GenerateId()).Returns("1");

            IOperationsFactory operationsFactory = new OperationsFactory(idGenerator);

            FinanceDiaryManager financeDiaryManager = new FinanceDiaryManager(
                operationsFactory, NullLogger<FinanceDiaryManager>.Instance);

            FinanceOperation expectedFinanceOperation = operationsFactory.CreateFinanceOperation(
                "24/06/2020", OperationType.Withdraw, 3000, OperationKind.CreditCard, "testing");

            financeDiaryManager.AddFinanceOperation(
                expectedFinanceOperation.Date.ToString(), 
                expectedFinanceOperation.OperationType,
                expectedFinanceOperation.Amount,
                expectedFinanceOperation.OperationKind,
                expectedFinanceOperation.Reason);

            string tempCsvFile = Path.GetRandomFileName() + ".csv";

            try
            {
                await financeDiaryManager.SaveToCsv(tempCsvFile);
                Assert.True(File.Exists(tempCsvFile));
            }
            finally
            {
                File.Delete(tempCsvFile);
            }
        }
    }
}