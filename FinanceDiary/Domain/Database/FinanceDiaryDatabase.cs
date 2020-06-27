using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.Database.CsvHelper;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Infra.Options;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
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

            using CsvWriterAdapter csvWriterAdapter = new CsvWriterAdapter(csvPath);

            await csvWriterAdapter.Write(cashRegisters);
        }

        public async Task SaveFinanceOperationsToCsv(IEnumerable<FinanceOperation> financeOperations)
        {
            string csvPath = Path.Combine(DatabasePath, FinanceOperationsCsv);

            using CsvWriterAdapter csvWriterAdapter = new CsvWriterAdapter(csvPath);

            await csvWriterAdapter.Write(financeOperations);
        }

        public async Task SaveNeutralOperationsToCsv(IEnumerable<NeutralOperation> neutralOperations)
        {
            string csvPath = Path.Combine(DatabasePath, NeutralOperationsCsv);

            using CsvWriterAdapter csvWriterAdapter = new CsvWriterAdapter(csvPath);

            await csvWriterAdapter.Write(neutralOperations);
        }

        public List<CashRegister> LoadCashRegistersFromCsv()
        {
            string csvPath = Path.Combine(DatabasePath, CashRegisterCsv);

            using CsvReaderAdapter csvReaderAdapter = new CsvReaderAdapter(csvPath);

            List<CashRegister> cashRegisters = new List<CashRegister>();
            csvReaderAdapter.CsvReader.Read();
            csvReaderAdapter.CsvReader.ReadHeader();
            while (csvReaderAdapter.CsvReader.Read())
            {
                CashRegister cashRegister = new CashRegister(
                    csvReaderAdapter.CsvReader.GetField("Name"), 
                    csvReaderAdapter.CsvReader.GetField<int>("CurrentAmount"));
                cashRegisters.Add(cashRegister);
            }

            return cashRegisters;
        }

        public List<FinanceOperation> LoadFinanceOperationsFromCsv()
        {
            string csvPath = Path.Combine(DatabasePath, FinanceOperationsCsv);

            using CsvReaderAdapter csvReaderAdapter = new CsvReaderAdapter(csvPath);

            List<FinanceOperation> financeOperations = new List<FinanceOperation>();
            csvReaderAdapter.CsvReader.Read();
            csvReaderAdapter.CsvReader.ReadHeader();
            while (csvReaderAdapter.CsvReader.Read())
            {
                FinanceOperation financeOperation = new FinanceOperation(
                    csvReaderAdapter.CsvReader.GetField("Id"), 
                    csvReaderAdapter.CsvReader.GetField("Date"),
                    csvReaderAdapter.CsvReader.GetField<OperationType>("OperationType"),
                    csvReaderAdapter.CsvReader.GetField<int>("Amount"),
                    csvReaderAdapter.CsvReader.GetField<OperationKind>("OperationKind"),
                    csvReaderAdapter.CsvReader.GetField("Reason"));

                financeOperations.Add(financeOperation);
            }

            return financeOperations;
        }

        public List<NeutralOperation> LoadNeutralOperationsFromCsv()
        {
            string csvPath = Path.Combine(DatabasePath, NeutralOperationsCsv);

            using CsvReaderAdapter csvReaderAdapter = new CsvReaderAdapter(csvPath);

            List<NeutralOperation> neutralOperations = new List<NeutralOperation>();
            csvReaderAdapter.CsvReader.Read();
            csvReaderAdapter.CsvReader.ReadHeader();
            while (csvReaderAdapter.CsvReader.Read())
            {
                NeutralOperation neutralOperation = new NeutralOperation(
                    csvReaderAdapter.CsvReader.GetField("Id"),
                    csvReaderAdapter.CsvReader.GetField("Date"),
                    csvReaderAdapter.CsvReader.GetField<int>("Amount"),
                    csvReaderAdapter.CsvReader.GetField("SourceCashRegister"),
                    csvReaderAdapter.CsvReader.GetField("DestinationCashRegister"),
                    csvReaderAdapter.CsvReader.GetField("Reason"));

                neutralOperations.Add(neutralOperation);
            }

            return neutralOperations;
        }
    }
}