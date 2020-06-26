using FakeItEasy;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerators;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        [Fact]
        public void Ctor_ValidParametersDateFormat_CorrectParametrs()
        {
            OperationType operationType = OperationType.Deposit;
            int amount = 30;
            OperationKind operationKind = OperationKind.Family;
            string reason = "reason";

            FinanceOperation financeOperation = mOperationsFactory.CreateFinanceOperation("20/01/1992",
                operationType,
                amount,
                operationKind,
                reason);

            Assert.Equal(20, financeOperation.Date.Day);
            Assert.Equal(1, financeOperation.Date.Month);
            Assert.Equal(1992, financeOperation.Date.Year);

            Assert.Equal(operationType, financeOperation.OperationType);
            Assert.Equal(amount, financeOperation.Amount);
            Assert.Equal(operationKind, financeOperation.OperationKind);
            Assert.Equal(reason, financeOperation.Reason);
        }

        [Fact]
        public void Ctor_InvalidDate_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => mOperationsFactory.CreateFinanceOperation(
                "32/01/1992", OperationType.Deposit, 30, OperationKind.Commission, "reason"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        public void Ctor_InvalidAmount_ThrowsArgumentException(int amount)
        {
            Assert.Throws<ArgumentException>(() => mOperationsFactory.CreateFinanceOperation(
                "26/01/1992", OperationType.Deposit, amount, OperationKind.Commission, "reason"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_InvalidReason_ThrowsArgumentException(string reason)
        {
            Assert.Throws<ArgumentException>(() => mOperationsFactory.CreateFinanceOperation(
                "26/01/1992", OperationType.Deposit, 30, OperationKind.Commission, reason));
        }
    }
}