﻿using FinanceDiary.Domain.FinanceOperations;
using FinanceDiary.Domain.IdGenerator;
using System;

namespace FinanceDiary.Domain.FinacneOperations
{
    public class FinanceOperation : IFinanceOperation
    {
        private static IIdGenerator mIdGenerator;

        public string Id { get; }
        public DateTime Date { get; private set; }
        public OperationType OperationType { get; private set; }
        public int Amount { get; private set; }
        public OperationKind OperationKind { get; private set; }
        public string Reason { get; private set; }

        internal FinanceOperation(IIdGenerator idGenerator)
        {
            mIdGenerator = idGenerator;
        }

        private FinanceOperation(
            string date,
            OperationType operationType,
            int amount,
            OperationKind operationKind,
            string reason)
        {
            //Id = mIdGenerator.GenerateId();
            ValidateAndSetDate(date);
            OperationType = operationType;
            ValidateAndSetAmount(amount);
            OperationKind = operationKind;
            ValidateAndSetReason(reason);
        }

        public static FinanceOperation Create(
            string date,
            OperationType operationType, 
            int amount, 
            OperationKind operationKind,
            string reason)
        {
            return new FinanceOperation(date, operationType, amount, operationKind, reason);
        }

        private void ValidateAndSetDate(string date)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
                throw new ArgumentException(nameof(date));

            Date = parsedDate;
        }

        private void ValidateAndSetAmount(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException($"{nameof(amount)} must be positive number");

            Amount = amount;
        }

        private void ValidateAndSetReason(string reason)
        {
            if (string.IsNullOrEmpty(reason))
                throw new ArgumentException($"{nameof(reason)} is null or empty");

            Reason = reason;
        }
    }
}