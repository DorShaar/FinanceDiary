using FinanceDiary.Domain.CashRegisters;

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
            CashRegister sourceCashRegister,
            CashRegister destinationCashRegister,
            string reason);
    }
}