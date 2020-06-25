using FinanceDiary.Domain.FinanceOperations;
using System;

namespace FinanceDiary.Domain.FinacneOperations
{
    public class FinanceOperation
    {
        public DateTime Date { get; private set; }
        public OperationType OperationType { get; private set; }
        public int Amount { get; private set; }
        public OperationKind OperationKind { get; }
        public string Reason { get; private set; }

        public FinanceOperation(
            string date,
            OperationType operationType, 
            int amount, 
            OperationKind operationKind,
            string reason)
        {
            ValidateAndSetDate(date);
            OperationType = operationType;
            ValidateAndSetAmount(amount);
            OperationKind = operationKind;
            ValidateAndSetReason(reason);
        }

        private void ValidateAndSetDate(string date)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
                throw new ArgumentException(nameof(date));

            Date = parsedDate;
        }

        private void ValidateAndSetAmount(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException($"{nameof(amount)} must be positive number");

            Amount = amount;
        }

        private void ValidateAndSetReason(string reason)
        {
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentException($"{nameof(reason)} is null or empty");

            Reason = reason;
        }
    }
}