using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace FinanceDiary.Domain.Database.CsvHelper
{
    public class CsvReaderAdapter : IDisposable, IAsyncDisposable
    {
        private bool mIsDisposed;
        private readonly StreamReader mStreamReader;
        public CsvReader CsvReader { get; }

        public CsvReaderAdapter(string csvPath)
        {
            mStreamReader = new StreamReader(csvPath);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

            csvConfiguration.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = new[] { "dd-MM-yyyy" };

            CsvReader = new CsvReader(mStreamReader, csvConfiguration);
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
                mStreamReader.Dispose();
                CsvReader.Dispose();
            }

            mIsDisposed = true;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PdfNetSanitizationServices"/> class.
        /// </summary>
        ~CsvReaderAdapter()
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