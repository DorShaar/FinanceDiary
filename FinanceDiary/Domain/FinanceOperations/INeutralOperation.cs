using FinanceDiary.Domain.CashRegisters;
using System;

namespace FinanceDiary.Domain.FinanceOperations
{
    public interface INeutralOperation
    {
        string Id { get; }
        DateTime Date { get; }
        int Amount { get; }
        CashRegister SourceCashRegister { get; }
        CashRegister DestinationCashRegister { get; }
        string Reason { get; }
    }
}