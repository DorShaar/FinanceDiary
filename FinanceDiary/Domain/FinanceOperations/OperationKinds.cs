using System;

namespace FinanceDiary.Domain.FinanceOperations
{
    public class OperationKinds
    {
        public OperationKind OperationKind1 { get; set; }
        public OperationKind OperationKind2 { get; set; }
        public OperationKind OperationKind3 { get; set; }

        public OperationKinds(params OperationKind[] operationKinds)
        {
            if (operationKinds.Length == 0)
                throw new ArgumentException(nameof(operationKinds));

            if (operationKinds.Length == 1)
            {
                OperationKind1 = operationKinds[0];
                return;
            }

            OperationKind1 = operationKinds[0];

            if (operationKinds.Length == 2)
            {
                OperationKind2 = operationKinds[1];
                return;
            }

            OperationKind2 = operationKinds[1];
            OperationKind3 = operationKinds[2];
        }
    }
}