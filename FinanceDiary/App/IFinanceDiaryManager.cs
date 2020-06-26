using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDiary.App
{
    public interface IFinanceDiaryManager
    {
        IEnumerable<CashRegister> GetAllCashRegisters();
        bool AddCashRegister(string cachRegisterName);
        bool AddFinanceOperation(string date, OperationType operationType, int amount,
            OperationKind operationKind, string reason);
        bool AddNeutralOperation(string date, int amount, string sourceCashRegisterName,
            string destinationCashRegisterName, string reason);
        Task SaveToCsv(string csvPath);
    }
}