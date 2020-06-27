using System;

namespace FinanceDiary.Domain.FinanceOperations
{
    public class NeutralOperation
    {
        public string Id { get; }
        public DateTime Date { get; private set; }
        public int Amount { get; private set; }
        public string SourceCashRegister { get; private set; }
        public string DestinationCashRegister { get; private set; }
        public string Reason { get; private set; }

        internal NeutralOperation(
            string id,
            string date,
            int amount,
            string sourceCashRegister,
            string destinationCashRegister,
            string reason)
        {
            Id = id;
            ValidateAndSetDate(date);
            ValidateAndSetAmount(amount);
            ValidateAndSetCashRegisters(sourceCashRegister, destinationCashRegister);
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

        private void ValidateAndSetCashRegisters(string sourceCashRegister, string destinationCashRegister)
        {
            if (sourceCashRegister.ToLowerInvariant().Equals(destinationCashRegister.ToLowerInvariant()))
                throw new ArgumentException($"{nameof(sourceCashRegister)} is equal to {nameof(destinationCashRegister)}");

            SourceCashRegister = sourceCashRegister;
            DestinationCashRegister = destinationCashRegister;
        }

        private void ValidateAndSetReason(string reason)
        {
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentException($"{nameof(reason)} is null or empty");

            Reason = reason;
        }
    }
}