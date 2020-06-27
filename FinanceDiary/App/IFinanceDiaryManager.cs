using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceDiary.App
{
    public interface IFinanceDiaryManager
    {
        IEnumerable<CashRegister> GetAllCashRegisters();
        FinanceReport GetReport();

        bool AddCashRegister(string cachRegisterName, int initialAmount = 0);
        bool AddFinanceOperation(string date, OperationType operationType, int amount,
            IEnumerable<OperationKind> operationKinds, string reason);
        bool AddNeutralOperation(string date, int amount, string sourceCashRegisterName,
            string destinationCashRegisterName, string reason);

        Task SaveToDatabase();
    }
}