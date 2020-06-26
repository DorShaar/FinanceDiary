using FinanceDiary.Infra.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace FinanceDiary.Domain.IdGenerators
{
    public class IdGenerator : IIdGenerator
    {
        private const string NextIdHolderName = "id_producer.db";

        private static int mLastId = 0;
        private readonly ILogger<IdGenerator> mLogger;

        public IdGenerator(IOptionsMonitor<DatabaseConfiguration> configuration, ILogger<IdGenerator> logger)
        {
            mLogger = logger;

            string idHolderPath = 
                Path.Combine(Path.GetDirectoryName(configuration.CurrentValue.CsvPath), NextIdHolderName);

            string idString = File.ReadAllText(idHolderPath);
            if (!int.TryParse(idString, out int lastId))
            {
                mLogger.LogError($"Could not parse next id from path {idHolderPath}");
                throw new ArgumentException($"Could not parse next id from path {idHolderPath}");
            }

            mLastId = lastId;
        }

        public string GenerateId()
        {
            string stringID = mLastId.ToString();
            mLastId++;
            return stringID;
        }
    }
}