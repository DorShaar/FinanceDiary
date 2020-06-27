using CsvHelper;
using CsvHelper.Configuration;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Infra.Options;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace FinanceDiary.Domain.Database
{
    public class FinanceDiaryDatabase : IFinanceDiaryDatabase
    {
        private const string CashRegisterCsv = "cash_register.csv";
        private const string FinanceOperationsCsv = "finance_operations.csv";
        private const string NeutralOperationsCsv = "neutral_operations.csv";
        private readonly string DatabasePath;

        public FinanceDiaryDatabase(IOptionsMonitor<DatabaseConfiguration> configuration)
        {
            DatabasePath = configuration.CurrentValue.DatabasePath;
        }

        public async Task SaveCashRegistersToCsv(HashSet<CashRegister> cashRegisters)
        {
            string csvPath = Path.Combine(DatabasePath, CashRegisterCsv);

            using StreamWriter writer = new StreamWriter(csvPath);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            using CsvWriter csvWriter = new CsvWriter(writer, csvConfiguration);
            
            await csvWriter.WriteRecordsAsync(cashRegisters);
        }

        public async Task SaveFinanceOperationsToCsv(IEnumerable<FinanceOperation> financeOperations)
        {
            string csvPath = Path.Combine(DatabasePath, FinanceOperationsCsv);

            using StreamWriter writer = new StreamWriter(csvPath);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            using CsvWriter csvWriter = new CsvWriter(writer, csvConfiguration);
            
            await csvWriter.WriteRecordsAsync(financeOperations);
        }

        public async Task SaveNeutralOperationsToCsv(IEnumerable<NeutralOperation> neutralOperations)
        {
            string csvPath = Path.Combine(DatabasePath, NeutralOperationsCsv);

            using StreamWriter writer = new StreamWriter(csvPath);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            using CsvWriter csvWriter = new CsvWriter(writer, csvConfiguration);

            await csvWriter.WriteRecordsAsync(neutralOperations);
        }

        public Task<List<CashRegister>> LoadCashRegistersFromCsv()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<FinanceOperation>> LoadFinanceOperationsFromCsv()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<NeutralOperation>> LoadNeutralOperationsFromCsv()
        {
            throw new System.NotImplementedException();
        }
    }
}