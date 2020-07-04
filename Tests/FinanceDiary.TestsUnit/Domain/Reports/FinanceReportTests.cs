using FakeItEasy;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerators;
using FinanceDiary.Domain.Reports;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FinanceDiary.TestsUnit.Domain.Reports
{
    public class FinanceReportTests
    {
        private readonly IOperationsFactory mOperationsFactory;

        public FinanceReportTests()
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            A.CallTo(() => idGenerator.GenerateId()).Returns("1");

            mOperationsFactory = new OperationsFactory(idGenerator);
        }

        [Theory]
        [InlineData(0, 2020)]
        [InlineData(-1, 2020)]
        [InlineData(13, 2020)]
        public void FilterByMonthAndYear_InvalidMonth_ReturnNullReport(int month, int year)
        {
            HashSet<CashRegister> cashRegisters = new HashSet<CashRegister>
            {
                new CashRegister("cash1")
            };

            FinanceReport financeReport = new FinanceReport(cashRegisters, new List<FinanceOperation>(), new List<NeutralOperation>());
            Assert.Equal(FinanceReport.NullFinanceReport, financeReport.FilterByMonthAndYear(month, year));
        }

        [Fact]
        public void FilterByMonthAndYear_InvalidYear_ReturnEmptyReport()
        {
            HashSet<CashRegister> cashRegisters = new HashSet<CashRegister>
            {
                new CashRegister("cash1"),
                new CashRegister("Default_Account")
            };

            List<FinanceOperation> financeOperations = new List<FinanceOperation>
            {
                mOperationsFactory.CreateFinanceOperation("24/06/2022", OperationType.Deposit, 300, new List<OperationKind> { OperationKind.Friends }, "reason"),
                mOperationsFactory.CreateFinanceOperation("24/07/2022", OperationType.Deposit, 300, new List<OperationKind> { OperationKind.Friends }, "reason"),
                mOperationsFactory.CreateFinanceOperation("22/06/2022", OperationType.Deposit, 300, new List<OperationKind> { OperationKind.Friends }, "reason")
            };

            List<NeutralOperation> neutralOperations = new List<NeutralOperation>
            {
                mOperationsFactory.CreateNeutralOperation("21/06/2022", 300, "cash1", "Default_Account", "reason"),
                mOperationsFactory.CreateNeutralOperation("28/06/2022", 300, "cash1", "Default_Account", "reason"),
                mOperationsFactory.CreateNeutralOperation("24/09/2022", 300, "cash1", "Default_Account", "reason"),
            };

            FinanceReport financeReport = new FinanceReport(cashRegisters, financeOperations, neutralOperations);
            FinanceReport filteredFinanceReport = financeReport.FilterByMonthAndYear(6, 2019);

            Assert.Equal(filteredFinanceReport.CashRegisters, financeReport.CashRegisters);
            Assert.Empty(filteredFinanceReport.FinanceOperations);
            Assert.Empty(filteredFinanceReport.NeutralOperations);
        }

        [Fact]
        public void FilterByMonthAndYear_ValidMonth_ReturnFilteredReportAsExpected()
        {
            List<FinanceOperation> financeOperations = new List<FinanceOperation>
            {
                mOperationsFactory.CreateFinanceOperation("24/06/2020", OperationType.Deposit, 500, new List<OperationKind> { OperationKind.Friends }, "reason"),
                mOperationsFactory.CreateFinanceOperation("24/07/2020", OperationType.Deposit, 600, new List<OperationKind> { OperationKind.Friends }, "reason"),
                mOperationsFactory.CreateFinanceOperation("22/06/2020", OperationType.Deposit, 700, new List<OperationKind> { OperationKind.Friends }, "reason")
            };

            List<NeutralOperation> neutralOperations = new List<NeutralOperation>
            {
                mOperationsFactory.CreateNeutralOperation("21/06/2020", 400, "cash1", "Default_Account", "reason"),
                mOperationsFactory.CreateNeutralOperation("28/06/2020", 300, "cash1", "Default_Account", "reason"),
                mOperationsFactory.CreateNeutralOperation("24/09/2020", 350, "cash1", "Default_Account", "reason"),
            };

            CashRegister defaultCashRegister = new CashRegister("Default_Account");
            defaultCashRegister.Deposit(500);
            defaultCashRegister.Deposit(600);
            defaultCashRegister.Deposit(700);

            CashRegister cash1Register = new CashRegister("cash1", 2000);
            cash1Register.Withdraw(400);
            defaultCashRegister.Deposit(400);
            cash1Register.Withdraw(300);
            defaultCashRegister.Deposit(300);
            cash1Register.Withdraw(350);
            defaultCashRegister.Deposit(350);

            HashSet<CashRegister> cashRegisters = new HashSet<CashRegister>(new CashRegisterComparer())
            {
                defaultCashRegister, cash1Register
            };

            FinanceReport financeReport = new FinanceReport(cashRegisters, financeOperations, neutralOperations);
            FinanceReport filteredFinanceReport = financeReport.FilterByMonthAndYear(6, 2020);

            filteredFinanceReport.CashRegisters.TryGetValue(new CashRegister("Default_Account"), out CashRegister defaultFilteredCashRegister);
            filteredFinanceReport.CashRegisters.TryGetValue(new CashRegister("cash1"), out CashRegister cash1FilteredRegister);

            Assert.Equal(0, defaultFilteredCashRegister.InitialAmount);
            Assert.Equal(1900, defaultFilteredCashRegister.CurrentAmount);

            Assert.Equal(2000, cash1FilteredRegister.InitialAmount);
            Assert.Equal(1300, cash1FilteredRegister.CurrentAmount);

            Assert.Equal(2, filteredFinanceReport.FinanceOperations.Count());
            Assert.Equal(2, filteredFinanceReport.NeutralOperations.Count());
        }
    }
}