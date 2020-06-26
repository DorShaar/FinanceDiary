using System;

namespace FinanceDiary.Domain.FinanceOperations
{
    public interface IFinanceOperation
    {
        string Id { get; }
        DateTime Date { get;}
        OperationType OperationType { get; }
        int Amount { get; }
        OperationKind OperationKind { get; }
        string Reason { get; }
    }
}