using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using System;
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

        public FinanceReport FilterByMonthAndYear(int month, int year)
        {
            if (month < 1 || month > 12)
                return NullFinanceReport;

            IEnumerable<FinanceOperation> filteredFinanceOperations = FinanceOperations.Where(op => op.Date.Month == month && op.Date.Year == year);
            IEnumerable<NeutralOperation> filteredNeutralOperations = NeutralOperations.Where(op => op.Date.Month == month && op.Date.Year == year);

            return new FinanceReport(CalculateCachRegisters(month, year), filteredFinanceOperations, filteredNeutralOperations);
        }

        private HashSet<CashRegister> CalculateCachRegisters(int month, int year)
        {
            DateTime dateTimeToCalculateFromUntilNow = new DateTime(year, month, 1).AddMonths(1);
            IEnumerable<FinanceOperation> filteredFinanceOperations = FinanceOperations.Where(op => op.Date > dateTimeToCalculateFromUntilNow);
            IEnumerable<NeutralOperation> filteredNeutralOperations = NeutralOperations.Where(op => op.Date > dateTimeToCalculateFromUntilNow);

            HashSet<CashRegister> filteredCashRegisters = new HashSet<CashRegister>(CashRegisters.Count, new CashRegisterComparer());
            foreach(CashRegister cashRegister in CashRegisters)
            {
                filteredCashRegisters.Add(cashRegister.CreateCopy());
            }

            filteredCashRegisters.TryGetValue(new CashRegister("Default_Account"), out CashRegister defaultCashRegister);

            foreach (FinanceOperation financeOperation in filteredFinanceOperations)
            {
                switch (financeOperation.OperationType)
                {
                    case OperationType.Deposit:
                        defaultCashRegister.Withdraw(financeOperation.Amount);
                        break;
                    case OperationType.Withdraw: 
                        defaultCashRegister.Deposit(financeOperation.Amount);
                        break;
                    default:
                        throw new InvalidOperationException($"There is no relevant operation for {financeOperation.OperationType}");
                }
            }

            foreach (NeutralOperation neutralOperation in filteredNeutralOperations)
            {
                filteredCashRegisters.TryGetValue(new CashRegister(neutralOperation.SourceCashRegister), out CashRegister sourceCashRegister);
                filteredCashRegisters.TryGetValue(new CashRegister(neutralOperation.DestinationCashRegister), out CashRegister destinationCashRegister);

                sourceCashRegister.Deposit(neutralOperation.Amount);
                destinationCashRegister.Withdraw(neutralOperation.Amount);
            }

            return filteredCashRegisters;
        }
    }
}