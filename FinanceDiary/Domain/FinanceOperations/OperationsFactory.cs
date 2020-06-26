using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.IdGenerators;

namespace FinanceDiary.Domain.FinanceOperations
{
    public class OperationsFactory : IOperationsFactory
    {
        private readonly IIdGenerator mIdGenerator;

        public OperationsFactory(IIdGenerator idGenerator)
        {
            mIdGenerator = idGenerator;
        }

        public FinanceOperation CreateFinanceOperation(
            string date, 
            OperationType operationType, 
            int amount, 
            OperationKind operationKind, 
            string reason)
        {
            string id = mIdGenerator.GenerateId();
            return new FinanceOperation(id, date, operationType, amount, operationKind, reason);
        }

        public NeutralOperation CreateNeutralOperation(
            string date, 
            int amount, 
            CashRegister sourceCashRegister, 
            CashRegister destinationCashRegister, 
            string reason)
        {
            string id = mIdGenerator.GenerateId();
            return new NeutralOperation(
                id, date, amount, sourceCashRegister, destinationCashRegister, reason);
        }
    }
}