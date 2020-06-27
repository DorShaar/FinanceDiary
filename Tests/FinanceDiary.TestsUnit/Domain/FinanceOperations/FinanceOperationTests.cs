using FakeItEasy;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FinanceDiary.TestsUnit.Domain.FinanceOperations
{
    public class FinanceOperationTests
    {
        private readonly IOperationsFactory mOperationsFactory;

        public FinanceOperationTests()
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            A.CallTo(() => idGenerator.GenerateId()).Returns("1");

            mOperationsFactory = new OperationsFactory(idGenerator);
        }

        [Theory]
        [InlineData("20/01/1992")]
        [InlineData("20/01/1992 00:00:00")]
        public void Ctor_ValidParametersDateFormat_CorrectParametrs(string date)
        {
            OperationType operationType = OperationType.Deposit;
            int amount = 30;
            IEnumerable<OperationKind> operationKinds = new List<OperationKind> { OperationKind.Family, OperationKind.Fun };
            string reason = "reason";

            FinanceOperation financeOperation = mOperationsFactory.CreateFinanceOperation(date,
                operationType,
                amount,
                operationKinds,
                reason);

            Assert.Equal(20, financeOperation.Date.Day);
            Assert.Equal(1, financeOperation.Date.Month);
            Assert.Equal(1992, financeOperation.Date.Year);

            Assert.Equal(operationType, financeOperation.OperationType);
            Assert.Equal(amount, financeOperation.Amount);
            Assert.True(AreOperationKindsEqual(operationKinds, financeOperation.OperationKinds));
            Assert.Equal(reason, financeOperation.Reason);
        }

        private bool AreOperationKindsEqual(IEnumerable<OperationKind> operationKinds, OperationKinds operationKindsObj)
        {
            List<OperationKind> operationKindsList = operationKinds.ToList();
            int maxLength = Math.Min(operationKindsList.Count, 3);

            for (int i = 0; i < maxLength; ++i)
            {
                if (i == 0 && operationKindsObj.OperationKind1 != operationKindsList[i])
                    return false;

                if (i == 1 && operationKindsObj.OperationKind2 != operationKindsList[i])
                    return false;

                if (i == 2 && operationKindsObj.OperationKind3 != operationKindsList[i])
                    return false;
            }

            return true;
        }

        [Fact]
        public void Ctor_InvalidDate_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => mOperationsFactory.CreateFinanceOperation(
                "32/01/1992", OperationType.Deposit, 30, new List<OperationKind> { OperationKind.Commission }, "reason"));
        }

        [Fact]
        public void Ctor_InvalidOperationType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => mOperationsFactory.CreateFinanceOperation(
                "28/01/1992", (OperationType)(-1), 30, new List<OperationKind> { OperationKind.Commission }, "reason"));
        }

        [Fact]
        public void Ctor_InvalidOperationKind_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => mOperationsFactory.CreateFinanceOperation(
                "28/01/1992", OperationType.Deposit, 30, new List<OperationKind> { (OperationKind)(-1) }, "reason"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        public void Ctor_InvalidAmount_ThrowsArgumentException(int amount)
        {
            Assert.Throws<ArgumentException>(() => mOperationsFactory.CreateFinanceOperation(
                "26/01/1992", OperationType.Deposit, amount, new List<OperationKind> { OperationKind.Commission }, "reason"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_InvalidReason_ThrowsArgumentException(string reason)
        {
            Assert.Throws<ArgumentException>(() => mOperationsFactory.CreateFinanceOperation(
                "26/01/1992", OperationType.Deposit, 30, new List<OperationKind> { OperationKind.Commission }, reason));
        }
    }
}