using ConsoleTables;
using FinanceDiary.Domain.CashRegisters;
using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.Reports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FinanceDiary.Domain.ConsoleTableAdapters
{
    internal class ConsoleTableAdapter
    {
        private static readonly DateTime MinimalDateTime = DateTime.Parse("01/06/2019");

        public static void PrintCashRegisterStatus(IEnumerable<CashRegister> cashRegisters)
        {
            string[] columnsHeaders = cashRegisters.Select(cash => cash.Name).Append("Total").ToArray();
            ConsoleTable consoleTable = new ConsoleTable(columnsHeaders);

            int[] amounts = cashRegisters.Select(cash => cash.CurrentAmount).ToArray();
            object[] amountsObject = new object[columnsHeaders.Length];

            int total = 0;
            for (int i = 0; i < amounts.Length; ++i)
            {
                total += amounts[i];
                amountsObject[i] = amounts[i];
            }

            amountsObject[columnsHeaders.Length - 1] = total;

            consoleTable.AddRow(amountsObject);

            consoleTable.Write(Format.Alternative);
        }

        public static void PrintReportSince(FinanceReport report, DateTime sinceTime)
        {
            if (sinceTime < MinimalDateTime)
                sinceTime = MinimalDateTime;

            while (sinceTime < DateTime.Now)
            {
                PrintMonthlyReport(report, sinceTime.Month, sinceTime.Year);
                sinceTime = sinceTime.AddMonths(1);
            }
        }

        private static void PrintMonthlyReport(FinanceReport report, int month, int year)
        {
            FinanceReport monthlyReport = report.FilterByMonthAndYear(month, year);
            string[] columnsHeaders = BuildMonthlyReportHeaders(monthlyReport.CashRegisters);

            ConsoleTable consoleTable = new ConsoleTable(columnsHeaders);

            foreach (FinanceOperation financeOperation in monthlyReport.FinanceOperations)
            {
                object[] rowObjects = BuildRow(
                    columnsHeaders.Length, 
                    financeOperation.Date.ToShortDateString(), 
                    financeOperation.Reason, 
                    GetFinanceOperationSign(financeOperation.OperationType), 
                    financeOperation.Amount);

                consoleTable.AddRow(rowObjects);
            }

            foreach (NeutralOperation neutralOperation in monthlyReport.NeutralOperations)
            {
                object[] rowObjects = BuildRow(
                    columnsHeaders.Length,
                    neutralOperation.Date.ToShortDateString(), 
                    neutralOperation.Reason, 
                    'N', 
                    neutralOperation.Amount);

                consoleTable.AddRow(rowObjects);
            }

            int[] amounts = monthlyReport.CashRegisters.Select(cash => cash.CurrentAmount).ToArray();
            int total = CalculateTotal(amounts);
            object[] summaryRowObjects = BuildRow(columnsHeaders.Length, ' ', ' ', ' ', ' ', amounts, total);
            consoleTable.AddRow(summaryRowObjects);

            consoleTable.Write(Format.Alternative);
        }

        private static string[] BuildMonthlyReportHeaders(HashSet<CashRegister> cashRegisters)
        {
            List<string> columnsHeadersList = new List<string> { "Date", "Reason", "Type", "Amount" };
            columnsHeadersList.AddRange(cashRegisters.Select(cash => cash.Name));
            return columnsHeadersList.Append("Total").ToArray();
        }

        private static char GetFinanceOperationSign(OperationType operationType)
        {
            return operationType switch
            {
                OperationType.Deposit => '+',
                OperationType.Withdraw => '-',
                _ => '?',
            };
        }

        private static int CalculateTotal(int[] amounts)
        {
            int total = 0;
            for (int i = 0; i < amounts.Length; ++i)
            {
                total += amounts[i];
            }

            return total;
        }

        private static object[] BuildRow(int rowSize, params object[] objects)
        {
            int index = 0;
            object[] rowObjects = new object[rowSize];

            foreach (object obj in objects)
            {
                if (IsCollectionType(obj.GetType()))
                {
                    ICollection collection = obj as ICollection;
                    BuildRow(rowObjects, collection, index);
                    index += collection.Count;
                }
                else
                {
                    rowObjects[index] = obj;
                    index++;
                }
            }

            while (index < rowSize)
            {
                rowObjects[index] = ' ';
                index++;
            }

            return rowObjects;
        }

        private static bool IsCollectionType(Type type)
        {
            return type.GetInterface(nameof(ICollection)) != null;
        }

        private static void BuildRow(object[] row, ICollection collection, int index)
        {
            foreach(object obj in collection)
            {
                row[index] = obj;
                index++;
            }
        }
    }
}