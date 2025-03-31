using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace WanderlustJournal.Services
{
    public class GeocodingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GeocodingService> _logger;
        
        public GeocodingService(IHttpClientFactory httpClientFactory, ILogger<GeocodingService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        
        public async Task<(decimal? latitude, decimal? longitude)> GeocodeLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return (null, null);
            }
            
            try
            {
                // Create a new HttpClient for each request from the factory
                using var httpClient = _httpClientFactory.CreateClient("NominatimApi");
                
                // Set user agent for this request
                if (!httpClient.DefaultRequestHeaders.Contains("User-Agent"))
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "WanderlustJournal/1.0");
                }
                
                var encodedLocation = Uri.EscapeDataString(location);
                var requestUrl = $"https://nominatim.openstreetmap.org/search?q={encodedLocation}&format=json&limit=1";
                
                // Add a delay to respect Nominatim usage policy (max 1 request per second)
                await Task.Delay(1000);
                
                var response = await httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                
                var results = await response.Content.ReadFromJsonAsync<List<NominatimResult>>();
                
                if (results != null && results.Count > 0)
                {
                    if (decimal.TryParse(results[0].Lat, out var lat) && 
                        decimal.TryParse(results[0].Lon, out var lon))
                    {
                        return (lat, lon);
                    }
                }
                
                return (null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error geocoding location: {location}");
                return (null, null);
            }
        }
        
        private class NominatimResult
        {
            public string Lat { get; set; } = string.Empty;
            public string Lon { get; set; } = string.Empty;
            public string Display_Name { get; set; } = string.Empty;
        }
    }
}