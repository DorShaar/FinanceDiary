using System;

namespace FinanceDiary.Domain.CashRegisters
{
    public class CashRegister
    {
        public string Name { get; }
        public int InitialAmount { get; }
        public int CurrentAmount { get; private set; }

        public CashRegister(string name, int initialAmount = 0)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"{nameof(name)}");

            Name = name;
            InitialAmount = initialAmount;
            CurrentAmount = initialAmount;
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