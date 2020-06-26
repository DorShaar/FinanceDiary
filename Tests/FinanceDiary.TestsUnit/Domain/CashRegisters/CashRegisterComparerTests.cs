using FinanceDiary.Domain.CashRegisters;
using Xunit;

namespace FinanceDiary.TestsUnit.Domain.CashRegisters
{
    public class CashRegisterComparerTests
    {
        [Theory]
        [InlineData(null, null, false)]
        [InlineData(null, "cash2", false)]
        [InlineData("cash1", null, false)]
        [InlineData("cash1", "cach2", false)]
        [InlineData("cash1", "cash1", true)]
        public void Equals_AsExpected(string cache1Name, string cache2Name, bool shouldBeEqual)
        {
            CashRegisterComparer cashRegisterComparer = new CashRegisterComparer();

            CashRegister cashRegister1 = null;
            CashRegister cashRegister2 = null;

            if (cache1Name != null)
                cashRegister1 = new CashRegister(cache1Name);

            if (cache2Name != null)
                cashRegister2 = new CashRegister(cache2Name);

            Assert.Equal(shouldBeEqual, cashRegisterComparer.Equals(cashRegister1, cashRegister2));
        }
    }
}