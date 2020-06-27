﻿using System;
using System.Globalization;

namespace FinanceDiary.Domain.FinanceOperations
{
    public class FinanceOperation
    {
        public string Id { get; }
        public DateTime Date { get; private set; }
        public OperationType OperationType { get; private set; }
        public int Amount { get; private set; }
        public OperationKind OperationKind { get; private set; }
        public string Reason { get; private set; }

        internal FinanceOperation(
            string id,
            string date,
            OperationType operationType,
            int amount,
            OperationKind operationKind,
            string reason)
        {
            Id = id;
            ValidateAndSetDate(date);
            ValidateanSetOperationType(operationType);
            ValidateAndSetAmount(amount);
            ValidateanSetOperationKind(operationKind);
            ValidateAndSetReason(reason);
        }

        private void ValidateAndSetDate(string date)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
            {
                if (!DateTime.TryParseExact(
                    date, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateSecondTry))
                {
                    throw new ArgumentException(nameof(date));
                }

                parsedDate = parsedDateSecondTry;
            }

            Date = parsedDate;
        }

        private void ValidateanSetOperationType(OperationType operationType)
        {
            if (!Enum.IsDefined(typeof(OperationType), operationType))
            {
                throw new ArgumentException(nameof(operationType));
            }

            OperationType = operationType;
        }

        private void ValidateAndSetAmount(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException($"{nameof(amount)} must be positive number");

            Amount = amount;
        }

        private void ValidateanSetOperationKind(OperationKind operationKind)
        {
            if (!Enum.IsDefined(typeof(OperationKind), operationKind))
            {
                throw new ArgumentException(nameof(operationKind));
            }

            OperationKind = operationKind;
        }

        private void ValidateAndSetReason(string reason)
        {
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentException($"{nameof(reason)} is null or empty");

            Reason = reason.Trim('\"');
        }
    }
}