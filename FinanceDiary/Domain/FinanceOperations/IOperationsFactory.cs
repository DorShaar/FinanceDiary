namespace FinanceDiary.Domain.FinanceOperations
{
    public interface IOperationsFactory
    {
        FinanceOperation CreateFinanceOperation(
            string date,
            OperationType operationType,
            int amount,
            OperationKind operationKind,
            string reason);

        NeutralOperation CreateNeutralOperation(
            string date,
            int amount,
            string sourceCashRegister,
            string destinationCashRegister,
            string reason);
    }
}