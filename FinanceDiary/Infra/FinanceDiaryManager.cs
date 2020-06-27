using FinanceDiary.App;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.Database;
using FinanceDiary.Domain.FinanceOperations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceDiary.Infra
{
    public class FinanceDiaryManager : IFinanceDiaryManager
    {
        private readonly IOperationsFactory mOperationsFactory;
        private readonly IFinanceDiaryDatabase mFinanceDiaryDatabase;
        private readonly ILogger<FinanceDiaryManager> mLogger;

        private const string DefaultAccountName = "Default Account";
        private readonly CashRegister DefaultCacheRegister;
        private HashSet<CashRegister> mCashRegisters;
        private List<FinanceOperation> mFinanceOperations;
        private List<NeutralOperation> mNeutralOperations;

        public FinanceDiaryManager(
            IOperationsFactory operationsFactory,
            IFinanceDiaryDatabase financeDiaryDatabase,
            ILogger<FinanceDiaryManager> logger)
        {
            mOperationsFactory = operationsFactory;
            mFinanceDiaryDatabase = financeDiaryDatabase;
            mLogger = logger;

            LoadFromDatabase().Wait();
            DefaultCacheRegister = mCashRegisters.First(cash => cash.Name.Equals(DefaultAccountName));
        }

        public bool AddCashRegister(string cachRegisterName)
        {
            if (!mCashRegisters.Add(new CashRegister(cachRegisterName)))
            {
                mLogger.LogDebug($"Cash register with name {cachRegisterName} is already exists");
                return false;
            }

            return true;
        }

        public bool AddFinanceOperation(string date, OperationType operationType, int amount,
            OperationKind operationKind, string reason)
        {
            try
            {
                FinanceOperation financeOperation = mOperationsFactory.CreateFinanceOperation(
                    date, operationType, amount, operationKind, reason);

                UpdateDefaultCashRegister(financeOperation);

                mFinanceOperations.Add(financeOperation);

                return true;
            }
            catch (ArgumentException ex)
            {
                mLogger.LogWarning(ex, $"Failed in adding finance operation");
                return false;
            }
        }

        private void UpdateDefaultCashRegister(FinanceOperation financeOperation)
        {
            mLogger.LogDebug($"Performing {financeOperation.OperationType} operation id {financeOperation.Id} of " +
                $"{financeOperation.Amount} at {financeOperation.Date} with reason {financeOperation.Reason}");

            if (financeOperation.OperationType == OperationType.Deposit)
            {
                DefaultCacheRegister.Deposit(financeOperation.Amount);
                return;
            }

            if (financeOperation.OperationType == OperationType.Withdraw)
            {
                DefaultCacheRegister.Withdraw(financeOperation.Amount);
                return;
            }
        }

        public bool AddNeutralOperation(string date, int amount, string sourceCashRegisterName,
            string destinationCashRegisterName, string reason)
        {
            if (!mCashRegisters.TryGetValue(new CashRegister(sourceCashRegisterName),
                out CashRegister sourceCashRegister))
            {
                mLogger.LogWarning($"Cash register {sourceCashRegisterName} does not exist");
                return false;
            }

            if (!mCashRegisters.TryGetValue(new CashRegister(destinationCashRegisterName),
                out CashRegister destinationCashRegister))
            {
                mLogger.LogWarning($"Cash register {destinationCashRegisterName} does not exist");
                return false;
            }

            if (sourceCashRegister == destinationCashRegister)
            {
                mLogger.LogWarning($"Destination and source cash registers are the same: {destinationCashRegister.Name}");
                return false;
            }

            try
            {
                NeutralOperation neutralOperation = mOperationsFactory.CreateNeutralOperation(
                    date, amount, sourceCashRegister, destinationCashRegister, reason);

                UpdateCashRegisters(neutralOperation);

                mNeutralOperations.Add(neutralOperation);

                return true;
            }
            catch (ArgumentException ex)
            {
                mLogger.LogWarning(ex, $"Failed in adding finance operation");
                return false;
            }
        }

        private void UpdateCashRegisters(NeutralOperation neutralOperation)
        {
            mLogger.LogDebug($"Performing neutral operation id {neutralOperation.Id} of " +
                $"{neutralOperation.Amount} at {neutralOperation.Date} with reason {neutralOperation.Reason}" +
                $"from {neutralOperation.SourceCashRegister.Name} to {neutralOperation.DestinationCashRegister.Name}");

            neutralOperation.SourceCashRegister.Withdraw(neutralOperation.Amount);
            neutralOperation.DestinationCashRegister.Deposit(neutralOperation.Amount);
        }

        public IEnumerable<CashRegister> GetAllCashRegisters()
        {
            return mCashRegisters.ToImmutableArray();
        }

        public async Task SaveToDatabase()
        {
            Task[] tasks = new Task[]
            {
                mFinanceDiaryDatabase.SaveCashRegistersToCsv(mCashRegisters),
                mFinanceDiaryDatabase.SaveFinanceOperationsToCsv(mFinanceOperations),
                mFinanceDiaryDatabase.SaveNeutralOperationsToCsv(mNeutralOperations)
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task LoadFromDatabase()
        {
            List<CashRegister> cashRegisters = await mFinanceDiaryDatabase.LoadCashRegistersFromCsv();
            mCashRegisters = cashRegisters.ToHashSet(new CashRegisterComparer());

            mFinanceOperations = await mFinanceDiaryDatabase.LoadFinanceOperationsFromCsv();

            mNeutralOperations = await mFinanceDiaryDatabase.LoadNeutralOperationsFromCsv();
        }
    }
}