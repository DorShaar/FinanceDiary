using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FinanceDiary.Domain.CashRegisters
{
    public class CashRegisterComparer : EqualityComparer<CashRegister>
    {
        public override bool Equals([AllowNull] CashRegister x, [AllowNull] CashRegister y)
        {
            if (x == null || y == null)
                return false;

            return x.Name.ToLower().Equals(y.Name.ToLower());
        }

        public override int GetHashCode([DisallowNull] CashRegister obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}