using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using System;
using Xunit;

namespace FinanceDiary.TestsUnit.Domain.FinanceOperations
{
    public class NeutralOperationTests
    {
        [Fact]
        public void Ctor_ValidParametersDateFormat_CorrectParametrs()
        {
            int amount = 30;
            CashRegister cashRegister1 = new CashRegister("cach_register1");
            CashRegister cashRegister2 = new CashRegister("cach_register2");
            string reason = "reason";

            NeutralOperation neutralOperation = NeutralOperation.Create("20/01/1992",
                30,
                cashRegister1,
                cashRegister2,
                reason);

            Assert.Equal(20, neutralOperation.Date.Day);
            Assert.Equal(1, neutralOperation.Date.Month);
            Assert.Equal(1992, neutralOperation.Date.Year);

            Assert.Equal(amount, neutralOperation.Amount);
            Assert.Equal(cashRegister1, neutralOperation.SourceCashRegister);
            Assert.Equal(cashRegister2, neutralOperation.DestinationCashRegister);
            Assert.Equal(reason, neutralOperation.Reason);
        }

        [Fact]
        public void Ctor_InvalidDate_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => NeutralOperation.Create(
                "32/01/1992", 30, new CashRegister("cach_register1"), new CashRegister("cach_register2"), "reason"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-20)]
        public void Ctor_InvalidAmount_ThrowsArgumentException(int amount)
        {
            Assert.Throws<ArgumentException>(() => NeutralOperation.Create(
                "26/01/1992", amount, new CashRegister("cach_register1"), new CashRegister("cach_register2"), "reason"));
        }

        [Fact]
        public void Ctor_InvalidSourceAndDestinationCachRegisters_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => NeutralOperation.Create(
                 "26/01/1992", 30, new CashRegister("cach_register1"), new CashRegister("cach_register1"), "reason"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_InvalidReason_ThrowsArgumentException(string reason)
        {
            Assert.Throws<ArgumentException>(() => NeutralOperation.Create(
                 "26/01/1992", 30, new CashRegister("cach_register1"), new CashRegister("cach_register2"), reason));
        }
    }
}