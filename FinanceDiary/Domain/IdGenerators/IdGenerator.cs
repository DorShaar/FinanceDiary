using FinanceDiary.Domain.Options;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FinanceDiary.Domain.IdGenerators
{
    public class IdGenerator : IIdGenerator
    {
        private const string NextIdHolderName = "id_producer.db";

        private static int mLastId = 0;
        private readonly string mIdHolderPath;

        public IdGenerator(IOptionsMonitor<DatabaseConfiguration> configuration)
        {
            mIdHolderPath = Path.Combine(configuration.CurrentValue.DatabasePath, NextIdHolderName);

            string idString = File.ReadAllText(mIdHolderPath);
            if (!int.TryParse(idString, out int lastId))
            {
                throw new ArgumentException($"Could not parse next id from path {mIdHolderPath}");
            }

            mLastId = lastId;
        }

        public string GenerateId()
        {
            string stringID = mLastId.ToString();
            mLastId++;
            return stringID;
        }

        public async Task SaveState()
        {
            await File.WriteAllTextAsync(mIdHolderPath, mLastId.ToString()).ConfigureAwait(false);
        }
    }
}