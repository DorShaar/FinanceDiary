using System.Collections.Generic;

namespace FinanceDiary.Domain.FinanceOperations
{
    public interface IOperationsFactory
    {
        FinanceOperation CreateFinanceOperation(
            string date,
            OperationType operationType,
            int amount,
            IEnumerable<OperationKind> operationKinds,
            string reason);

        NeutralOperation CreateNeutralOperation(
            string date,
            int amount,
            string sourceCashRegister,
            string destinationCashRegister,
            string reason);
    }
}