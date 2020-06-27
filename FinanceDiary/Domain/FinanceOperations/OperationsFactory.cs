using FinanceDiary.Domain.IdGenerators;
using System.Collections.Generic;

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
            IEnumerable<OperationKind> operationKinds, 
            string reason)
        {
            string id = mIdGenerator.GenerateId();
            return new FinanceOperation(id, date, operationType, amount, operationKinds, reason);
        }

        public NeutralOperation CreateNeutralOperation(
            string date, 
            int amount, 
            string sourceCashRegister, 
            string destinationCashRegister, 
            string reason)
        {
            string id = mIdGenerator.GenerateId();
            return new NeutralOperation(
                id, date, amount, sourceCashRegister, destinationCashRegister, reason);
        }
    }
}