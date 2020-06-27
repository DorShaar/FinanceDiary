using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace FinanceDiary.Domain.Database.CsvHelper
{
    public class CsvWriterAdapter : IDisposable, IAsyncDisposable
    {
        private bool mIsDisposed;
        private readonly StreamWriter mStreamWriter;
        private readonly CsvWriter mCsvWriter;

        public CsvWriterAdapter(string csvPath)
        {
            mStreamWriter = new StreamWriter(csvPath);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            csvConfiguration.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = new[] { "dd-MM-yyyy" };

            mCsvWriter = new CsvWriter(mStreamWriter, csvConfiguration);
        }

        public async Task Write<T>(IEnumerable<T> enumerable)
        {
            await mCsvWriter.WriteRecordsAsync(enumerable);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mIsDisposed)
                return;

            if (disposing)
            {
                mStreamWriter.Dispose();
                mCsvWriter.Dispose();
            }

            mIsDisposed = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PdfNetSanitizationServices"/> class.
        /// </summary>
        ~CsvWriterAdapter()
        {
            Dispose(false);
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return default;
        }
    }
}