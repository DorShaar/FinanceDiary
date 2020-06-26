using FinanceDiary.Domain.CashRegisters;
using System;
using Xunit;

namespace FinanceDiary.TestsUnit.Domain.CashRegisters
{
    public class CashRegisterTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_InvalidName_ThrowArgumentException(string name)
        {
            Assert.Throws<ArgumentException>(() => new CashRegister(name));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void Deposit_InvalidAmount_ThrowArgumentException(int amount)
        {
            CashRegister cashRegister = new CashRegister("cash_register");
            Assert.Throws<ArgumentException>(() => cashRegister.Deposit(amount));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void Withdraw_InvalidAmount_ThrowArgumentException(int amount)
        {
            CashRegister cashRegister = new CashRegister("cash_register");
            Assert.Throws<ArgumentException>(() => cashRegister.Withdraw(amount));
        }

        [Fact]
        public void CurrentAmount_AsExpected()
        {
            CashRegister cashRegister = new CashRegister("cash_register");
            Assert.Equal(0, cashRegister.CurrentAmount);

            cashRegister.Withdraw(100);
            Assert.Equal(-100, cashRegister.CurrentAmount);

            cashRegister.Deposit(500);
            Assert.Equal(400, cashRegister.CurrentAmount);
        }
    }
}