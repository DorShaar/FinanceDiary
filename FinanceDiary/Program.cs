using FinanceDiary.Infra;
using System.Threading;
using System.Threading.Tasks;

namespace FinanceDiary
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await new FinanceDiaryMicroserviceHost().RunAsync(args, CancellationToken.None).ConfigureAwait(false);
        }
    }
}