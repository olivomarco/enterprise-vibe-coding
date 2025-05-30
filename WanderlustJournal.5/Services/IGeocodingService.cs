using System.Threading.Tasks;

namespace WanderlustJournal.Services
{
    public interface IGeocodingService
    {
        Task<(decimal? latitude, decimal? longitude)> GeocodeLocationAsync(string location);
    }
}