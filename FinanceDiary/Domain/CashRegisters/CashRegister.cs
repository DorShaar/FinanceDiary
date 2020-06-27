using System;

namespace FinanceDiary.Domain.CashRegisters
{
    public class CashRegister
    {
        public string Name { get; }
        public int CurrentAmount { get; private set; }

        public CashRegister(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"{nameof(name)}");

            Name = name;
        }

        internal CashRegister(string name, int currentAmount)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"{nameof(name)}");

            Name = name;
            CurrentAmount = currentAmount;
        }

        public void Deposit(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException($"{nameof(amount)} must be positive number");

            CurrentAmount += amount;
        }

        public void Withdraw(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException($"{nameof(amount)} must be positive number");

            CurrentAmount -= amount;
        }
    }
}