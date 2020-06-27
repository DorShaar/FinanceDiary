﻿using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDiary.Domain.Database
{
    public interface IFinanceDiaryDatabase
    {
        Task SaveCashRegistersToCsv(HashSet<CashRegister> cashRegisters);
        Task SaveFinanceOperationsToCsv(IEnumerable<FinanceOperation> financeOperations);
        Task SaveNeutralOperationsToCsv(IEnumerable<NeutralOperation> neutralOperations);
        Task<List<CashRegister>> LoadCashRegistersFromCsv();
        Task<List<FinanceOperation>> LoadFinanceOperationsFromCsv();
        Task<List<NeutralOperation>> LoadNeutralOperationsFromCsv();
    }
}