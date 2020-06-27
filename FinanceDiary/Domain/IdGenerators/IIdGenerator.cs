using System.Threading.Tasks;

namespace FinanceDiary.Domain.IdGenerators
{
    public interface IIdGenerator
    {
        string GenerateId();
        Task SaveState();
    }
}