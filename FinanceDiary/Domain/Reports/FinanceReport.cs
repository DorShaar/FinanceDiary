using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using System.Collections.Generic;
using System.Linq;

namespace FinanceDiary.Domain.Reports
{
    public class FinanceReport
    {
        public static FinanceReport NullFinanceReport = 
            new FinanceReport(new HashSet<CashRegister>(), new List<FinanceOperation>(), new List<NeutralOperation>());

        public HashSet<CashRegister> CashRegisters { get; }
        public IOrderedEnumerable<FinanceOperation> FinanceOperations { get; }
        public IOrderedEnumerable<NeutralOperation> NeutralOperations { get; }

        public FinanceReport(HashSet<CashRegister> cashRegister,
            IEnumerable<FinanceOperation> financeOperations,
            IEnumerable<NeutralOperation> neutralOperations)
        {
            CashRegisters = cashRegister;
            FinanceOperations = financeOperations.OrderBy(op => op.Date).ThenBy(op => op.Id);
            NeutralOperations = neutralOperations.OrderBy(op => op.Date).ThenBy(op => op.Id);
        }

        public FinanceReport FilterByMonth(int month)
        {
            if (month < 1 || month > 12)
                return NullFinanceReport;

            IEnumerable<FinanceOperation> filteredFinanceOperations = FinanceOperations.Where(op => op.Date.Month == month);
            IEnumerable<NeutralOperation> filteredNeutralOperations = NeutralOperations.Where(op => op.Date.Month == month);

            return new FinanceReport(CashRegisters, filteredFinanceOperations, filteredNeutralOperations);
        }
    }
}