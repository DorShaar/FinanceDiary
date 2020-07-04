using FinanceDiary.App;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.Database;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.Reports;
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

        private const string DefaultAccountName = "Default_Account";
        private CashRegister DefaultCacheRegister;
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
        }

        public IEnumerable<CashRegister> GetAllCashRegisters()
        {
            return mCashRegisters.ToImmutableHashSet();
        }

        public FinanceReport GetReport()
        {
            HashSet<CashRegister> cashRegisters = new HashSet<CashRegister>(mCashRegisters.Count, new CashRegisterComparer());

            foreach(CashRegister cashRegister in mCashRegisters)
            {
                cashRegisters.Add(cashRegister.CreateCopy());
            }

            return new FinanceReport(cashRegisters, mFinanceOperations, mNeutralOperations);
        }

        public bool AddCashRegister(string cachRegisterName, int initialAmount = 0)
        {
            if (!mCashRegisters.Add(new CashRegister(cachRegisterName, initialAmount)))
            {
                mLogger.LogDebug($"Cash register with name {cachRegisterName} is already exists");
                return false;
            }

            return true;
        }

        public bool AddFinanceOperation(string date, OperationType operationType, int amount,
            IEnumerable<OperationKind> operationKinds, string reason)
        {
            try
            {
                FinanceOperation financeOperation = mOperationsFactory.CreateFinanceOperation(
                    date, operationType, amount, operationKinds, reason);

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
            if (sourceCashRegisterName.ToLowerInvariant().Equals(destinationCashRegisterName.ToLowerInvariant()))
            {
                mLogger.LogWarning($"Destination and source cash registers are the same: {sourceCashRegisterName}");
                return false;
            }

            try
            {
                NeutralOperation neutralOperation = mOperationsFactory.CreateNeutralOperation(
                    date, amount, sourceCashRegisterName, destinationCashRegisterName, reason);

                if (!UpdateCashRegisters(neutralOperation))
                {
                    mLogger.LogWarning($"Failed adding finance operation");
                    return false;
                }

                mNeutralOperations.Add(neutralOperation);

                return true;
            }
            catch (ArgumentException ex)
            {
                mLogger.LogWarning(ex, $"Failed adding finance operation");
                return false;
            }
        }

        private bool UpdateCashRegisters(NeutralOperation neutralOperation)
        {
            if (!mCashRegisters.TryGetValue(new CashRegister(neutralOperation.SourceCashRegister),
                out CashRegister sourceCashRegister))
            {
                mLogger.LogWarning($"Cash register {neutralOperation.SourceCashRegister} does not exist");
                return false;
            }

            if (!mCashRegisters.TryGetValue(new CashRegister(neutralOperation.DestinationCashRegister),
                out CashRegister destinationCashRegister))
            {
                mLogger.LogWarning($"Cash register {neutralOperation.DestinationCashRegister} does not exist");
                return false;
            }

            mLogger.LogDebug($"Performing neutral operation id {neutralOperation.Id} of " +
                $"{neutralOperation.Amount} at {neutralOperation.Date} with reason {neutralOperation.Reason}" +
                $"from {neutralOperation.SourceCashRegister} to {neutralOperation.DestinationCashRegister}");

            sourceCashRegister.Withdraw(neutralOperation.Amount);
            destinationCashRegister.Deposit(neutralOperation.Amount);

            return true;
        }

        public async Task SaveToDatabase()
        {
            Task[] tasks = new Task[]
            {
                mFinanceDiaryDatabase.SaveCashRegistersToCsv(mCashRegisters),
                mFinanceDiaryDatabase.SaveFinanceOperationsToCsv(mFinanceOperations),
                mFinanceDiaryDatabase.SaveNeutralOperationsToCsv(mNeutralOperations),
                mOperationsFactory.SaveState()
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private Task LoadFromDatabase()
        {
            mFinanceOperations = mFinanceDiaryDatabase.LoadFinanceOperationsFromCsv().ToList();

            mNeutralOperations = mFinanceDiaryDatabase.LoadNeutralOperationsFromCsv().ToList();

            List<CashRegister> cashRegisters = mFinanceDiaryDatabase.LoadCashRegistersFromCsv().ToList();
            mCashRegisters = cashRegisters.ToHashSet(new CashRegisterComparer());

            DefaultCacheRegister = mCashRegisters.First(cash => cash.Name.Equals(DefaultAccountName));

            UpdateCashRegistersAfterLoad();

            return Task.CompletedTask;
        }

        private void UpdateCashRegistersAfterLoad()
        {
            foreach(FinanceOperation financeOperation in mFinanceOperations)
            {
                UpdateDefaultCashRegister(financeOperation);
            }

            foreach(NeutralOperation neutralOperation in mNeutralOperations)
            {
                UpdateCashRegisters(neutralOperation);
            }
        }
    }
}