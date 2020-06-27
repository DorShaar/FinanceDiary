using FakeItEasy;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerators;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FinanceDiary.TestsUnit.Domain.FinanceOperations
{
    public class OperationsFactoryTests
    {
        [Fact]
        public void CreateFinanceOperation_CreatedAsExpected()
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            OperationsFactory operationsFactory = new OperationsFactory(idGenerator);

            FinanceOperation financeOperation = operationsFactory.CreateFinanceOperation(
                "24/06/2018", OperationType.Deposit, 200, new List<OperationKind> { OperationKind.BankWithdrawal }, "reason");

            A.CallTo(() => idGenerator.GenerateId()).MustHaveHappenedOnceExactly();
            financeOperation.OperationType.Should().Be(OperationType.Deposit);
            financeOperation.Amount.Should().Be(200);
        }

        [Fact]
        public void CreateNeutralOperation_CreatedAsExpected()
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            OperationsFactory operationsFactory = new OperationsFactory(idGenerator);

            NeutralOperation neutralOperation = operationsFactory.CreateNeutralOperation(
                "24/06/2018", 200, "cash1", "cash2", "reason");

            A.CallTo(() => idGenerator.GenerateId()).MustHaveHappenedOnceExactly();
            neutralOperation.Amount.Should().Be(200);
        }

        [Fact]
        public async Task SaveState_IdGeneratorSaveStateMustBeCalled()
        {
            IIdGenerator idGenerator = A.Fake<IIdGenerator>();
            OperationsFactory operationsFactory = new OperationsFactory(idGenerator);

            await operationsFactory.SaveState().ConfigureAwait(false);

            A.CallTo(() => idGenerator.SaveState()).MustHaveHappenedOnceExactly();
        }
    }
}