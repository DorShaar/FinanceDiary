using CsvHelper;
using CsvHelper.Configuration;
using FinanceDiary.App;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinacneOperations;
using FinanceDiary.Domain.FinanceOperations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace FinanceDiary.Infra
{
    public class FinanceDiaryManager : IFinanceDiaryManager
    {
        private readonly ILogger<FinanceDiaryManager> mLogger;

        private const string DefaultAccountName = "Default Account";
        private readonly CashRegister DefaultCacheRegister = new CashRegister(DefaultAccountName);
        private readonly HashSet<CashRegister> mCashRegisters;
        private readonly List<FinanceOperation> mFinanceOperations = new List<FinanceOperation>();
        private readonly List<NeutralOperation> mNeutralOperations = new List<NeutralOperation>();

        public FinanceDiaryManager(ILogger<FinanceDiaryManager> logger)
        {
            mCashRegisters = new HashSet<CashRegister>(new CashRegisterComparer())
            {
                DefaultCacheRegister
            };

            mLogger = logger;
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
                FinanceOperation financeOperation =
                    FinanceOperation.Create(date, operationType, amount, operationKind, reason);

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
                NeutralOperation neutralOperation = NeutralOperation.Create(
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

        public async Task SaveToCsv(string csvPath)
        {
            using StreamWriter writer = new StreamWriter(csvPath);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            using CsvWriter csvWriter = new CsvWriter(writer, csvConfiguration);
            await csvWriter.WriteRecordsAsync(mFinanceOperations);
        }
    }
}