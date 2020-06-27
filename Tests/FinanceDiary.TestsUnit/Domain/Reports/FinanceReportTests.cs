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
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(13)]
        public void FilterByMonth_InvalidMonth_ReturnNullReport(int month)
        {
            HashSet<CashRegister> cashRegisters = new HashSet<CashRegister>
            {
                new CashRegister("cash1")
            };

            FinanceReport financeReport = new FinanceReport(cashRegisters, new List<FinanceOperation>(), new List<NeutralOperation>());
            Assert.Equal(FinanceReport.NullFinanceReport, financeReport.FilterByMonth(month));
        }

        [Fact]
        public void FilterByMonth_ValidMonth_ReturnFilteredReport()
        {
            HashSet<CashRegister> cashRegisters = new HashSet<CashRegister>
            {
                new CashRegister("cash1")
            };

            List<FinanceOperation> financeOperations = new List<FinanceOperation>
            {
                mOperationsFactory.CreateFinanceOperation("24/06/2022", OperationType.Deposit, 300, new List<OperationKind> { OperationKind.Friends }, "reason"),
                mOperationsFactory.CreateFinanceOperation("24/07/2022", OperationType.Deposit, 300, new List<OperationKind> { OperationKind.Friends }, "reason"),
                mOperationsFactory.CreateFinanceOperation("22/06/2022", OperationType.Deposit, 300, new List<OperationKind> { OperationKind.Friends }, "reason")
            };

            List<NeutralOperation> neutralOperations = new List<NeutralOperation>
            {
                mOperationsFactory.CreateNeutralOperation("21/06/2022", 300, "cash1", "cash2", "reason"),
                mOperationsFactory.CreateNeutralOperation("28/06/2022", 300, "cash1", "cash2", "reason"),
                mOperationsFactory.CreateNeutralOperation("24/09/2022", 300, "cash1", "cash2", "reason"),
            };

            FinanceReport financeReport = new FinanceReport(cashRegisters, financeOperations, neutralOperations);
            FinanceReport filteredFinanceReport = financeReport.FilterByMonth(6);

            Assert.Equal(filteredFinanceReport.CashRegisters, financeReport.CashRegisters);
            Assert.Equal(2, filteredFinanceReport.FinanceOperations.Count());
            Assert.Equal(2, filteredFinanceReport.NeutralOperations.Count());
        }
    }
}