using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.IdGenerator;
using System;

namespace FinanceDiary.Domain.FinanceOperations
{
    public class NeutralOperation : INeutralOperation
    {
        private static IIdGenerator mIdGenerator;

        public string Id { get; }
        public DateTime Date { get; private set; }
        public int Amount { get; private set; }
        public CashRegister SourceCashRegister { get; private set; }
        public CashRegister DestinationCashRegister { get; private set; }
        public string Reason { get; private set; }

        public NeutralOperation(IIdGenerator idGenerator)
        {
            mIdGenerator = idGenerator;
        }

        private NeutralOperation(
            string date,
            int amount,
            CashRegister sourceCashRegister,
            CashRegister destinationCashRegister,
            string reason)
        {
            //Id = mIdGenerator.GenerateId();
            ValidateAndSetDate(date);
            ValidateAndSetAmount(amount);
            ValidateAndSetCashRegisters(sourceCashRegister, destinationCashRegister);
            ValidateAndSetReason(reason);
        }

        public static NeutralOperation Create(
            string date,
            int amount,
            CashRegister sourceCashRegister,
            CashRegister destinationCashRegister,
            string reason)
        {
            return new NeutralOperation(date, amount, sourceCashRegister, destinationCashRegister, reason);
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

        private void ValidateAndSetCashRegisters(CashRegister sourceCashRegister,
            CashRegister destinationCashRegister)
        {
            if (sourceCashRegister.Name.Equals(destinationCashRegister.Name))
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